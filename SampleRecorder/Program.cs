using ForzaData.Core;
using System.Net;

namespace ForzaData.SampleRecorder;

class Program
{
	internal enum ExitCodes
	{
		ArgsError = -1,
		OK = 0,
		Help = 1
	}

	private static Arguments? _args;

	static void Main(string[] args)
	{
		try
		{
			_args = PowArgs.Parser<Arguments>.Parse(args);
		}
		catch (Exception ex)
		{
			Console.Error.WriteLine(ex.Message);
			ShowHelp();
			Exit(ExitCodes.ArgsError);
			return;
		}

		if (_args.Help)
		{
			ShowHelp();
			Exit(ExitCodes.Help);
			return;
		}

		var serverIpAddress = IPAddress.Parse(_args.ServerIpAddress!);
		var outputFile = new FileInfo(_args.Output!);
		
		// output directory creation
		if (outputFile.Directory?.Exists ?? false)
		{
			outputFile.Directory.Create();
		}

		using (var cancellationTokenSource = new CancellationTokenSource())
		using (var listener = new UdpStreamListener(_args.Port, serverIpAddress))
		{
			// cancellation provided by CTRL + C / CTRL + break
			System.Console.CancelKeyPress += (sender, e) =>
			{
				e.Cancel = true;
				cancellationTokenSource.Cancel();
			};

			// udp packet observer to output file
			using var output = new FileStream(outputFile.FullName, FileMode.Create, FileAccess.Write);
			using var sampleRecorder = new UdpStreamSampleRecorder(output);

			sampleRecorder.Subscribe(listener);

			Console.Out.WriteLine($"Listening to {serverIpAddress}...");

			try
			{
				// forza data observable
				listener.Listen(cancellationTokenSource.Token);
			}
			catch (OperationCanceledException)
			{
				// user cancellation requested
			}

			sampleRecorder.Unsubscribe();

			Console.Out.Write("Listening stopped. ");

			if (output.Position > 0L)
				Console.Out.WriteLine($"{sampleRecorder.BytesRead:N0} bytes read, {sampleRecorder.BytesWritten:N0} bytes written");
			else
				Console.Out.WriteLine("Nothing was read.");
		}

		outputFile.Refresh();

		// empty file deletion
		if (outputFile.Length == 0L)
		{
			outputFile.Delete();
		}

		Exit(ExitCodes.OK);
	}

	private static void ShowHelp()
	{
		foreach (string helpTextLine in PowArgs.Helper<Arguments>.GetHelpText())
		{
			Console.Out.WriteLine(helpTextLine);
		}
	}

	private static void Exit(ExitCodes exitCode)
	{
		Environment.Exit((int)exitCode);
	}
}
