using System;

namespace ForzaData.Core
{
	public partial class ForzaDataReader
	{
		/// <summary>
		/// Map of all horizon extras values
		/// </summary>
		/// <remarks>
		/// See <see cref="HorizonExtrasRange"/> for info.
		/// </remarks>
		internal static class HorizonExtrasMap
		{
			// People assumptions, since there's no documentation:
			// https://forums.forza.net/t/data-out-telemetry-variables-and-structure/535984/5
			internal static readonly Range CarCategory = 0..4;
			internal static readonly Range UnknownField1 = 4..8;
			internal static readonly Range UnknownField2 = 8..12;
		}

		private static ForzaHorizonExtrasDataStruct ReadHorizonExtrasData(in ReadOnlySpan<byte> data) => new()
		{
			CarCategory = BitConverter.ToInt32(data[HorizonExtrasMap.CarCategory]),
			UnknownField1 = BitConverter.ToInt32(data[HorizonExtrasMap.UnknownField1]),
			UnknownField2 = BitConverter.ToInt32(data[HorizonExtrasMap.UnknownField2])
		};
	}
}
