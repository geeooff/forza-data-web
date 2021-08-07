using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ForzaData.Core
{
	public class SampleReader : IDisposable
	{
		private readonly BinaryReader _reader;

		private bool _isDisposed = false;

		public SampleReader(Stream input)
		{
			_reader = new BinaryReader(input, Encoding.UTF8, true);
		}

		public int Reads { get; private set; } = 0;
		public bool EndOfStream { get; private set; } = false;

		public void Dispose() => Dispose(true);

		protected virtual void Dispose(bool isDisposing)
		{
			if (!_isDisposed)
			{
				if (isDisposing)
				{
					_reader.Dispose();
				}

				_isDisposed = true;
			}
		}

		public bool TryRead(out SampleStruct chunk)
		{
			EndOfStream = false;

			try
			{
				long elapsed = _reader.ReadInt64();
				int length = _reader.ReadInt32();

				if (elapsed >= 0 && length > 0)
				{
					byte[] data = _reader.ReadBytes(length);

					if (length == data.Length)
					{
						chunk = new SampleStruct
						{
							Elapsed = elapsed,
							Length = length,
							Data = data
						};
						Reads++;
						return true;
					}
				}

				chunk = default;
				return false;
			}
			catch (EndOfStreamException)
			{
				EndOfStream = true;
				chunk = default;
				return false;
			}
			catch (Exception ex)
			{
				throw new SampleException("Unexpected sample read error", ex);
			}
		}
	}
}
