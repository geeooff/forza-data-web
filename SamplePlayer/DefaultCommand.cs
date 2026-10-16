using System.Net;
using System.Net.Sockets;

namespace ForzaData.SamplePlayer;

public sealed class DefaultCommand : AsyncCommand<DefaultCommandSettings>
{
	private const int DefaultPort = 7777;
	private const string DefaultIp = "127.0.0.1";

	public override async Task<int> ExecuteAsync(CommandContext context, DefaultCommandSettings settings)
	{
		var inputFile = new FileInfo(settings.Input!);
		
		if (!inputFile.Exists)
		{
			Console.Error.WriteLine($"Input file '{inputFile.FullName}' does not exist");
			return -1;
		}

		// Resolve output IP and port
		IPAddress outputIp;
		int outputPort = settings.OutputPort ?? DefaultPort;
		
		if (string.IsNullOrWhiteSpace(settings.OutputIp))
		{
			if (!IPAddress.TryParse(DefaultIp, out outputIp!))
			{
				Console.Error.WriteLine("Invalid default IP address");
				return -1;
			}
		}
		else
		{
			if (!IPAddress.TryParse(settings.OutputIp, out outputIp!))
			{
				Console.Error.WriteLine($"Invalid output IP address: {settings.OutputIp}");
				return -1;
			}
		}

		// Setup UDP client for playback
		using var udpClient = new UdpClient();
		udpClient.Connect(outputIp, outputPort);

		// Setup cancellation token for Ctrl+C
		using var cts = new CancellationTokenSource();
		Console.CancelKeyPress += (sender, e) =>
		{
			e.Cancel = true;
			cts.Cancel();
		};

		// Create player service
		var playerService = new SamplePlayerService();

		Console.Out.WriteLine($"Playing: {inputFile.Name}");
		Console.Out.WriteLine($"Output: {outputIp}:{outputPort}");
		Console.Out.WriteLine($"Speed: {settings.Speed}x");
		Console.Out.WriteLine($"Loop: {settings.Loop}");
		Console.Out.WriteLine("Press Ctrl+C to stop...");

		do
		{
			// Reset service for loop
			playerService.Reset();

			// Open input file
			using var inputStream = new FileStream(inputFile.FullName, FileMode.Open, FileAccess.Read);

			try
			{
				await playerService.PlayAsync(inputStream, udpClient, settings.Speed, cts.Token);
			}
			catch (OperationCanceledException)
			{
				// User cancelled
				Console.Out.WriteLine("Playback cancelled");
				return 0;
			}

			// Display stats for this iteration
			Console.Out.WriteLine($"Playback iteration completed: {playerService.TotalChunks} chunks, {playerService.TotalBytes} bytes");

			if (!settings.Loop)
				break;

			// Small delay between loops
			await Task.Delay(1000, cts.Token);
			Console.Out.WriteLine("Looping...");
			
		} while (settings.Loop && !cts.IsCancellationRequested);

		return 0;
	}
}
