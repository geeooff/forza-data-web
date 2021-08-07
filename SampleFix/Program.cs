using ForzaData.Core;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace ForzaData.SampleFrequencyFix
{
	class Program
	{
		private const double ChunkTime = 1000d / 60d; // 60 Hz = 16.667 ms elapsed per chunk

		internal enum ExitCodes
		{
			ArgsError = -1,
			OK = 0,
			Help = 1,
			InputError = 2,
			NonIncreasingTimestamps = 3
		}

		private static Arguments _args;

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
			else if (_args.UseTimestamp == _args.UseStaticFrequency)
			{
				Console.Error.WriteLine("You must specify which fix to use: timestamp OR frequency");
				ShowHelp();
				Exit(ExitCodes.ArgsError);
				return;
			}

			var inputFile = new FileInfo(_args.Input);
			var outputFile = new FileInfo(_args.Output);
			string typeOfFix = _args.UseTimestamp
				? "timestamp"
				: _args.UseStaticFrequency
					? "fixed frequency"
					: "unknown";

			// input file existence
			if (!inputFile.Exists)
			{
				Console.Error.WriteLine($"Input file {inputFile.Name} is not found or its access is denied");
				Exit(ExitCodes.InputError);
				return;
			}

			// output directory creation
			if (!outputFile.Directory.Exists)
			{
				outputFile.Directory.Create();
			}

			Console.Out.WriteLine($"Input: {inputFile.Name}");
			Console.Out.WriteLine($"Output: {outputFile.Name}");
			Console.Out.WriteLine($"Fix: {typeOfFix}");

			// input read
			using (FileStream input = new FileStream(inputFile.FullName, FileMode.Open, FileAccess.Read))
			using (FileStream output = new FileStream(outputFile.FullName, FileMode.Create, FileAccess.Write))
			using (SampleReader reader = new SampleReader(input))
			using (SampleWriter writer = new SampleWriter(output))
			{
				ForzaDataReader dataReader = new ForzaDataReader();
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
					if (_args.UseTimestamp)
					{
						// non increasing timestamps
						if (prevForzaData?.Sled.TimestampMS > forzaData.Sled.TimestampMS)
						{
							if (_args.IgnoreInvalidChunks)
							{
								Console.Out.WriteLine($"Ignoring chunk #{chunkIndex}, {chunk.Length} bytes (non increasing timestamp)");
								continue;
							}
							else
							{
								Console.Error.WriteLine($"Non increasing timestamp at chunk #{chunkIndex}, {chunk.Length} bytes");
								Exit(ExitCodes.NonIncreasingTimestamps);
								break;
							}
						}

						// fixing elapsed ticks from previous chunk timestamp diff
						uint elapsedMilliseconds = forzaData.Sled.TimestampMS - firstForzaData.Value.Sled.TimestampMS;
						chunk.Elapsed = TimeSpan.FromMilliseconds(elapsedMilliseconds).Ticks;
					}
					else if (_args.UseStaticFrequency)
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
}
