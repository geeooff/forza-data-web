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

		[PowArgs.Attributes.Argument("IP of Forza Motorsport 7 server (your console) to listen to", required: true)]
		public string ServerIpAddress { get; set; }

		[PowArgs.Attributes.Argument("Data version, sled (V1) or car dash (V2)", required: true)]
		public ForzaDataVersion Version { get; set; }

		[PowArgs.Attributes.Argument("Show this program arguments help")]
		public bool Help { get; set; } = false;
	}
}
