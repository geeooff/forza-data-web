using Spectre.Console.Cli;
using System.ComponentModel;
using System.Net;

namespace ForzaData.SamplePlayer;

public sealed class DefaultCommandSettings : CommandSettings
{
	[Description("Input file path to play")]
	[CommandArgument(0, "<INPUT>")]
	public string? Input { get; init; }

	[Description("Output IP address to send UDP packets to")]
	[CommandOption("-o|--output-ip <IP>")]
	public string? OutputIp { get; init; }

	[Description("Output port to send UDP packets to")]
	[CommandOption("-p|--output-port <PORT>")]
	public int? OutputPort { get; init; }

	[Description("Playback speed (1.0 = normal, 0.5 = half speed, 2.0 = double speed)")]
	[CommandOption("-s|--speed <SPEED>")]
	[DefaultValue(1.0)]
	public double Speed { get; init; } = 1.0;

	[Description("Loop playback indefinitely")]
	[CommandOption("-l|--loop")]
	public bool Loop { get; init; } = false;

	[Description("Display data in console (like the Console project)")]
	[CommandOption("-d|--display")]
	public bool Display { get; init; } = false;

	[Description("Server IP address for display (if different from output)")]
	[CommandOption("--server-ip <IP>")]
	public string? ServerIp { get; init; }

	[Description("Server port for display (if different from output)")]
	[CommandOption("--server-port <PORT>")]
	public int? ServerPort { get; init; }

	public override ValidationResult Validate()
	{
		if (string.IsNullOrWhiteSpace(Input))
			return ValidationResult.Error("Input file path is required");

		if (OutputPort is null || OutputPort <= 0 || OutputPort > 65535)
			return ValidationResult.Error("Output port must be between 1 and 65535");

		if (Speed <= 0)
			return ValidationResult.Error("Speed must be greater than 0");

		if (!string.IsNullOrWhiteSpace(OutputIp) && !IPAddress.TryParse(OutputIp, out _))
			return ValidationResult.Error("Invalid output IP address");

		if (Display && ServerPort is not null && (ServerPort <= 0 || ServerPort > 65535))
			return ValidationResult.Error("Server port must be between 1 and 65535");

		if (Display && !string.IsNullOrWhiteSpace(ServerIp) && !IPAddress.TryParse(ServerIp, out _))
			return ValidationResult.Error("Invalid server IP address");

		return ValidationResult.Success();
	}
}
