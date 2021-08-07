using System;
using System.Collections.Generic;
using System.Text;

namespace ForzaData.Core
{
	[Serializable]
	public class SampleException : Exception
	{
		public SampleException() { }
		public SampleException(string message) : base(message) { }
		public SampleException(string message, Exception inner) : base(message, inner) { }
		protected SampleException(
			System.Runtime.Serialization.SerializationInfo info,
			System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
	}
}
