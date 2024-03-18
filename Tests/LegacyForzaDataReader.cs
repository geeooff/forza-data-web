namespace ForzaData.Core;

public static class LegacyForzaDataReader
{
	private const int SledPacketSize = 232;
	private const int CarDashPacketSize = SledPacketSize + 79;
	private const int HorizonCarDashPacketSize = SledPacketSize + 92;

	public static bool TryRead(byte[] input, out ForzaDataStruct output)
	{
		try
		{
			output = Read(input);
			return true;
		}
		catch
		{
			output = new ForzaDataStruct()
			{
				Version = ForzaDataVersion.Unknown
			};
			return false;
		}
	}

	public static ForzaDataStruct Read(byte[] input)
	{
		var output = new ForzaDataStruct()
		{
			Version = ReadVersion(input)
		};

		if (output.Version != ForzaDataVersion.Unknown)
		{
			using var stream = new MemoryStream(input);
			using var reader = new BinaryReader(stream);

			// common data
			output.Sled = ReadSledData(reader);

			switch (output.Version)
			{
				// forza motorsport 7 car dash data
				case ForzaDataVersion.CarDashV1:
					output.CarDash = ReadCarDashData(reader);
					break;

				// undocumented forza horizon 4 car dash data
				case ForzaDataVersion.CarDashV2:
					break;

				default:
					throw new NotImplementedException(
						$"Unimplemented forza data version {output.Version}. " +
						$"This legacy data reader is not meant to be used after FH4."
					);
			}
		}

		return output;
	}

	private static ForzaDataVersion ReadVersion(byte[] input) => input.Length switch
	{
		SledPacketSize => ForzaDataVersion.Sled,
		CarDashPacketSize => ForzaDataVersion.CarDashV1,
		HorizonCarDashPacketSize => ForzaDataVersion.CarDashV2,
		_ => ForzaDataVersion.Unknown,
	};

	private static ForzaSledDataStruct ReadSledData(BinaryReader reader)
	{
		return new ForzaSledDataStruct
		{
			IsRaceOn = reader.ReadInt32(),

			TimestampMS = reader.ReadUInt32(),

			EngineMaxRpm = reader.ReadSingle(),
			EngineIdleRpm = reader.ReadSingle(),
			CurrentEngineRpm = reader.ReadSingle(),

			AccelerationX = reader.ReadSingle(),
			AccelerationY = reader.ReadSingle(),
			AccelerationZ = reader.ReadSingle(),

			VelocityX = reader.ReadSingle(),
			VelocityY = reader.ReadSingle(),
			VelocityZ = reader.ReadSingle(),

			AngularVelocityX = reader.ReadSingle(),
			AngularVelocityY = reader.ReadSingle(),
			AngularVelocityZ = reader.ReadSingle(),

			Yaw = reader.ReadSingle(),
			Pitch = reader.ReadSingle(),
			Roll = reader.ReadSingle(),

			NormalizedSuspensionTravelFrontLeft = reader.ReadSingle(),
			NormalizedSuspensionTravelFrontRight = reader.ReadSingle(),
			NormalizedSuspensionTravelRearLeft = reader.ReadSingle(),
			NormalizedSuspensionTravelRearRight = reader.ReadSingle(),

			TireSlipRatioFrontLeft = reader.ReadSingle(),
			TireSlipRatioFrontRight = reader.ReadSingle(),
			TireSlipRatioRearLeft = reader.ReadSingle(),
			TireSlipRatioRearRight = reader.ReadSingle(),

			WheelRotationSpeedFrontLeft = reader.ReadSingle(),
			WheelRotationSpeedFrontRight = reader.ReadSingle(),
			WheelRotationSpeedRearLeft = reader.ReadSingle(),
			WheelRotationSpeedRearRight = reader.ReadSingle(),

			WheelOnRumbleStripFrontLeft = reader.ReadInt32(),
			WheelOnRumbleStripFrontRight = reader.ReadInt32(),
			WheelOnRumbleStripRearLeft = reader.ReadInt32(),
			WheelOnRumbleStripRearRight = reader.ReadInt32(),

			WheelInPuddleDepthFrontLeft = reader.ReadSingle(),
			WheelInPuddleDepthFrontRight = reader.ReadSingle(),
			WheelInPuddleDepthRearLeft = reader.ReadSingle(),
			WheelInPuddleDepthRearRight = reader.ReadSingle(),

			SurfaceRumbleFrontLeft = reader.ReadSingle(),
			SurfaceRumbleFrontRight = reader.ReadSingle(),
			SurfaceRumbleRearLeft = reader.ReadSingle(),
			SurfaceRumbleRearRight = reader.ReadSingle(),

			TireSlipAngleFrontLeft = reader.ReadSingle(),
			TireSlipAngleFrontRight = reader.ReadSingle(),
			TireSlipAngleRearLeft = reader.ReadSingle(),
			TireSlipAngleRearRight = reader.ReadSingle(),

			TireCombinedSlipFrontLeft = reader.ReadSingle(),
			TireCombinedSlipFrontRight = reader.ReadSingle(),
			TireCombinedSlipRearLeft = reader.ReadSingle(),
			TireCombinedSlipRearRight = reader.ReadSingle(),

			SuspensionTravelMetersFrontLeft = reader.ReadSingle(),
			SuspensionTravelMetersFrontRight = reader.ReadSingle(),
			SuspensionTravelMetersRearLeft = reader.ReadSingle(),
			SuspensionTravelMetersRearRight = reader.ReadSingle(),

			CarOrdinal = reader.ReadInt32(),
			CarClass = reader.ReadInt32(),
			CarPerformanceIndex = reader.ReadInt32(),
			DrivetrainType = reader.ReadInt32(),
			NumCylinders = reader.ReadInt32()
		};
	}

	private static ForzaCarDashDataStruct ReadCarDashData(BinaryReader reader)
	{
		return new ForzaCarDashDataStruct
		{
			PositionX = reader.ReadSingle(),
			PositionY = reader.ReadSingle(),
			PositionZ = reader.ReadSingle(),

			Speed = reader.ReadSingle(),
			Power = reader.ReadSingle(),
			Torque = reader.ReadSingle(),

			TireTempFrontLeft = reader.ReadSingle(),
			TireTempFrontRight = reader.ReadSingle(),
			TireTempRearLeft = reader.ReadSingle(),
			TireTempRearRight = reader.ReadSingle(),

			Boost = reader.ReadSingle(),
			Fuel = reader.ReadSingle(),
			DistanceTraveled = reader.ReadSingle(),
			BestLap = reader.ReadSingle(),
			LastLap = reader.ReadSingle(),
			CurrentLap = reader.ReadSingle(),
			CurrentRaceTime = reader.ReadSingle(),

			LapNumber = reader.ReadUInt16(),
			RacePosition = reader.ReadByte(),

			Accel = reader.ReadByte(),
			Brake = reader.ReadByte(),
			Clutch = reader.ReadByte(),
			HandBrake = reader.ReadByte(),
			Gear = reader.ReadByte(),
			Steer = reader.ReadSByte(),

			NormalizedDrivingLine = reader.ReadSByte(),
			NormalizedAIBrakeDifference = reader.ReadSByte()
		};
	}
}