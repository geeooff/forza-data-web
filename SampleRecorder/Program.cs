using ForzaData.Core;
using System;
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

			using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
			using (UdpStreamListener listener = new UdpStreamListener(_args.Port, serverIpAddress))
			{
				// cancellation provided by CTRL + C / CTRL + break
				System.Console.CancelKeyPress += (sender, e) =>
				{
					e.Cancel = true;
					cancellationTokenSource.Cancel();
				};

				// forza data observer
				UdpStreamSampleRecorder sampleRecorder = new UdpStreamSampleRecorder();
				sampleRecorder.Subscribe(listener);

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
			}
		}
	}
}
