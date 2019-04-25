using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ForzaData.Core
{
	public class ForzaDataListener : UdpStreamListener, IObservable<ForzaDataStruct>
	{
		private readonly ICollection<IObserver<ForzaDataStruct>> _observers;
		private readonly ForzaDataReader _reader;

		private ForzaDataStruct? _lastData;

		public ForzaDataListener(int port, IPAddress serverIpAddress) : base(port, serverIpAddress)
		{
			_observers = new List<IObserver<ForzaDataStruct>>();
			_reader = new ForzaDataReader();

			_lastData = null;
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

		protected override void NotifyData(byte[] data)
		{
			base.NotifyData(data);

			ForzaDataStruct forzaData;

			try
			{
				forzaData = _reader.Read(data);

				// success reading
				NotifyData(forzaData);
			}
			catch (Exception ex)
			{
				// read exception: notified to observers
				NotifyError(new ForzaDataException("An error occured while trying to read data", ex));
			}
		}

		protected void NotifyData(ForzaDataStruct data)
		{
			foreach (var observer in _observers)
			{
				observer.OnNext(data);
			}
		}

		protected void NotifyError(ForzaDataException error)
		{
			foreach (var observer in _observers)
			{
				observer.OnError(error);
			}
		}

		protected override void NotifyCompletion()
		{
			base.NotifyCompletion();

			foreach (var observer in _observers)
			{
				observer.OnCompleted();
			}
		}
	}
}
