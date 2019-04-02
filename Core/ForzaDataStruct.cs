using System;
using System.Collections.Generic;
using System.Text;

namespace ForzaData.Core
{
	/// <summary>
	/// Forza Horizon 4 / Forza Motorsport 6 Data Out structure
	/// </summary>
	public struct ForzaDataStruct
	{
		/// <summary>
		/// Version number: 1 for V1, Sled data, 2 for V2, Sled + Car Dash, data
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
	}
}
