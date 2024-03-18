namespace ForzaData.Core;

internal sealed class UdpStreamUnsubscriber : IDisposable
{
	private readonly ICollection<IObserver<byte[]>> _observers;
	private readonly IObserver<byte[]> _observer;

	internal UdpStreamUnsubscriber(ICollection<IObserver<byte[]>> observers, IObserver<byte[]> observer)
	{
		_observers = observers;
		_observer = observer;
	}

	public void Dispose() => _observers.Remove(_observer);
}
