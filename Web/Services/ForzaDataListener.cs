using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Receiver.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Receiver.Services
{
    public class ForzaDataListener : IDisposable
    {
		private readonly ILogger _logger;
		private readonly AppSettings _settings;
		private readonly UdpClient _client;

		public ForzaDataListener(
			ILogger<ForzaDataListener> logger,
			IOptions<AppSettings> settingsAccessor)
		{
			_logger = logger;
			_settings = settingsAccessor.Value;
			_client = new UdpClient(_settings.UdpPort);
		}

		public void Dispose()
		{
			_client.Dispose();
		}

		//public void Start()
		//{
		//	_client.Receive()
		//}
	}
}
