using ForzaData.Core;
using System.Diagnostics;
using System.Text;

namespace ForzaData.SampleRecorder;

public class UdpStreamSampleRecorder(Stream output) : UdpStreamObserver, IDisposable
{
	private readonly BinaryWriter _writer = new(output, Encoding.UTF8, true);
	private readonly Stopwatch _stopwatch = new();

	private bool _isDisposed = false;

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

		if (!_stopwatch.IsRunning)
		{
			_stopwatch.Start();
		}

		_writer.Write(elapsed);			BytesWritten += 8;				// 8 bytes
		_writer.Write(value.Length);	BytesWritten += 4;				// 4 bytes
		_writer.Write(value);			BytesWritten += value.Length;	// n bytes
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
		GC.SuppressFinalize(this);
	}
}
