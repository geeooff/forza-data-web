using System.ComponentModel;
using System.Net;

namespace ForzaData.SampleRecorder;

public sealed class DefaultCommandSettings : CommandSettings
{
	[Description("IP of server (your PC or console running the game) to listen to")]
	[CommandOption("-s|--server <server>")]
	[TypeConverter(typeof(IPAddressTypeConverter))]
	public IPAddress? Server { get; set; }

	[Description("Local network port to listen on")]
	[CommandOption("-p|--port <port>")]
	public ushort? Port { get; set; }

	[Description("Output file")]
	[CommandOption("-o|--output")]
	public string? Output { get; set; }

	public override ValidationResult Validate()
	{
		if (Server is null)
			return ValidationResult.Error("The server IP address is required");
		if (Port is null)
			return ValidationResult.Error("The local network port number is required");
		if (Port is < 1024 or > 65535)
			return ValidationResult.Error("The local network port number must be in 1024-65535 range");
		if (string.IsNullOrEmpty(Output))
			return ValidationResult.Error("An output file must be specified");

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