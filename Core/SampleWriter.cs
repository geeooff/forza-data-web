using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ForzaData.Core
{
	public class SampleWriter : IDisposable
	{
		private readonly BinaryWriter _writer;

		private bool _isDisposed = false;

		public SampleWriter(Stream output)
		{
			_writer = new BinaryWriter(output, Encoding.UTF8, true);
		}

		public int Writes { get; private set; } = 0;

		public void Flush() => _writer.Flush();

		public void Dispose() => Dispose(true);

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
			else if (chunk.Data?.Length == 0)
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
}
