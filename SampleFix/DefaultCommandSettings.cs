using System.ComponentModel;

namespace ForzaData.SampleFix;

public sealed class DefaultCommandSettings : CommandSettings
{
	[Description("Use constant 60 Hz to resynchronize sample")]
	[CommandOption("--use-static-frequency")]
	[DefaultValue(false)]
	public bool UseStaticFrequency { get; set; } = false;

	[Description("Use forza data's decoded timestamps to resynchronize (recommended)")]
	[CommandOption("--use-timestamp")]
	[DefaultValue(false)]
	public bool UseTimestamp { get; set; } = false;

	[Description("Ignores invalid chunks (not increasing / not parsable)")]
	[CommandOption("--ignore-invalid-chunks")]
	[DefaultValue(false)]
	public bool IgnoreInvalidChunks { get; set; } = false;

	[Description("Input file")]
	[CommandOption("-i|--input")]
	public string? Input { get; set; }

	[Description("Output file")]
	[CommandOption("-o|--output")]
	public string? Output { get; set; }

	public override ValidationResult Validate()
	{
		if (UseStaticFrequency == UseTimestamp)
			return ValidationResult.Error("You must specify which fix to use: timestamp OR frequency");

		if (string.IsNullOrEmpty(Input))
			return ValidationResult.Error("An input file must be specified");

		if (string.IsNullOrEmpty(Output))
			return ValidationResult.Error("An output file must be specified");

		var inputFile = new FileInfo(Input);

		if (!inputFile.Exists)
			return ValidationResult.Error($"Input file {inputFile.Name} is not found or its access is denied");

		var outputFile = new FileInfo(Output);

		if (outputFile.Directory?.Exists ?? false)
		{
			try
			{
				outputFile.Directory.Create();
			}
			catch (Exception ex)
			{
				return ValidationResult.Error($"An error occured while creating output file directory: {ex.Message}");
			}
		}

		return ValidationResult.Success();
	}
}