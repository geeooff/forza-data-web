using ForzaData.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ForzaData.Console
{
	public class ForzaDataConsole : ForzaDataObserver
	{
		private static readonly object SyncRoot = new object();

		private const ConsoleColor DefaultValueColor = ConsoleColor.White;
		private const ConsoleColor DefaultEngineLowRpmColor = ConsoleColor.Green;
		private const ConsoleColor DefaultEngineMediumRpmColor = ConsoleColor.Yellow;
		private const ConsoleColor DefaultEngineHighRpmColor = ConsoleColor.Red;
		private const float DefaultEngineMediumRpmPercent = 0.5f; // percent of maximum rpm
		private const float DefaultEngineHighRpmPercent = 0.85f; // percent of maximum rpm

		public ForzaDataConsole()
		{

		}

		public virtual ConsoleColor ValueColor { get; set; } = DefaultValueColor;
		public virtual ConsoleColor EngineLowRpmColor { get; set; } = DefaultEngineLowRpmColor;
		public virtual ConsoleColor EngineMediumRpmColor { get; set; } = DefaultEngineMediumRpmColor;
		public virtual ConsoleColor EngineHighRpmColor { get; set; } = DefaultEngineHighRpmColor;
		public virtual float EngineMediumRpmPercent { get; set; } = DefaultEngineMediumRpmPercent;
		public virtual float EngineHighRpmPercent { get; set; } = DefaultEngineHighRpmPercent;

		public override void Subscribe(ForzaDataListener listener)
		{
			base.Subscribe(listener);
			InitializeUI();
		}

		public override void OnCompleted()
		{
			System.Console.Clear();
			System.Console.Error.WriteLine("Listening completed");
		}

		public override void OnError(Exception error)
		{
			System.Console.Clear();
			System.Console.Error.WriteLine(error);
		}

		public override void OnNext(ForzaDataStruct value)
		{
			UpdateUI(value);
		}

		protected virtual void InitializeUI()
		{
			lock (SyncRoot)
			{
				System.Console.Clear();
				System.Console.CursorTop = 0;
				System.Console.CursorLeft = 0;
				System.Console.CursorVisible = false;

				System.Console.WriteLine("Race XXX Car XXXXXXXXXXX Class X Index XXX Drivetrain XXX Cylinders XX");
				System.Console.WriteLine("Lap XXX Position XX Fuel XXXXX % Distance XXXXX KM Race X.XX:XX:XX.XXX");
				System.Console.WriteLine();
				System.Console.WriteLine("Current XXXXX RPM   Speed  XXXXX.X KPH   Cur. lap X.XX:XX:XX.XXX");
				System.Console.WriteLine("Idle    XXXXX RPM   Power  XXXXX.X KW    Last lap X.XX:XX:XX.XXX");
				System.Console.WriteLine("Maximum XXXXX RPM   Torque XXXXX.X Nm    Best lap X.XX:XX:XX.XXX");
				System.Console.WriteLine();
				System.Console.WriteLine("                X(right)       Y(up)  Z(forward)   Accelerator XXX %");
				System.Console.WriteLine("Acceleration XXXXXXXXXXX XXXXXXXXXXX XXXXXXXXXXX   Brake       XXX %");
				System.Console.WriteLine("Velocity     XXXXXXXXXXX XXXXXXXXXXX XXXXXXXXXXX   Clutch      XXX %");
				System.Console.WriteLine("                                                   Handbrake   XXX %");
				System.Console.WriteLine("                     Yaw       Pitch        Roll   Gear        XXX");
				System.Console.WriteLine("Position     XXXXXXXXXXX XXXXXXXXXXX XXXXXXXXXXX   Steer       XXX %");
				System.Console.WriteLine("Ang.Velocity XXXXXXXXXXX XXXXXXXXXXX XXXXXXXXXXX");
			}
		}

		protected virtual void UpdateUI(ForzaDataStruct data)
		{
			ForzaSledDataStruct sd = data.Sled;

			lock (SyncRoot)
			{
				// line 1
				ConsoleWriteAt(5, 0, GetRaceIsOneValue(sd.IsRaceOn), 3, GetRaceIsOneValueColor(sd.IsRaceOn));
				ConsoleWriteAt(13, 0, $"{sd.CarOrdinal,11:##0}", 11);
				ConsoleWriteAt(31, 0, "DCBASRPX"[sd.CarClass].ToString(), 1);
				ConsoleWriteAt(39, 0, $"{sd.CarPerformanceIndex,3:##0}", 3);
				ConsoleWriteAt(54, 0, GetDriveTrainValue(sd.DrivetrainType), 3);
				ConsoleWriteAt(68, 0, string.Format("{0:#0}", sd.NumCylinders), 2);

				// engine
				ConsoleWriteAt(8, 3, $"{sd.CurrentEngineRpm,5:####0}", 5, GetCurrentEngineRpmValueColor(sd.CurrentEngineRpm, sd.EngineIdleRpm, sd.EngineMaxRpm));
				ConsoleWriteAt(8, 4, $"{sd.EngineIdleRpm,5:####0}", 5);
				ConsoleWriteAt(8, 5, $"{sd.EngineMaxRpm,5:####0}", 5);

				// acceleration
				ConsoleWriteAt(13, 8, $"{sd.AccelerationX,11:###0.000000}", 11);
				ConsoleWriteAt(25, 8, $"{sd.AccelerationY,11:###0.000000}", 11);
				ConsoleWriteAt(37, 8, $"{sd.AccelerationZ,11:###0.000000}", 11);

				// velocity
				ConsoleWriteAt(13, 9, $"{sd.VelocityX,11:###0.000000}", 11);
				ConsoleWriteAt(25, 9, $"{sd.VelocityY,11:###0.000000}", 11);
				ConsoleWriteAt(37, 9, $"{sd.VelocityZ,11:###0.000000}", 11);

				// angle
				ConsoleWriteAt(13, 12, $"{sd.Yaw,11:###0.000000}", 11);
				ConsoleWriteAt(25, 12, $"{sd.Pitch,11:###0.000000}", 11);
				ConsoleWriteAt(37, 12, $"{sd.Roll,11:###0.000000}", 11);

				// angular velocity
				ConsoleWriteAt(13, 13, $"{sd.AngularVelocityX,11:###0.000000}", 11);
				ConsoleWriteAt(25, 13, $"{sd.AngularVelocityY,11:###0.000000}", 11);
				ConsoleWriteAt(37, 13, $"{sd.AngularVelocityZ,11:###0.000000}", 11);

				if (data.CarDash.HasValue)
				{
					ForzaCarDashDataStruct cdd = data.CarDash.Value;

					// line 2
					ConsoleWriteAt(4, 1, $"{cdd.LapNumber + 1}", 3);
					ConsoleWriteAt(17, 1, $"{cdd.RacePosition}", 2);
					ConsoleWriteAt(25, 1, $"{cdd.Fuel * 100f,5:##0.0}", 5);
					ConsoleWriteAt(42, 1, $"{cdd.DistanceTraveled / 1000f,5:##0.0}", 5);
					ConsoleWriteAt(56, 1, GetRaceTimeValue(cdd.CurrentRaceTime), 14);

					// speed, power, torque
					ConsoleWriteAt(27, 3, $"{cdd.Speed * 3.6f,7:####0.0}", 7);
					ConsoleWriteAt(27, 4, $"{cdd.Power / 1000f,7:####0.0}", 7);
					ConsoleWriteAt(27, 5, $"{cdd.Torque,7:####0.0}", 7);

					// laps
					ConsoleWriteAt(50, 3, GetRaceTimeValue(cdd.CurrentLap), 14);
					ConsoleWriteAt(50, 4, GetRaceTimeValue(cdd.LastLap), 14);
					ConsoleWriteAt(50, 5, GetRaceTimeValue(cdd.BestLap), 14);

					// controls
					ConsoleWriteAt(63, 7, $"{cdd.Accel / 2.55f,3:##0}", 3);
					ConsoleWriteAt(63, 8, $"{cdd.Brake / 2.55f,3:##0}", 3);
					ConsoleWriteAt(63, 9, $"{cdd.Clutch / 2.55f,3:##0}", 3);
					ConsoleWriteAt(63, 10, $"{cdd.HandBrake / 2.55f,3:##0}", 3);
					ConsoleWriteAt(63, 11, $"{cdd.Gear,3:##0}", 3);
					ConsoleWriteAt(62, 12, $"{cdd.Steer / 1.27f,3:+0;-0;0}", 4); 
				}
			}
		}

		protected virtual string GetRaceIsOneValue(int isRaceOn)
		{
			return (isRaceOn == 1) ? " ON" : "OFF";
		}

		protected virtual ConsoleColor GetRaceIsOneValueColor(int isRaceOn)
		{
			return (isRaceOn == 1) ? ConsoleColor.Green : ConsoleColor.Red;
		}

		protected virtual string GetDriveTrainValue(int driveTrainType)
		{
			switch (driveTrainType)
			{
				case 0: return "FWD";
				case 1: return "RWD";
				case 2: return "AWD";
				default: return "   ";
			}
		}

		protected virtual string GetRaceTimeValue(float time)
		{
			TimeSpan ts = TimeSpan.FromSeconds(time);

			string format = ts.Days > 0
				? "d'.'hh':'mm':'ss'.'fff"
				: ts.Hours > 0
					? "hh':'mm':'ss'.'fff"
					: ts.Minutes > 0
						? "mm':'ss'.'fff"
						: "ss'.'fff";

			return ts.ToString(format).TrimStart('0');
		}

		protected virtual ConsoleColor GetCurrentEngineRpmValueColor(float currentEngineRpm, float engineIdleRpm, float engineMaxRpm)
		{
			if (engineIdleRpm == 0f || engineMaxRpm == 0f)
				return ValueColor;

			float mediumRpmBound = (engineMaxRpm * EngineMediumRpmPercent);
			float highRpmBound = (engineMaxRpm * EngineHighRpmPercent);

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

		protected void ConsoleWriteAt(int left, int top, string value, int length, ConsoleColor? color = null)
		{
			int bufferHeight = System.Console.BufferHeight;
			int bufferWidth = System.Console.BufferWidth;

			// avoid buffer-overflow writing
			if (top > bufferHeight || left > bufferWidth)
				return;

			System.Console.SetCursorPosition(left, top);

			int availableChars = Math.Min(length, (bufferWidth - left));
			if (value.Length > availableChars)
			{
				Debug.WriteLine($"Substring occured on value \"{value}\" because of {availableChars} available chars");

				// substring value according buffer width or value max length
				value = value.Substring(0, availableChars);
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
	}
}
