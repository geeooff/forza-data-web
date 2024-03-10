using System.Runtime.InteropServices;

namespace ForzaData.Core
{
	/// <summary>
	/// Horizon-only extras data structure
	/// </summary>
	[StructLayout(LayoutKind.Auto)]
	public struct ForzaHorizonExtrasDataStruct
	{
		public int CarCategory;
		public int UnknownField1;
		public int UnknownField2;
	}
}
