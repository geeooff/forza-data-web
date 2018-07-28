using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ForzaData.Core
{
    public class ForzaDataReader
    {
		public ForzaDataReader()
		{

		}

		public bool TryRead(byte[] input, out ForzaDataStruct output)
		{
			try
			{
				output = Read(input);
				return true;
			}
			catch
			{
				output = new ForzaDataStruct();
				return false;
			}
		}

		public ForzaDataStruct Read(byte[] input)
		{
			ForzaDataStruct output = new ForzaDataStruct();

			using (MemoryStream stream = new MemoryStream(input))
			using (BinaryReader reader = new BinaryReader(stream))
			{
				output.IsRaceOn = reader.ReadInt32();

				output.TimestampMS = reader.ReadUInt32();

				output.EngineMaxRpm = reader.ReadSingle();
				output.EngineIdleRpm = reader.ReadSingle();
				output.CurrentEngineRpm = reader.ReadSingle();

				output.AccelerationX = reader.ReadSingle();
				output.AccelerationY = reader.ReadSingle();
				output.AccelerationZ = reader.ReadSingle();

				output.VelocityX = reader.ReadSingle();
				output.VelocityY = reader.ReadSingle();
				output.VelocityZ = reader.ReadSingle();

				output.AngularVelocityX = reader.ReadSingle();
				output.AngularVelocityY = reader.ReadSingle();
				output.AngularVelocityZ = reader.ReadSingle();

				output.Yaw = reader.ReadSingle();
				output.Pitch = reader.ReadSingle();
				output.Roll = reader.ReadSingle();

				output.NormalizedSuspensionTravelFrontLeft = reader.ReadSingle();
				output.NormalizedSuspensionTravelFrontRight = reader.ReadSingle();
				output.NormalizedSuspensionTravelRearLeft = reader.ReadSingle();
				output.NormalizedSuspensionTravelRearRight = reader.ReadSingle();

				output.TireSlipRatioFrontLeft = reader.ReadSingle();
				output.TireSlipRatioFrontRight = reader.ReadSingle();
				output.TireSlipRatioRearLeft = reader.ReadSingle();
				output.TireSlipRatioRearRight = reader.ReadSingle();

				output.WheelRotationSpeedFrontLeft = reader.ReadSingle();
				output.WheelRotationSpeedFrontRight = reader.ReadSingle();
				output.WheelRotationSpeedRearLeft = reader.ReadSingle();
				output.WheelRotationSpeedRearRight = reader.ReadSingle();

				output.WheelOnRumbleStripFrontLeft = reader.ReadInt32();
				output.WheelOnRumbleStripFrontRight = reader.ReadInt32();
				output.WheelOnRumbleStripRearLeft = reader.ReadInt32();
				output.WheelOnRumbleStripRearRight = reader.ReadInt32();

				output.WheelInPuddleDepthFrontLeft = reader.ReadSingle();
				output.WheelInPuddleDepthFrontRight = reader.ReadSingle();
				output.WheelInPuddleDepthRearLeft = reader.ReadSingle();
				output.WheelInPuddleDepthRearRight = reader.ReadSingle();

				output.SurfaceRumbleFrontLeft = reader.ReadSingle();
				output.SurfaceRumbleFrontRight = reader.ReadSingle();
				output.SurfaceRumbleRearLeft = reader.ReadSingle();
				output.SurfaceRumbleRearRight = reader.ReadSingle();

				output.TireSlipAngleFrontLeft = reader.ReadSingle();
				output.TireSlipAngleFrontRight = reader.ReadSingle();
				output.TireSlipAngleRearLeft = reader.ReadSingle();
				output.TireSlipAngleRearRight = reader.ReadSingle();

				output.TireCombinedSlipFrontLeft = reader.ReadSingle();
				output.TireCombinedSlipFrontRight = reader.ReadSingle();
				output.TireCombinedSlipRearLeft = reader.ReadSingle();
				output.TireCombinedSlipRearRight = reader.ReadSingle();

				output.SuspensionTravelMetersFrontLeft = reader.ReadSingle();
				output.SuspensionTravelMetersFrontRight = reader.ReadSingle();
				output.SuspensionTravelMetersRearLeft = reader.ReadSingle();
				output.SuspensionTravelMetersRearRight = reader.ReadSingle();

				output.CarOrdinal = reader.ReadInt32();
				output.CarClass = reader.ReadInt32();
				output.CarPerformanceIndex = reader.ReadInt32();
				output.DrivetrainType = reader.ReadInt32();
				output.NumCylinders = reader.ReadInt32();
			}

			return output;
		}
    }
}
