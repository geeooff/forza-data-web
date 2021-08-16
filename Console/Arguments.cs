using ForzaData.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace ForzaData.Console
{
	public class Arguments
	{
		[PowArgs.Attributes.Argument("UDP local network port to bind")]
		public int Port { get; set; } = 7777;

		[PowArgs.Attributes.Argument("IP of server (your PC or console running the game) to listen to", required: true)]
		public string ServerIpAddress { get; set; }

		[PowArgs.Attributes.Argument("Show this program arguments help")]
		public bool Help { get; set; } = false;
	}
}
