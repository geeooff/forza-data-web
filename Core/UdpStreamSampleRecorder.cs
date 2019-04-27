using ForzaData.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ForzaData.Core
{
	public class UdpStreamSampleRecorder : UdpStreamObserver, IDisposable
	{
		private readonly BinaryWriter _writer;
		private readonly Stopwatch _stopwatch;

		private bool _isDisposed = false;

		public UdpStreamSampleRecorder(Stream output)
		{
			_writer = new BinaryWriter(output, Encoding.UTF8, true);
			_stopwatch = new Stopwatch();
		}

		public long BytesRead { get; private set; }
		public long BytesWritten { get; private set; }

		public override void OnCompleted()
		{
			_writer.Flush();
		}

		public override void OnError(Exception error)
		{
			Debug.WriteLine(error);
		}

		public override void OnNext(byte[] value)
		{
			long elapsed = _stopwatch.ElapsedTicks;
			int length = value.Length;

			if (!_stopwatch.IsRunning)
			{
				_stopwatch.Start();
			}

			_writer.Write(elapsed);	// 8 bytes
			_writer.Write(length);	// 4 bytes
			_writer.Write(value);

			BytesRead += length;
			BytesWritten += (length + 12);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (!_isDisposed)
			{
				if (isDisposing)
				{
					_writer.Dispose();
				}

				_isDisposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}
	}
}
