using ForzaData.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ForzaData.SampleFrequencyFix
{
	public class Arguments
	{
		[PowArgs.Attributes.Argument("Use constant 60 Hz to resynchronize sample")]
		public bool UseStaticFrequency { get; set; } = false;

		[PowArgs.Attributes.Argument("Use data's decoded timestamps and resynchronize sample to them")]
		public bool UseTimestamp { get; set; } = false;

		[PowArgs.Attributes.Argument("Ignores invalid chunks (not increasing / not parsable)")]
		public bool IgnoreInvalidChunks { get; set; } = false;

		[PowArgs.Attributes.Argument("Input file", required: true)]
		public string Input { get; set; }

		[PowArgs.Attributes.Argument("Output file", required: true)]
		public string Output { get; set; }

		[PowArgs.Attributes.Argument("Show this program arguments help")]
		public bool Help { get; set; } = false;
	}
}
