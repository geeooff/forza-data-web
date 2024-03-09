using System;

namespace ForzaData.Core
{
	public class ForzaDataException : Exception
	{
		public ForzaDataException() { }
		public ForzaDataException(string message) : base(message) { }
		public ForzaDataException(string message, Exception inner) : base(message, inner) { }
	}
}
