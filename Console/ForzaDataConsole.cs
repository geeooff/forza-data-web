using ForzaData.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForzaData.Console
{
	public class ForzaDataConsole : ForzaDataObserver
	{
		public ForzaDataConsole()
		{
			
		}

		public override void Subscribe(ForzaDataListener listener)
		{
			base.Subscribe(listener);
			InitializeUI();
		}

		public override void OnCompleted()
		{
			System.Console.Error.WriteLine("Listening completed");
		}

		public override void OnError(Exception error)
		{
			System.Console.Error.WriteLine(error);
		}

		public override void OnNext(ForzaDataStruct value)
		{
			UpdateUI(value);
		}

		private void InitializeUI()
		{
			System.Console.Clear();
			System.Console.CursorTop = 0;
			System.Console.CursorLeft = 0;
			System.Console.CursorVisible = false;

			System.Console.WriteLine("Race XXX Car XXXXXXXXXXX Class X Index XXX Drivetrain XXX Cylinders X");
			System.Console.WriteLine();
			System.Console.WriteLine("Current : XXXXX RPM");
			System.Console.WriteLine("Idle    : XXXXX RPM");
			System.Console.WriteLine("Maximum : XXXXX RPM");
			System.Console.WriteLine();
			System.Console.WriteLine("                 X(right)      Y(up) Z(forward)");
			System.Console.WriteLine("Acceleration : XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX");
			System.Console.WriteLine("Velocity     : XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX");
			System.Console.WriteLine();
			System.Console.WriteLine("                      Yaw      Pitch       Roll");
			System.Console.WriteLine("Position     : XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX");
			System.Console.WriteLine("Ang.Velocity : XXXXXXXXXX XXXXXXXXXX XXXXXXXXXX");
		}

		private void UpdateUI(ForzaDataStruct data)
		{
			bool isRaceOn = (data.IsRaceOn == 1);
			string raceValue = isRaceOn ? " ON" : "OFF";
			ConsoleColor raceColor = isRaceOn ? ConsoleColor.Green : ConsoleColor.Red;

			ConsoleWriteAt(5, 0, raceValue, raceColor);
			ConsoleWriteAt(13, 0, $"{data.CarOrdinal,11:##0}");
			ConsoleWriteAt(31, 0, "DCBASRPX"[data.CarClass].ToString());
			ConsoleWriteAt(39, 0, $"{data.CarPerformanceIndex,3:##0}");

			string drivetrainValue;
			switch (data.DrivetrainType)
			{
				case 0: drivetrainValue = "FWD"; break;
				case 1: drivetrainValue = "RWD"; break;
				case 2: drivetrainValue = "AWD"; break;
				default: drivetrainValue = "   "; break;
			}

			ConsoleWriteAt(54, 0, drivetrainValue);
			ConsoleWriteAt(68, 0, string.Format("{0:#}", data.NumCylinders));

			ConsoleWriteAt(10, 2, $"{data.CurrentEngineRpm,5:####0}");
			ConsoleWriteAt(10, 3, $"{data.EngineIdleRpm,5:####0}");
			ConsoleWriteAt(10, 4, $"{data.EngineMaxRpm,5:####0}");

			ConsoleWriteAt(15, 7, $"{data.AccelerationX,10:F6}");
			ConsoleWriteAt(26, 7, $"{data.AccelerationY,10:F6}");
			ConsoleWriteAt(37, 7, $"{data.AccelerationZ,10:F6}");

			ConsoleWriteAt(15, 8, $"{data.VelocityX,10:F6}");
			ConsoleWriteAt(26, 8, $"{data.VelocityY,10:F6}");
			ConsoleWriteAt(37, 8, $"{data.VelocityZ,10:F6}");

			ConsoleWriteAt(15, 11, $"{data.Yaw,10:F6}");
			ConsoleWriteAt(26, 11, $"{data.Pitch,10:F6}");
			ConsoleWriteAt(37, 11, $"{data.Roll,10:F6}");

			ConsoleWriteAt(15, 12, $"{data.AngularVelocityX,10:F6}");
			ConsoleWriteAt(26, 12, $"{data.AngularVelocityY,10:F6}");
			ConsoleWriteAt(37, 12, $"{data.AngularVelocityZ,10:F6}");
		}

		private void ConsoleWriteAt(int left, int top, string value, ConsoleColor color = ConsoleColor.Yellow)
		{
			System.Console.SetCursorPosition(left, top);

			if (System.Console.ForegroundColor != color)
			{
				System.Console.ForegroundColor = color;
			}

			System.Console.Write(value);
		}
	}
}
