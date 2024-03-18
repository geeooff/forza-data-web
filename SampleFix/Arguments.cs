using PowArgs.Attributes;

namespace ForzaData.SampleFix;

public class Arguments
{
	[Argument("Use constant 60 Hz to resynchronize sample")]
	public bool UseStaticFrequency { get; set; } = false;

	[Argument("Use forza data's decoded timestamps to resynchronize (recommended)")]
	public bool UseTimestamp { get; set; } = false;

	[Argument("Ignores invalid chunks (not increasing / not parsable)")]
	public bool IgnoreInvalidChunks { get; set; } = false;

	[Argument("Input file", required: true)]
	public string? Input { get; set; }

	[Argument("Output file", required: true)]
	public string? Output { get; set; }

	[Argument("Show this program arguments help")]
	public bool Help { get; set; } = false;
}
