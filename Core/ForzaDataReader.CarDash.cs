using System;

namespace ForzaData.Core
{
	public partial class ForzaDataReader
	{
		/// <summary>
		/// Map of all car dash values
		/// </summary>
		/// <remarks>
		/// Indexes are not absolute, they are relative to the data offset.
		/// See <see cref="CarDashRange"/> and <see cref="HorizonCarDashRange"/> for info.
		/// </remarks>
		internal static class CarDashMap
		{
			internal static readonly Range PositionX = 0..4;
			internal static readonly Range PositionY = 4..8;
			internal static readonly Range PositionZ = 8..12;

			internal static readonly Range Speed = 12..18;
			internal static readonly Range Power = 16..20;
			internal static readonly Range Torque = 20..24;

			internal static readonly Range TireTempFrontLeft = 24..28;
			internal static readonly Range TireTempFrontRight = 28..32;
			internal static readonly Range TireTempRearLeft = 32..36;
			internal static readonly Range TireTempRearRight = 36..40;

			internal static readonly Range Boost = 40..44;
			internal static readonly Range Fuel = 44..48;
			internal static readonly Range DistanceTraveled = 48..52;
			internal static readonly Range BestLap = 52..56;
			internal static readonly Range LastLap = 56..60;
			internal static readonly Range CurrentLap = 60..64;
			internal static readonly Range CurrentRaceTime = 64..68;

			internal static readonly Range LapNumber = 68..70;
			internal static readonly Index RacePosition = 70;

			internal static readonly Index Accel = 71;
			internal static readonly Index Brake = 72;
			internal static readonly Index Clutch = 73;
			internal static readonly Index HandBrake = 74;
			internal static readonly Index Gear = 75;
			internal static readonly Index Steer = 76;

			internal static readonly Index NormalizedDrivingLine = 77;
			internal static readonly Index NormalizedAIBrakeDifference = 78;
		}

		private ForzaCarDashDataStruct ReadCarDashData(in ReadOnlySpan<byte> data) => new ForzaCarDashDataStruct
		{
			PositionX = BitConverter.ToSingle(data[CarDashMap.PositionX]),
			PositionY = BitConverter.ToSingle(data[CarDashMap.PositionY]),
			PositionZ = BitConverter.ToSingle(data[CarDashMap.PositionZ]),

			Speed = BitConverter.ToSingle(data[CarDashMap.Speed]),
			Power = BitConverter.ToSingle(data[CarDashMap.Power]),
			Torque = BitConverter.ToSingle(data[CarDashMap.Torque]),

			TireTempFrontLeft = BitConverter.ToSingle(data[CarDashMap.TireTempFrontLeft]),
			TireTempFrontRight = BitConverter.ToSingle(data[CarDashMap.TireTempFrontRight]),
			TireTempRearLeft = BitConverter.ToSingle(data[CarDashMap.TireTempRearLeft]),
			TireTempRearRight = BitConverter.ToSingle(data[CarDashMap.TireTempRearRight]),

			Boost = BitConverter.ToSingle(data[CarDashMap.Boost]),
			Fuel = BitConverter.ToSingle(data[CarDashMap.Fuel]),
			DistanceTraveled = BitConverter.ToSingle(data[CarDashMap.DistanceTraveled]),
			BestLap = BitConverter.ToSingle(data[CarDashMap.BestLap]),
			LastLap = BitConverter.ToSingle(data[CarDashMap.LastLap]),
			CurrentLap = BitConverter.ToSingle(data[CarDashMap.CurrentLap]),
			CurrentRaceTime = BitConverter.ToSingle(data[CarDashMap.CurrentRaceTime]),

			LapNumber = BitConverter.ToUInt16(data[CarDashMap.LapNumber]),
			RacePosition = data[CarDashMap.RacePosition],

			Accel = data[CarDashMap.Accel],
			Brake = data[CarDashMap.Brake],
			Clutch = data[CarDashMap.Clutch],
			HandBrake = data[CarDashMap.HandBrake],
			Gear = data[CarDashMap.Gear],
			Steer = unchecked((sbyte)data[CarDashMap.Steer]),

			NormalizedDrivingLine = unchecked((sbyte)data[CarDashMap.NormalizedDrivingLine]),
			NormalizedAIBrakeDifference = unchecked((sbyte)data[CarDashMap.NormalizedAIBrakeDifference])
		};
	}
}
