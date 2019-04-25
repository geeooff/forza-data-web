using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Receiver.Configuration
{
    public class AppSettings
    {
		public AppSettings()
		{
			UdpPort = 7777;
		}

		public int UdpPort { get; set; }
	}
}
