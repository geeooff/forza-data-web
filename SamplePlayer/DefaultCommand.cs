using System.Diagnostics;
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

		// Setup display if requested
		ForzaDataConsole? consoleDisplay = null;
		
		if (settings.Display)
		{
			consoleDisplay = new ForzaDataConsole();
		}

		// Open input file and reader
		using var inputStream = new FileStream(inputFile.FullName, FileMode.Open, FileAccess.Read);
		using var reader = new SampleReader(inputStream);

		Console.Out.WriteLine($"Playing: {inputFile.Name}");
		Console.Out.WriteLine($"Output: {outputIp}:{outputPort}");
		Console.Out.WriteLine($"Speed: {settings.Speed}x");
		Console.Out.WriteLine($"Loop: {settings.Loop}");
		Console.Out.WriteLine($"Display: {settings.Display}");
		Console.Out.WriteLine("Press Ctrl+C to stop...");

		var stopwatch = Stopwatch.StartNew();
		long totalChunks = 0;
		long totalBytes = 0;

		do
		{
			// Reset for loop
			inputStream.Position = 0;
			reader.Reads = 0;
			stopwatch.Restart();

			while (reader.TryRead(out SampleStruct chunk))
			{
				totalChunks++;
				totalBytes += chunk.Length;

				// Calculate target elapsed time for this chunk
				long targetTicks = (long)(chunk.Elapsed / settings.Speed);
				long currentTicks = stopwatch.ElapsedTicks;
				long ticksToWait = targetTicks - currentTicks;

				// Wait if needed (with 1ms precision to avoid busy waiting)
				if (ticksToWait > TimeSpan.TicksPerMillisecond)
				{
					int msToWait = (int)(ticksToWait / TimeSpan.TicksPerMillisecond);
					await Task.Delay(msToWait);
				}

				// Send UDP packet
				udpClient.Send(chunk.Data, chunk.Data.Length);

				// Also display if enabled
				if (settings.Display && consoleDisplay != null)
				{
					try
					{
						var dataReader = new ForzaDataReader();
						var forzaData = dataReader.Read(chunk.Data);
						consoleDisplay.OnNext(forzaData);
					}
					catch (Exception ex)
					{
						Console.Error.WriteLine($"Error displaying data: {ex.Message}");
					}
				}
			}

			if (!reader.EndOfStream)
			{
				Console.Error.WriteLine("Warning: End of file reached unexpectedly");
			}

			Console.Out.WriteLine($"Playback completed: {totalChunks} chunks, {totalBytes} bytes");

			if (!settings.Loop)
				break;

			// Small delay between loops
			await Task.Delay(1000);
			Console.Out.WriteLine("Looping...");
			
		} while (settings.Loop);

		return 0;
	}
}
