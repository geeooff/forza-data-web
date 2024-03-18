namespace ForzaData.SampleFix;

public sealed class DefaultCommand : Command<DefaultCommandSettings>
{
	private const double ChunkTime = 1000d / 60d; // 60 Hz = 16.667 ms elapsed per chunk

	internal enum ExitCodes
	{
		OK = 0,
		Help = 1,
		InputError = 2,
		NonIncreasingTimestamps = 3
	}

	public override int Execute([NotNull] CommandContext context, [NotNull] DefaultCommandSettings settings)
	{
		var inputFile = new FileInfo(settings.Input!);
		var outputFile = new FileInfo(settings.Output!);

		string typeOfFix = settings.UseTimestamp
			? "timestamp"
			: settings.UseStaticFrequency
				? "fixed frequency"
				: "unknown";

		Console.Out.WriteLine($"Input: {inputFile.Name}");
		Console.Out.WriteLine($"Output: {outputFile.Name}");
		Console.Out.WriteLine($"Fix: {typeOfFix}");

		// input read
		using var input = new FileStream(inputFile.FullName, FileMode.Open, FileAccess.Read);
		using var output = new FileStream(outputFile.FullName, FileMode.Create, FileAccess.Write);
		using var reader = new SampleReader(input);
		using var writer = new SampleWriter(output);

		ForzaDataReader dataReader = new();
		ForzaDataStruct? firstForzaData = null;
		ForzaDataStruct? prevForzaData = null;
		int chunkIndex;

		// iterate every chunk
		for (chunkIndex = 0; reader.TryRead(out SampleStruct chunk); chunkIndex++)
		{
			// decode forza data structure
			ForzaDataStruct forzaData = dataReader.Read(chunk.Data);
			if (!firstForzaData.HasValue)
				firstForzaData = forzaData;

			// if timestamps are used, validate them
			if (settings.UseTimestamp)
			{
				// non increasing timestamps
				if (prevForzaData?.Sled.TimestampMS > forzaData.Sled.TimestampMS)
				{
					if (settings.IgnoreInvalidChunks)
					{
						Console.Out.WriteLine($"Ignoring chunk #{chunkIndex}, {chunk.Length} bytes (non increasing timestamp)");
						continue;
					}
					else
					{
						Console.Error.WriteLine($"Non increasing timestamp at chunk #{chunkIndex}, {chunk.Length} bytes");
						return (int)ExitCodes.NonIncreasingTimestamps;
					}
				}

				// fixing elapsed ticks from previous chunk timestamp diff
				uint elapsedMilliseconds = forzaData.Sled.TimestampMS - firstForzaData.Value.Sled.TimestampMS;
				chunk.Elapsed = TimeSpan.FromMilliseconds(elapsedMilliseconds).Ticks;
			}
			else if (settings.UseStaticFrequency)
			{
				// fixing elapsed ticks with static chunk time
				uint elapsedMilliseconds = (uint)Math.Round(
					chunkIndex * ChunkTime,
					MidpointRounding.AwayFromZero
				);
				chunk.Elapsed = TimeSpan.FromMilliseconds(elapsedMilliseconds).Ticks;
			}

			// fixed chunk written to output
			writer.Write(chunk);

			prevForzaData = forzaData;
		}

		// warning if not on end of stream
		if (!reader.EndOfStream)
		{
			Console.Error.WriteLine($"Can't decode more chunks, the rest of the source is ignored");
		}

		writer.Flush();
		outputFile.Refresh();

		// final result
		Console.Out.WriteLine($"Chunks: {reader.Reads} read, {writer.Writes} written (diff: {writer.Writes - reader.Reads})");
		Console.Out.WriteLine($"Size: {inputFile.Length} bytes on input, {outputFile.Length} bytes on output, (diff: {outputFile.Length - inputFile.Length})");

		return (int)ExitCodes.OK;
	}
}