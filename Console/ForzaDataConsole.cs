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
				System.Console.WriteLine();
				System.Console.WriteLine("Current XXXXX RPM");
				System.Console.WriteLine("Idle    XXXXX RPM");
				System.Console.WriteLine("Maximum XXXXX RPM");
				System.Console.WriteLine();
				System.Console.WriteLine("                X(right)       Y(up)  Z(forward)");
				System.Console.WriteLine("Acceleration XXXXXXXXXXX XXXXXXXXXXX XXXXXXXXXXX");
				System.Console.WriteLine("Velocity     XXXXXXXXXXX XXXXXXXXXXX XXXXXXXXXXX");
				System.Console.WriteLine();
				System.Console.WriteLine("                     Yaw       Pitch        Roll");
				System.Console.WriteLine("Position     XXXXXXXXXXX XXXXXXXXXXX XXXXXXXXXXX");
				System.Console.WriteLine("Ang.Velocity XXXXXXXXXXX XXXXXXXXXXX XXXXXXXXXXX");
			}
		}

		protected virtual void UpdateUI(ForzaDataStruct data)
		{
			lock (SyncRoot)
			{
				ConsoleWriteAt(5, 0, GetRaceIsOneValue(data.IsRaceOn), 3, GetRaceIsOneValueColor(data.IsRaceOn));
				ConsoleWriteAt(13, 0, $"{data.CarOrdinal,11:##0}", 11);
				ConsoleWriteAt(31, 0, "DCBASRPX"[data.CarClass].ToString(), 1);
				ConsoleWriteAt(39, 0, $"{data.CarPerformanceIndex,3:##0}", 3);
				ConsoleWriteAt(54, 0, GetDriveTrainValue(data.DrivetrainType), 3);
				ConsoleWriteAt(68, 0, string.Format("{0:#0}", data.NumCylinders), 2);

				ConsoleWriteAt(8, 2, $"{data.CurrentEngineRpm,5:####0}", 5, GetCurrentEngineRpmValueColor(data.CurrentEngineRpm, data.EngineIdleRpm, data.EngineMaxRpm));
				ConsoleWriteAt(8, 3, $"{data.EngineIdleRpm,5:####0}", 5);
				ConsoleWriteAt(8, 4, $"{data.EngineMaxRpm,5:####0}", 5);

				ConsoleWriteAt(13, 7, $"{data.AccelerationX,11:###0.000000}", 11);
				ConsoleWriteAt(25, 7, $"{data.AccelerationY,11:###0.000000}", 11);
				ConsoleWriteAt(37, 7, $"{data.AccelerationZ,11:###0.000000}", 11);

				ConsoleWriteAt(13, 8, $"{data.VelocityX,11:###0.000000}", 11);
				ConsoleWriteAt(25, 8, $"{data.VelocityY,11:###0.000000}", 11);
				ConsoleWriteAt(37, 8, $"{data.VelocityZ,11:###0.000000}", 11);

				ConsoleWriteAt(13, 11, $"{data.Yaw,11:###0.000000}", 11);
				ConsoleWriteAt(25, 11, $"{data.Pitch,11:###0.000000}", 11);
				ConsoleWriteAt(37, 11, $"{data.Roll,11:###0.000000}", 11);

				ConsoleWriteAt(13, 12, $"{data.AngularVelocityX,11:###0.000000}", 11);
				ConsoleWriteAt(25, 12, $"{data.AngularVelocityY,11:###0.000000}", 11);
				ConsoleWriteAt(37, 12, $"{data.AngularVelocityZ,11:###0.000000}", 11);
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
