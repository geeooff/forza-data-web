namespace ForzaData.Console;

public class ForzaDataConsole : ForzaDataObserver
{
	private static readonly object SyncRoot = new();

	private static readonly string BlankScreen = @"
╔════════════════════════════════════════════════════════════════════════╗
║ Car  XXXXXXX    Class XX   Index XXX    Drivetrain XXX   Cylinders  XX ║
║ Lap     XXXX     Position XXX     Fuel XXX.X %      Distance XXXXXX KM ║
╟──────────────────────┬─────────────────────┬───────────────────────────╢
║ Current   XXXXXX RPM │ Speed  XXXXX.X km/h │ Current lap XXX:XX:XX,XXX ║
║ Idle      XXXXXX RPM │ Power  XXXXX.X kW   │ Last lap    XXX:XX:XX,XXX ║
║ Maximum   XXXXXX RPM │ Torque XXXXX.X Nm   │ Best lap    XXX:XX:XX,XXX ║
║                      │ Boost  XXXX.XX bar  │ Race        XXX:XX:XX,XXX ║
╟──────────────────────┴─────────────────────┴─────┬─────────────────────╢
║                 X(right)       Y(up)  Z(forward) │ Accelerator   XXX % ║
║ Acceleration  XXXX.XX  G  XXXX.XX  G  XXXX.XX  G │ Brake         XXX % ║
║ Velocity      XXXX.X m/s  XXXX.X m/s  XXXX.X m/s │ Clutch        XXX % ║
╟──────────────────────────────────────────────────┤ Handbrake     XXX % ║
║                      Yaw       Pitch        Roll │ Gear           XX   ║
║ Position     XXXXXXXXXXX XXXXXXXXXXX XXXXXXXXXXX │ Steer         XXX % ║
║ Ang.Velocity XXXXXXXXXXX XXXXXXXXXXX XXXXXXXXXXX │                     ║
╚══════════════════════════════════════════════════╧═════════════════════╝
".Trim();

	private static readonly (int width, int height) BlankScreenSize = GetMinimumSize(BlankScreen);

	private const ConsoleColor DefaultValueColor = ConsoleColor.White;
	private const ConsoleColor DefaultEngineLowRpmColor = ConsoleColor.Green;
	private const ConsoleColor DefaultEngineMediumRpmColor = ConsoleColor.Yellow;
	private const ConsoleColor DefaultEngineHighRpmColor = ConsoleColor.Red;
	private const float DefaultEngineMediumRpmPercent = 0.5f; // percent of maximum rpm
	private const float DefaultEngineHighRpmPercent = 0.85f; // percent of maximum rpm
	private const ConsoleColor DefaultGearNeutralColor = ConsoleColor.Yellow;
	private const ConsoleColor DefaultGearDriveColor = ConsoleColor.Green;
	private const ConsoleColor DefaultGearReverseColor = ConsoleColor.Red;
	private const float PsiToBarDivisor = 14.503773773022f;
	private const float StandardGravity = 9.80665f; // m/s²

	private ForzaDataStruct? previousData;
	private bool hadEnoughSpace;

	public ForzaDataConsole()
	{
		hadEnoughSpace = true;
	}

	public virtual ConsoleColor ValueColor { get; set; } = DefaultValueColor;
	public virtual ConsoleColor EngineLowRpmColor { get; set; } = DefaultEngineLowRpmColor;
	public virtual ConsoleColor EngineMediumRpmColor { get; set; } = DefaultEngineMediumRpmColor;
	public virtual ConsoleColor EngineHighRpmColor { get; set; } = DefaultEngineHighRpmColor;
	public virtual float EngineMediumRpmPercent { get; set; } = DefaultEngineMediumRpmPercent;
	public virtual float EngineHighRpmPercent { get; set; } = DefaultEngineHighRpmPercent;
	public virtual ConsoleColor GearNeutralColor { get; set; } = DefaultGearNeutralColor;
	public virtual ConsoleColor GearDriveColor { get; set; } = DefaultGearDriveColor;
	public virtual ConsoleColor GearReverseColor { get; set; } = DefaultGearReverseColor;

