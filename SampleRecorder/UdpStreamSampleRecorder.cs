using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using ForzaData.Core;

namespace ForzaData.SampleRecorder
{
	public class UdpStreamSampleRecorder : UdpStreamObserver
	{
		private readonly Stopwatch _stopwatch;
		private readonly Stream _output;
		private readonly PipeWriter _writer;

		public UdpStreamSampleRecorder(Stream output)
		{
			_output = output ?? throw new ArgumentNullException(nameof(output));
			_writer = PipeWriter.Create(output);
			_stopwatch = new Stopwatch();
		}

		public long BytesRead { get; private set; }
		public long BytesWritten { get; private set; }

		public override void OnCompleted()
		{
			_stopwatch.Stop();
			_writer.Complete();
			_output.Flush();
		}

		public override void OnError(Exception error)
		{
			Debug.WriteLine(error);
		}

		public override void OnNext(byte[] value)
		{
			if (!_stopwatch.IsRunning)
			{
				_stopwatch.Start();
			}

			long elapsed = _stopwatch.Elapsed.Ticks;

			BytesRead += value.Length;

			)

			_output.Write(BitConverter.GetBytes(elapsed));		BytesWritten += 8;				// 8 bytes
			_output.Write(BitConverter.GetBytes(value.Length));	BytesWritten += 4;				// 4 bytes
			_output.Write(value);								BytesWritten += value.Length;	// n bytes
		}
	}
}
