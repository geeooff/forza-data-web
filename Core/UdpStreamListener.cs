using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ForzaData.Core
{
	public class UdpStreamListener : IDisposable, IObservable<byte[]>
	{
		private readonly UdpClient _udpClient;
		private readonly ICollection<IObserver<byte[]>> _observers;

		private bool _isDisposed = false;
		private IPEndPoint _serverEndPoint;
		private byte[] _lastData;

		public UdpStreamListener(int port, IPAddress serverIpAddress)
		{
			_udpClient = new UdpClient(port);
			_observers = new List<IObserver<byte[]>>();
			_serverEndPoint = new IPEndPoint(serverIpAddress, 0);
			_lastData = null;
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (!_isDisposed)
			{
				if (isDisposing)
				{
					_udpClient.Dispose();
				}

				_lastData = null;
				_isDisposed = true;
			}
		}

		public void Dispose()
		{
			Dispose(true);
		}

		public IDisposable Subscribe(IObserver<byte[]> observer)
		{
			if (!_observers.Contains(observer))
			{
				_observers.Add(observer);

				if (_lastData != null)
				{
					observer.OnNext(_lastData);
				}
			}
			return new UdpStreamUnsubscriber(_observers, observer);
		}

		protected virtual void NotifyData(byte[] data)
		{
			foreach (var observer in _observers)
			{
				observer.OnNext(data);
			}
		}

		protected virtual void NotifyError(Exception error)
		{
			foreach (var observer in _observers)
			{
				observer.OnError(error);
			}
		}

		protected virtual void NotifyCompletion()
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
			byte[] data;

			try
			{
				_lastData = data = _udpClient.EndReceive(asyncResult, ref _serverEndPoint);
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

			// success reading
			NotifyData(data);
		}
	}
}
