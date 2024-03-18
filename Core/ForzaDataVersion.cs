namespace ForzaData.Core;

/// <summary>
/// Internal forza data versions
/// </summary>
public enum ForzaDataVersion : byte
{
	/// <summary>
	/// Unknown version
	/// </summary>
	Unknown = 0,

	/// <summary>
	/// Sled-only data (Forza Motorsport 7 and Forza Motorsport (2023))
	/// </summary>
	Sled = 1,

	/// <summary>
	/// Car dash V1 (Forza Motorsport 7)
	/// </summary>
	CarDashV1 = 2,

	/// <summary>
	/// Car dash V2 (Forza Horizon 4 and 5)
	/// </summary>
	CarDashV2 = 3,

	/// <summary>
	/// Car dash V3 (Forza Motorsport (2023))
	/// </summary>
	CarDashV3 = 4
}
