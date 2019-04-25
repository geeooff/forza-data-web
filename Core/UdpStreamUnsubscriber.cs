using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ForzaData.Core
{
	internal class UdpStreamUnsubscriber : IDisposable
	{
		private readonly ICollection<IObserver<byte[]>> _observers;
		private readonly IObserver<byte[]> _observer;

		internal UdpStreamUnsubscriber(ICollection<IObserver<byte[]>> observers, IObserver<byte[]> observer)
		{
			_observers = observers;
			_observer = observer;
		}

		public void Dispose()
		{
			if (_observers.Contains(_observer))
			{
				_observers.Remove(_observer);
			}
		}
	}
}
