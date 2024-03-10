using System.Runtime.InteropServices;

namespace ForzaData.Core
{
	/// <summary>
	/// Forza Data Out structure
	/// </summary>
	[StructLayout(LayoutKind.Auto)]
	public struct ForzaDataStruct
	{
		/// <summary>
		/// Protocol version
		/// </summary>
		public ForzaDataVersion Version;

		/// <summary>
		/// Sled data
		/// </summary>
		public ForzaSledDataStruct Sled;

		/// <summary>
		/// Car dash data
		/// </summary>
		public ForzaCarDashDataStruct? CarDash;

		/// <summary>
		/// Horizon extras data
		/// </summary>
		public ForzaHorizonExtrasDataStruct? HorizonExtras;

		/// <summary>
		/// Motorsport extras data
		/// </summary>
		public ForzaMotorsportExtrasDataStruct? MotorsportExtras;
	}
}