	public override void OnCompleted()
	{
		lock (SyncRoot)
		{
			ClearUI();
			System.Console.Error.WriteLine("Listening completed");
		}
	}

	public override void OnError(ForzaDataException error)
	{
		lock (SyncRoot)
		{
			ClearUI();
			System.Console.Error.WriteLine(error);
		}
	}

	public override void OnError(Exception error)
	{
		lock (SyncRoot)
		{
			ClearUI();
			System.Console.Error.WriteLine(error);
		}
	}

	public override void OnNext(ForzaDataStruct value)
	{
		lock (SyncRoot)
		{
			bool hasEnoughSpace = HasEnoughSpace();

			if (hasEnoughSpace)
			{
				if (ShouldResetUI(value) || !hadEnoughSpace)
					InitializeUI();

				UpdateUI(value);
			}
			else if (hadEnoughSpace)
			{
				ShowNotEnoughWindowSpaceMessage();
			}

			previousData = value;
			hadEnoughSpace = hasEnoughSpace;
		}
	}

	protected virtual void ClearUI()
	{
		System.Console.Clear();
		System.Console.CursorTop = 0;
		System.Console.CursorLeft = 0;
		System.Console.CursorVisible = false;
	}

	protected virtual void InitializeUI()
	{
		ClearUI();
		System.Console.Write(BlankScreen);
	}

	protected virtual bool ShouldResetUI(ForzaDataStruct data)
	{
		// first time receiving data
		if (!previousData.HasValue)
			return true;

		// the UI should clean old values when data version has changed
		// or when race is paused/resumed
		return (previousData.Value.Version != data.Version)
			|| (previousData.Value.Sled.IsRaceOn != data.Sled.IsRaceOn);
	}

	protected virtual bool HasEnoughSpace()
	{
		(int width, int height) = (
			Math.Min(System.Console.BufferWidth, System.Console.WindowWidth),
			Math.Min(System.Console.BufferHeight, System.Console.WindowHeight)
		);

		return width >= BlankScreenSize.width
			&& height >= BlankScreenSize.height;
	}

	protected virtual void ShowNotEnoughWindowSpaceMessage()
	{
		ClearUI();
		System.Console.WriteLine("Not enough space to display the UI.");
		System.Console.WriteLine($"Please set your console to at least {BlankScreenSize.width}x{BlankScreenSize.height} chars");
	}

