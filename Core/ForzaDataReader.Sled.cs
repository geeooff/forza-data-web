using System;

namespace ForzaData.Core
{
	public partial class ForzaDataReader
	{
		/// <summary>
		/// Map of all sled values
		/// </summary>
		/// <remarks>
		/// See <see cref="SledRange"/> for info.
		/// </remarks>
		internal static class SledMap
		{
			internal static readonly Range IsRaceOn = 0..4;

			internal static readonly Range TimestampMS = 4..8;

			internal static readonly Range EngineMaxRpm = 8..12;
			internal static readonly Range EngineIdleRpm = 12..16;
			internal static readonly Range CurrentEngineRpm = 16..20;

			internal static readonly Range AccelerationX = 20..24;
			internal static readonly Range AccelerationY = 24..28;
			internal static readonly Range AccelerationZ = 28..32;

			internal static readonly Range VelocityX = 32..36;
			internal static readonly Range VelocityY = 36..40;
			internal static readonly Range VelocityZ = 40..44;

			internal static readonly Range AngularVelocityX = 44..48;
			internal static readonly Range AngularVelocityY = 48..52;
			internal static readonly Range AngularVelocityZ = 52..56;

			internal static readonly Range Yaw = 56..60;
			internal static readonly Range Pitch = 60..64;
			internal static readonly Range Roll = 64..68;

			internal static readonly Range NormalizedSuspensionTravelFrontLeft = 68..72;
			internal static readonly Range NormalizedSuspensionTravelFrontRight = 72..76;
			internal static readonly Range NormalizedSuspensionTravelRearLeft = 76..80;
			internal static readonly Range NormalizedSuspensionTravelRearRight = 80..84;

			internal static readonly Range TireSlipRatioFrontLeft = 84..88;
			internal static readonly Range TireSlipRatioFrontRight = 88..92;
			internal static readonly Range TireSlipRatioRearLeft = 92..96;
			internal static readonly Range TireSlipRatioRearRight = 96..100;

			internal static readonly Range WheelRotationSpeedFrontLeft = 100..104;
			internal static readonly Range WheelRotationSpeedFrontRight = 104..108;
			internal static readonly Range WheelRotationSpeedRearLeft = 108..112;
			internal static readonly Range WheelRotationSpeedRearRight = 112..116;

			internal static readonly Range WheelOnRumbleStripFrontLeft = 116..120;
			internal static readonly Range WheelOnRumbleStripFrontRight = 120..124;
			internal static readonly Range WheelOnRumbleStripRearLeft = 124..128;
			internal static readonly Range WheelOnRumbleStripRearRight = 128..132;

			internal static readonly Range WheelInPuddleDepthFrontLeft = 132..136;
			internal static readonly Range WheelInPuddleDepthFrontRight = 136..140;
			internal static readonly Range WheelInPuddleDepthRearLeft = 140..144;
			internal static readonly Range WheelInPuddleDepthRearRight = 144..148;

			internal static readonly Range SurfaceRumbleFrontLeft = 148..152;
			internal static readonly Range SurfaceRumbleFrontRight = 152..156;
			internal static readonly Range SurfaceRumbleRearLeft = 156..160;
			internal static readonly Range SurfaceRumbleRearRight = 160..164;

			internal static readonly Range TireSlipAngleFrontLeft = 164..168;
			internal static readonly Range TireSlipAngleFrontRight = 168..172;
			internal static readonly Range TireSlipAngleRearLeft = 172..176;
			internal static readonly Range TireSlipAngleRearRight = 176..180;

			internal static readonly Range TireCombinedSlipFrontLeft = 180..184;
			internal static readonly Range TireCombinedSlipFrontRight = 184..188;
			internal static readonly Range TireCombinedSlipRearLeft = 188..192;
			internal static readonly Range TireCombinedSlipRearRight = 192..196;

			internal static readonly Range SuspensionTravelMetersFrontLeft = 196..200;
			internal static readonly Range SuspensionTravelMetersFrontRight = 200..204;
			internal static readonly Range SuspensionTravelMetersRearLeft = 204..208;
			internal static readonly Range SuspensionTravelMetersRearRight = 208..212;

			internal static readonly Range CarOrdinal = 212..216;
			internal static readonly Range CarClass = 216..220;
			internal static readonly Range CarPerformanceIndex = 220..224;
			internal static readonly Range DrivetrainType = 224..228;
			internal static readonly Range NumCylinders = 228..232;
		}

