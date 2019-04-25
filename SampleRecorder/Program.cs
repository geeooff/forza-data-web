using ForzaData.Core;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace ForzaData.SampleRecorder
{
	class Program
	{
		private static Arguments _args;

		static void Main(string[] args)
		{
			try
			{
				_args = PowArgs.Parser<Arguments>.Parse(args);
			}
			catch (Exception ex)
			{
				System.Console.Error.WriteLine(ex.Message);
				return;
			}

			if (_args.Help)
			{
				foreach (string helpTextLine in PowArgs.Helper<Arguments>.GetHelpText())
				{
					System.Console.WriteLine(helpTextLine);
				}
				return;
			}

			var serverIpAddress = IPAddress.Parse(_args.ServerIpAddress);
			var outputFile = new FileInfo(_args.Output);
			
			// output directory creation
			if (!outputFile.Directory.Exists)
			{
				outputFile.Directory.Create();
			}

			using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
			using (UdpStreamListener listener = new UdpStreamListener(_args.Port, serverIpAddress))
			{
				// cancellation provided by CTRL + C / CTRL + break
				System.Console.CancelKeyPress += (sender, e) =>
				{
					e.Cancel = true;
					cancellationTokenSource.Cancel();
				};

				// udp packet observer to output file
				using (FileStream output = new FileStream(outputFile.FullName, FileMode.Create, FileAccess.Write))
				using (UdpStreamSampleRecorder sampleRecorder = new UdpStreamSampleRecorder(output))
				{
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
			}

			outputFile.Refresh();

			// empty file deletion
			if (outputFile.Length == 0L)
			{
				outputFile.Delete();
			}
		}
	}
}
