using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace ForzaData.Core
{
	public class ForzaDataListener : IDisposable, IObservable<ForzaDataStruct>
	{
		private readonly UdpClient _udpClient;
		private readonly ICollection<IObserver<ForzaDataStruct>> _observers;
		private readonly ForzaDataReader _reader;

		private IPEndPoint _serverEndPoint;
		private ForzaDataStruct? _lastData;

		public ForzaDataListener(int port, IPAddress serverIpAddress)
		{
			_udpClient = new UdpClient(port);
			_observers = new List<IObserver<ForzaDataStruct>>();
			_reader = new ForzaDataReader();

			_serverEndPoint = new IPEndPoint(serverIpAddress, 0);
			_lastData = null;
		}

		public void Close()
		{
			_udpClient.Close();
		}

		public void Dispose()
		{
			_udpClient.Dispose();
		}

		public IDisposable Subscribe(IObserver<ForzaDataStruct> observer)
		{
			if (!_observers.Contains(observer))
			{
				_observers.Add(observer);

				if (_lastData != null)
				{
					observer.OnNext(_lastData.Value);
				}
			}
			return new ForzaDataUnsubscriber(_observers, observer);
		}

		private void NotifyData(ForzaDataStruct data)
		{
			foreach (var observer in _observers)
			{
				observer.OnNext(data);
			}
		}

		private void NotifyError(Exception error)
		{
			foreach (var observer in _observers)
			{
				observer.OnError(error);
			}
		}

		private void NotifyCompletion()
		{
			foreach (var observer in _observers)
			{
				observer.OnCompleted();
			}
		}

		public void Listen(CancellationToken cancellation)
		{
			while (true)
			{
				IAsyncResult receiveResult = _udpClient.BeginReceive(ReceiveCallback, null);

				// wait for async receive or cancel requested
				WaitHandle.WaitAny(new WaitHandle[] { receiveResult.AsyncWaitHandle, cancellation.WaitHandle });

				if (cancellation.IsCancellationRequested)
				{
					// don't have choice, UdpClient will trigger receiveCallback on UDP socket closing
					_udpClient.Close();
					break;
				}
			}

			// end of notifications
			NotifyCompletion();

			// notify end of operations if requested
			cancellation.ThrowIfCancellationRequested();
		}

		private void ReceiveCallback(IAsyncResult asyncResult)
		{
			byte[] rawData;
			ForzaDataStruct forzaData;

			try
			{
				rawData = _udpClient.EndReceive(asyncResult, ref _serverEndPoint);
			}
			catch (ObjectDisposedException)
			{
				// UDP client has been closed previously to abort connect/receive
				// no need to notify observers or throw exception
				return;
			}
			catch (Exception ex)
			{
				// fatal exception: notify observers
				NotifyError(ex);
				throw;
			}

			try
			{
				forzaData = _reader.Read(rawData);

				// success reading
				NotifyData(forzaData);
			}
			catch (Exception ex)
			{
				// read exception: notified to observers
				NotifyError(new ForzaDataException("An error occured while trying to read data", ex));
			}
		}
	}
}