		private ForzaSledDataStruct ReadSledData(in ReadOnlySpan<byte> data) => new ForzaSledDataStruct
		{
			IsRaceOn = BitConverter.ToInt32(data[SledMap.IsRaceOn]),

			TimestampMS = BitConverter.ToUInt32(data[SledMap.TimestampMS]),

			EngineMaxRpm = BitConverter.ToSingle(data[SledMap.EngineMaxRpm]),
			EngineIdleRpm = BitConverter.ToSingle(data[SledMap.EngineIdleRpm]),
			CurrentEngineRpm = BitConverter.ToSingle(data[SledMap.CurrentEngineRpm]),

			AccelerationX = BitConverter.ToSingle(data[SledMap.AccelerationX]),
			AccelerationY = BitConverter.ToSingle(data[SledMap.AccelerationY]),
			AccelerationZ = BitConverter.ToSingle(data[SledMap.AccelerationZ]),

			VelocityX = BitConverter.ToSingle(data[SledMap.VelocityX]),
			VelocityY = BitConverter.ToSingle(data[SledMap.VelocityY]),
			VelocityZ = BitConverter.ToSingle(data[SledMap.VelocityZ]),

			AngularVelocityX = BitConverter.ToSingle(data[SledMap.AngularVelocityX]),
			AngularVelocityY = BitConverter.ToSingle(data[SledMap.AngularVelocityY]),
			AngularVelocityZ = BitConverter.ToSingle(data[SledMap.AngularVelocityZ]),

			Yaw = BitConverter.ToSingle(data[SledMap.Yaw]),
			Pitch = BitConverter.ToSingle(data[SledMap.Pitch]),
			Roll = BitConverter.ToSingle(data[SledMap.Roll]),

			NormalizedSuspensionTravelFrontLeft = BitConverter.ToSingle(data[SledMap.NormalizedSuspensionTravelFrontLeft]),
			NormalizedSuspensionTravelFrontRight = BitConverter.ToSingle(data[SledMap.NormalizedSuspensionTravelFrontRight]),
			NormalizedSuspensionTravelRearLeft = BitConverter.ToSingle(data[SledMap.NormalizedSuspensionTravelRearLeft]),
			NormalizedSuspensionTravelRearRight = BitConverter.ToSingle(data[SledMap.NormalizedSuspensionTravelRearRight]),

			TireSlipRatioFrontLeft = BitConverter.ToSingle(data[SledMap.TireSlipRatioFrontLeft]),
			TireSlipRatioFrontRight = BitConverter.ToSingle(data[SledMap.TireSlipRatioFrontRight]),
			TireSlipRatioRearLeft = BitConverter.ToSingle(data[SledMap.TireSlipRatioRearLeft]),
			TireSlipRatioRearRight = BitConverter.ToSingle(data[SledMap.TireSlipRatioRearRight]),

			WheelRotationSpeedFrontLeft = BitConverter.ToSingle(data[SledMap.WheelRotationSpeedFrontLeft]),
			WheelRotationSpeedFrontRight = BitConverter.ToSingle(data[SledMap.WheelRotationSpeedFrontRight]),
			WheelRotationSpeedRearLeft = BitConverter.ToSingle(data[SledMap.WheelRotationSpeedRearLeft]),
			WheelRotationSpeedRearRight = BitConverter.ToSingle(data[SledMap.WheelRotationSpeedRearRight]),

			WheelOnRumbleStripFrontLeft = BitConverter.ToInt32(data[SledMap.WheelOnRumbleStripFrontLeft]),
			WheelOnRumbleStripFrontRight = BitConverter.ToInt32(data[SledMap.WheelOnRumbleStripFrontRight]),
			WheelOnRumbleStripRearLeft = BitConverter.ToInt32(data[SledMap.WheelOnRumbleStripRearLeft]),
			WheelOnRumbleStripRearRight = BitConverter.ToInt32(data[SledMap.WheelOnRumbleStripRearRight]),

			WheelInPuddleDepthFrontLeft = BitConverter.ToSingle(data[SledMap.WheelInPuddleDepthFrontLeft]),
			WheelInPuddleDepthFrontRight = BitConverter.ToSingle(data[SledMap.WheelInPuddleDepthFrontRight]),
			WheelInPuddleDepthRearLeft = BitConverter.ToSingle(data[SledMap.WheelInPuddleDepthRearLeft]),
			WheelInPuddleDepthRearRight = BitConverter.ToSingle(data[SledMap.WheelInPuddleDepthRearRight]),

			SurfaceRumbleFrontLeft = BitConverter.ToSingle(data[SledMap.SurfaceRumbleFrontLeft]),
			SurfaceRumbleFrontRight = BitConverter.ToSingle(data[SledMap.SurfaceRumbleFrontRight]),
			SurfaceRumbleRearLeft = BitConverter.ToSingle(data[SledMap.SurfaceRumbleRearLeft]),
			SurfaceRumbleRearRight = BitConverter.ToSingle(data[SledMap.SurfaceRumbleRearRight]),

			TireSlipAngleFrontLeft = BitConverter.ToSingle(data[SledMap.TireSlipAngleFrontLeft]),
			TireSlipAngleFrontRight = BitConverter.ToSingle(data[SledMap.TireSlipAngleFrontRight]),
			TireSlipAngleRearLeft = BitConverter.ToSingle(data[SledMap.TireSlipAngleRearLeft]),
			TireSlipAngleRearRight = BitConverter.ToSingle(data[SledMap.TireSlipAngleRearRight]),

			TireCombinedSlipFrontLeft = BitConverter.ToSingle(data[SledMap.TireCombinedSlipFrontLeft]),
			TireCombinedSlipFrontRight = BitConverter.ToSingle(data[SledMap.TireCombinedSlipFrontRight]),
			TireCombinedSlipRearLeft = BitConverter.ToSingle(data[SledMap.TireCombinedSlipRearLeft]),
			TireCombinedSlipRearRight = BitConverter.ToSingle(data[SledMap.TireCombinedSlipRearRight]),

			SuspensionTravelMetersFrontLeft = BitConverter.ToSingle(data[SledMap.SuspensionTravelMetersFrontLeft]),
			SuspensionTravelMetersFrontRight = BitConverter.ToSingle(data[SledMap.SuspensionTravelMetersFrontRight]),
			SuspensionTravelMetersRearLeft = BitConverter.ToSingle(data[SledMap.SuspensionTravelMetersRearLeft]),
			SuspensionTravelMetersRearRight = BitConverter.ToSingle(data[SledMap.SuspensionTravelMetersRearRight]),

			CarOrdinal = BitConverter.ToInt32(data[SledMap.CarOrdinal]),
			CarClass = BitConverter.ToInt32(data[SledMap.CarClass]),
			CarPerformanceIndex = BitConverter.ToInt32(data[SledMap.CarPerformanceIndex]),
			DrivetrainType = BitConverter.ToInt32(data[SledMap.DrivetrainType]),
			NumCylinders = BitConverter.ToInt32(data[SledMap.NumCylinders])
		};
	}
}
