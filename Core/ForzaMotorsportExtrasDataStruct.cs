using System.Runtime.InteropServices;

namespace ForzaData.Core;

/// <summary>
/// Motorsport (2023) only extras data structure
/// </summary>
/// <remarks>
/// https://support.forzamotorsport.net/hc/en-us/articles/21742934024211-Forza-Motorsport-Data-Out-Documentation
/// </remarks>
[StructLayout(LayoutKind.Auto)]
public struct ForzaMotorsportExtrasDataStruct
{
	public float TireWearFrontLeft;
	public float TireWearFrontRight;
	public float TireWearRearLeft;
	public float TireWearRearRight;

	// ID for track
	public int TrackOrdinal;
}
