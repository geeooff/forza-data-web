using System.Net;

namespace ForzaData.SampleRecorder;

public sealed class DefaultCommand : Command<DefaultCommandSettings>
{
	public override int Execute([NotNull] CommandContext context, [NotNull] DefaultCommandSettings settings)
	{
		var serverIpAddress = settings.Server!;
		var outputFile = new FileInfo(settings.Output!);

		using (var cancellationTokenSource = new CancellationTokenSource())
		using (var listener = new UdpStreamListener((int)settings.Port!, serverIpAddress))
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

		return 0;
	}
}