	protected virtual void UpdateUI(ForzaDataStruct data)
	{
		ForzaSledDataStruct sd = data.Sled;

		// race on/off
		string raceIsOnValue = ' ' + GetRaceIsOneValue(sd.IsRaceOn) + ' ';
		ConsoleWriteAt(3, 0, raceIsOnValue, raceIsOnValue.Length, GetRaceIsOneValueColor(sd.IsRaceOn));

		// we don't display any other value if race is not on
		if (sd.IsRaceOn == 0)
			return;

		// header, line 1
		ConsoleWriteAt(7, 1, $"{sd.CarOrdinal,7:0}", 7);
		ConsoleWriteAt(24, 1, GetCarClassValue(sd.CarClass, sd.CarPerformanceIndex) ?? "XX", 2);
		ConsoleWriteAt(35, 1, $"{sd.CarPerformanceIndex,3:0}", 3);
		ConsoleWriteAt(53, 1, GetDriveTrainValue(sd.DrivetrainType), 3);
		ConsoleWriteAt(70, 1, $"{sd.NumCylinders,2:0}", 2);

		// engine, rotation per minute
		ConsoleWriteAt(12, 4, $"{sd.CurrentEngineRpm,6:0}", 6, GetCurrentEngineRpmValueColor(sd.CurrentEngineRpm, sd.EngineMaxRpm));
		ConsoleWriteAt(12, 5, $"{sd.EngineIdleRpm,6:0}", 6);
		ConsoleWriteAt(12, 6, $"{sd.EngineMaxRpm,6:0}", 6);

		// acceleration, m/s² -> Gs
		ConsoleWriteAt(16, 10, $"{sd.AccelerationX / StandardGravity,7:'◄ '#0.00;'► '#0.00;0.00}", 7);
		ConsoleWriteAt(28, 10, $"{sd.AccelerationY / StandardGravity,7:'▼ '#0.00;'▲ '#0.00;0.00}", 7);
		ConsoleWriteAt(40, 10, $"{sd.AccelerationZ / StandardGravity,7:'▼ '#0.00;'▲ '#0.00;0.00}", 7);

		// velocity, m/s
		ConsoleWriteAt(16, 11, $"{sd.VelocityX,6:'◄ '##0.0;'► '##0.0;0.0}", 6);
		ConsoleWriteAt(28, 11, $"{sd.VelocityY,6:'▲ '##0.0;'▼ '##0.0;0.0}", 6);
		ConsoleWriteAt(40, 11, $"{sd.VelocityZ,6:'▲ '##0.0;'▼ '##0.0;0.0}", 6);

		// position
		// TODO: need to figure the unit
		ConsoleWriteAt(15, 14, $"{sd.Yaw,11:0.000000}", 11);
		ConsoleWriteAt(27, 14, $"{sd.Pitch,11:0.000000}", 11);
		ConsoleWriteAt(39, 14, $"{sd.Roll,11:0.000000}", 11);

		// angular velocity
		// TODO: need to figure the unit
		ConsoleWriteAt(15, 15, $"{sd.AngularVelocityX,11:0.000000}", 11);
		ConsoleWriteAt(27, 15, $"{sd.AngularVelocityY,11:0.000000}", 11);
		ConsoleWriteAt(39, 15, $"{sd.AngularVelocityZ,11:0.000000}", 11);

		if (data.CarDash.HasValue)
		{
			ForzaCarDashDataStruct cdd = data.CarDash.Value;

			// header, line 2
			ConsoleWriteAt(10, 2, $"{cdd.LapNumber + 1}", 4);
			ConsoleWriteAt(28, 2, $"{cdd.RacePosition}", 3);
			ConsoleWriteAt(41, 2, $"{cdd.Fuel * 100f,5:0.0}", 5);
			ConsoleWriteAt(63, 2, $"{cdd.DistanceTraveled / 1000f,6:0.0}", 6);

			// speed (m/s -> km/h), power (watts -> killowatts), torque (newton-meter), boost (psi -> bar)
			ConsoleWriteAt(32, 4, $"{cdd.Speed * 3.6f,7:0.0}", 7);
			ConsoleWriteAt(32, 5, $"{cdd.Power / 1000f,7:0.0}", 7);
			ConsoleWriteAt(32, 6, $"{cdd.Torque,7:0.0}", 7);
			ConsoleWriteAt(32, 7, $"{cdd.Boost / PsiToBarDivisor,7:0.00}", 7);

			// times
			ConsoleWriteAt(59, 4, GetRaceTimeValue(cdd.CurrentLap), 13);
			ConsoleWriteAt(59, 5, GetRaceTimeValue(cdd.LastLap), 13);
			ConsoleWriteAt(59, 6, GetRaceTimeValue(cdd.BestLap), 13);
			ConsoleWriteAt(59, 7, GetRaceTimeValue(cdd.CurrentRaceTime), 13);

			// controls, in percentages
			ConsoleWriteAt(67, 9, $"{cdd.Accel / 2.55f,3:0}", 3);
			ConsoleWriteAt(67, 10, $"{cdd.Brake / 2.55f,3:0}", 3);
			ConsoleWriteAt(67, 11, $"{cdd.Clutch / 2.55f,3:0}", 3);
			ConsoleWriteAt(67, 12, $"{cdd.HandBrake / 2.55f,3:0}", 3);
			ConsoleWriteAt(68, 13, GetGearValue(cdd.Gear), 2, GetGearValueColor(cdd.Gear));
			ConsoleWriteAt(65, 14, $"{cdd.Steer / 1.27f,5:'► '##0;'◄ '##0;0}", 5);
		}
	}

