using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ForzaData.Core;

namespace ForzaData.Console
{
	class Program
	{
		internal enum ExitCodes
		{
			ArgsError = -1,
			OK = 0,
			Help = 1
		}

		private static Arguments _args;

		static void Main(string[] args)
		{
			// forcing UTF-8 encoding
			System.Console.OutputEncoding = Encoding.UTF8;

			try
			{
				_args = PowArgs.Parser<Arguments>.Parse(args);
			}
			catch (Exception ex)
			{
				System.Console.Error.WriteLine(ex.Message);
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

			var serverIpAddress = IPAddress.Parse(_args.ServerIpAddress);

			using (CancellationTokenSource cancellationTokenSource = new CancellationTokenSource())
			using (ForzaDataListener listener = new ForzaDataListener(_args.Port, serverIpAddress))
			{
				// cancellation provided by CTRL + C / CTRL + break
				System.Console.CancelKeyPress += (sender, e) =>
				{
					e.Cancel = true;
					cancellationTokenSource.Cancel();
				};

				// forza data observer
				ForzaDataConsole console = new ForzaDataConsole();
				console.Subscribe(listener);

				try
				{
					// forza data observable
					listener.Listen(cancellationTokenSource.Token);
				}
				catch (OperationCanceledException)
				{
					// user cancellation requested
				}

				console.Unsubscribe();
			}

			Exit(ExitCodes.OK);
		}

		private static void ShowHelp()
		{
			foreach (string helpTextLine in PowArgs.Helper<Arguments>.GetHelpText())
			{
				System.Console.Out.WriteLine(helpTextLine);
			}
		}

		private static void Exit(ExitCodes exitCode)
		{
			Environment.Exit((int)exitCode);
		}
	}
}
