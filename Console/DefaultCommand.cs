namespace ForzaData.Console;

public sealed class DefaultCommand : Command<DefaultCommandSettings>
{
	public override int Execute([NotNull] CommandContext context, [NotNull] DefaultCommandSettings settings)
	{
		using var cancellationTokenSource = new CancellationTokenSource();
		using var listener = new ForzaDataListener((int)settings.Port!, settings.Server!);

		// cancellation provided by CTRL + C / CTRL + break
		System.Console.CancelKeyPress += (sender, e) =>
		{
			e.Cancel = true;
			cancellationTokenSource.Cancel();
		};

		// forza data observer
		var console = new ForzaDataConsole();
		console.Subscribe(listener);

		try
		{
			System.Console.WriteLine($"Listening for data from {settings.Server} to local port {settings.Port}...");
			System.Console.WriteLine($"Please press CTRL+C to stop listening");

			// forza data observable
			listener.Listen(cancellationTokenSource.Token);
		}
		catch (OperationCanceledException)
		{
			// user cancellation requested
		}

		console.Unsubscribe();

		return 0;
	}
}