	protected virtual string GetRaceIsOneValue(int isRaceOn) => isRaceOn switch
	{
		0 => "PAUSE",
		1 => "RACE",
		_ => "???"
	};

	protected virtual ConsoleColor GetRaceIsOneValueColor(int isRaceOn) => isRaceOn == 0
		? ConsoleColor.Red
		: ConsoleColor.Green;

	protected virtual string GetDriveTrainValue(int driveTrainType) => driveTrainType switch
	{
		0 => "FWD",
		1 => "RWD",
		2 => "AWD",
		_ => string.Empty
	};

	protected virtual string GetRaceTimeValue(float time)
	{
		TimeSpan timeSpan = TimeSpan.FromSeconds(time);

		string timeString = timeSpan.ToString("mm':'ss','fff");

		if (timeSpan.TotalHours >= 1d)
		{
			ushort totalHours = (ushort)Math.Floor(timeSpan.TotalHours);
			string hoursString = totalHours.ToString("000").TrimStart('0');
			timeString = string.Concat(hoursString, ':', timeString);
		}

		return timeString;
	}

	protected virtual ConsoleColor GetCurrentEngineRpmValueColor(float currentEngineRpm, float engineMaxRpm)
	{
		if (engineMaxRpm == 0f)
			return ValueColor;

		float mediumRpmBound = engineMaxRpm * EngineMediumRpmPercent;
		float highRpmBound = engineMaxRpm * EngineHighRpmPercent;

		if (currentEngineRpm < mediumRpmBound)
		{
			return EngineLowRpmColor;
		}
		else if (currentEngineRpm >= highRpmBound)
		{
			return EngineHighRpmColor;
		}
		else
		{
			return EngineMediumRpmColor;
		}
	}

	protected virtual string GetGearValue(byte value) => value switch
	{
		0 => "R",
		11 => "N",
		_ => value.ToString("0")
	};

	protected virtual ConsoleColor GetGearValueColor(byte value) => value switch
	{
		0 => GearReverseColor,
		11 => GearNeutralColor,
		_ => GearDriveColor
	};

	protected virtual string? GetCarClassValue(int carClass, int carPerformance)
		=> CarClasses.GetCarClassCode(carClass, carPerformance);

	protected void ConsoleWriteAt(int left, int top, char value, ConsoleColor? color = null)
		=> ConsoleWriteAt(left, top, value.ToString(), 1, color);

	protected void ConsoleWriteAt(int left, int top, string value, int length, ConsoleColor? color = null)
	{
		int windowHeight = System.Console.WindowHeight;
		int windowWidth = System.Console.WindowWidth;

		// avoid buffer-overflow writing
		if (top > windowHeight || left > windowWidth)
			return;

		System.Console.SetCursorPosition(left, top);

		int availableChars = Math.Min(length, windowWidth - left);
		if (value.Length > availableChars)
		{
			Debug.WriteLine($"Substring occured on value \"{value}\" because of {availableChars} available chars");

			// substring value according buffer width or value max length
			value = value[..availableChars];
		}
		else if (value.Length < availableChars)
		{
			// right-align value
			value = value.PadLeft(availableChars);
		}

		// set foreground color and retain previous one
		ConsoleColor fgColor = color ?? ValueColor;
		ConsoleColor? prevFgColor = null;
		if (System.Console.ForegroundColor != fgColor)
		{
			prevFgColor = System.Console.ForegroundColor;
			System.Console.ForegroundColor = fgColor;
		}

		System.Console.Write(value);

		// restore previous color
		if (prevFgColor.HasValue)
		{
			System.Console.ForegroundColor = prevFgColor.Value;
		}
	}

	private static (int width, int height) GetMinimumSize(string value)
	{
		var lines = value.Split('\n').Select(line => line.Trim('\r'));
		return (
			lines.Max(line => line.Length),
			lines.Count()
		);
	}
}
