using PowArgs.Attributes;

namespace ForzaData.Console;

public class Arguments
{
	[Argument("UDP local network port to bind")]
	public int Port { get; set; } = 7777;

	[Argument("IP of server (your PC or console running the game) to listen to", required: true)]
	public string? ServerIpAddress { get; set; }

	[Argument("Show this program arguments help")]
	public bool Help { get; set; } = false;
}
