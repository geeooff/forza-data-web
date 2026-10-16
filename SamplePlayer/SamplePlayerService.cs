using System.Diagnostics;
using System.Threading;

namespace ForzaData.SamplePlayer;

public sealed class SamplePlayerService
{
	public long TotalChunks { get; set; }
	public long TotalBytes { get; set; }

	public async Task PlayAsync(
		Stream inputStream,
		UdpClient udpClient,
		double speed,
		CancellationToken cancellationToken)
	{
		using var reader = new SampleReader(inputStream);
		var stopwatch = Stopwatch.StartNew();

		TotalChunks = 0;
		TotalBytes = 0;

		while (!cancellationToken.IsCancellationRequested && reader.TryRead(out SampleStruct chunk))
		{
			TotalChunks++;
			TotalBytes += chunk.Length;

			// Calculate absolute target time for this chunk
			var targetElapsed = TimeSpan.FromTicks((long)(chunk.Elapsed / speed));
			var currentElapsed = stopwatch.Elapsed;
			
			if (targetElapsed > currentElapsed)
			{
				var remaining = targetElapsed - currentElapsed;
				
				// Wait for most of the time with Task.Delay
				if (remaining.TotalMilliseconds > 1)
				{
					await Task.Delay((int)(remaining.TotalMilliseconds - 1), cancellationToken);
				}
				
				// Fine wait for the last millisecond using SpinWait
				var spinWait = new SpinWait();
				while (stopwatch.Elapsed < targetElapsed && !cancellationToken.IsCancellationRequested)
				{
					spinWait.SpinOnce();
					cancellationToken.ThrowIfCancellationRequested();
				}
			}

			// Send UDP packet
			udpClient.Send(chunk.Data, chunk.Data.Length);

			cancellationToken.ThrowIfCancellationRequested();
		}
	}

	public void Reset()
	{
		TotalChunks = 0;
		TotalBytes = 0;
	}
}
