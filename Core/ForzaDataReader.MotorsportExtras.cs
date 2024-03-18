namespace ForzaData.Core;

public partial class ForzaDataReader
{
	/// <summary>
	/// Map of all motorsport extras values
	/// </summary>
	/// <remarks>
	/// See <see cref="MotorsportExtrasRange"/> for info.
	/// </remarks>
	internal static class MotorsportExtrasMap
	{
		internal static readonly Range TireWearFrontLeft = 0..4;
		internal static readonly Range TireWearFrontRight = 4..8;
		internal static readonly Range TireWearRearLeft = 8..12;
		internal static readonly Range TireWearRearRight = 12..16;
		internal static readonly Range TrackOrdinal = 16..20;
	}

	private static ForzaMotorsportExtrasDataStruct ReadMotorsportExtrasData(in ReadOnlySpan<byte> data) => new()
	{
		TireWearFrontLeft = BitConverter.ToSingle(data[MotorsportExtrasMap.TireWearFrontLeft]),
		TireWearFrontRight = BitConverter.ToSingle(data[MotorsportExtrasMap.TireWearFrontRight]),
		TireWearRearLeft = BitConverter.ToSingle(data[MotorsportExtrasMap.TireWearRearLeft]),
		TireWearRearRight = BitConverter.ToSingle(data[MotorsportExtrasMap.TireWearRearRight]),
		TrackOrdinal = BitConverter.ToInt32(data[MotorsportExtrasMap.TrackOrdinal])
	};
}
