using ForzaData.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForzaData.Core
{
	public abstract class ForzaDataObserver : IObserver<ForzaDataStruct>
	{
		private IDisposable _unsubscriber;

		public ForzaDataObserver()
		{
			
		}

		public virtual void Subscribe(ForzaDataListener listener)
		{
			_unsubscriber = listener.Subscribe(this);
		}

		public virtual void Unsubscribe()
		{
			if (_unsubscriber != null)
			{
				_unsubscriber.Dispose();
				_unsubscriber = null;
			}
		}

		public abstract void OnCompleted();

		public abstract void OnError(Exception error);

		public abstract void OnNext(ForzaDataStruct value);
	}
}
