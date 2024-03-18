using System.Text;

namespace ForzaData.Core;

public class SampleWriter(Stream output) : IDisposable
{
	private readonly BinaryWriter _writer = new(output, Encoding.UTF8, true);

	private bool _isDisposed = false;

	public int Writes { get; private set; } = 0;

	public void Flush() => _writer.Flush();

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
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

	public void Write(SampleStruct chunk)
	{
		if (chunk.Elapsed < 0)
			throw new SampleException("Chunk has an invalid elapsed value");
		else if (chunk.Length <= 0)
			throw new SampleException("Chunk has an invalid length");
		else if (chunk.Data == null || chunk.Data.Length == 0)
			throw new SampleException("Chunk has no data");
		else if (chunk.Length != chunk.Data.Length)
			throw new SampleException("Chunk has an invalid data length");

		try
		{
			_writer.Write(chunk.Elapsed);
			_writer.Write(chunk.Length);
			_writer.Write(chunk.Data);
			Writes++;
		}
		catch (Exception ex)
		{
			throw new SampleException("Unexpected sample write error", ex);
		}
	}
}
