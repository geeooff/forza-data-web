using System.Runtime.InteropServices;

namespace ForzaData.Core;

/// <summary>
/// Samples structure
/// </summary>
[StructLayout(LayoutKind.Auto)]
public struct SampleStruct
{
	/// <summary>
	/// Ticks elapsed since the record's beginning
	/// </summary>
	public long Elapsed;

	/// <summary>
	/// Length of <see cref="Data"/>
	/// </summary>
	public int Length;

	/// <summary>
	/// Data chunk
	/// </summary>
	public byte[] Data;
}
