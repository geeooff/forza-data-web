using System;
using System.Collections.Generic;
using System.Text;

namespace ForzaData.Core
{
	/// <summary>
	/// Samples structure
	/// </summary>
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
}
