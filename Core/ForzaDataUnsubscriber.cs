namespace ForzaData.Core;

internal sealed class ForzaDataUnsubscriber : IDisposable
{
	private readonly ICollection<IObserver<ForzaDataStruct>> _observers;
	private readonly IObserver<ForzaDataStruct> _observer;

	internal ForzaDataUnsubscriber(ICollection<IObserver<ForzaDataStruct>> observers, IObserver<ForzaDataStruct> observer)
	{
		_observers = observers;
		_observer = observer;
	}

	public void Dispose() => _observers.Remove(_observer);
}
