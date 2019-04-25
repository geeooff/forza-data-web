using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace ForzaData.Core
{
	internal class ForzaDataUnsubscriber : IDisposable
	{
		private readonly ICollection<IObserver<ForzaDataStruct>> _observers;
		private readonly IObserver<ForzaDataStruct> _observer;

		internal ForzaDataUnsubscriber(ICollection<IObserver<ForzaDataStruct>> observers, IObserver<ForzaDataStruct> observer)
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
