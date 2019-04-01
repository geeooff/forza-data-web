using System;
using System.Collections.Generic;
using System.Text;

namespace ForzaData.Core
{
	[Serializable]
	public class ForzaDataException : Exception
	{
		public ForzaDataException() { }
		public ForzaDataException(string message) : base(message) { }
		public ForzaDataException(string message, Exception inner) : base(message, inner) { }
		protected ForzaDataException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
