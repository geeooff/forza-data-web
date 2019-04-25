using ForzaData.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ForzaData.SampleRecorder
{
	public class UdpStreamSampleRecorder : UdpStreamObserver
	{
		public UdpStreamSampleRecorder()
		{

		}

		public override void OnCompleted()
		{
			System.Console.Error.WriteLine("Listening completed");
		}

		public override void OnError(Exception error)
		{
			System.Console.Error.WriteLine(error);
		}

		public override void OnNext(byte[] value)
		{
			System.Console.Error.WriteLine($"Received {value.Length} bytes");
		}
	}
}
