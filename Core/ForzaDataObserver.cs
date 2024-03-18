namespace ForzaData.Core;

public abstract class ForzaDataObserver : IObserver<ForzaDataStruct>
{
	private IDisposable? _unsubscriber;

	protected ForzaDataObserver()
	{
		
	}

	public void Subscribe(IObservable<ForzaDataStruct> listener)
	{
		_unsubscriber = listener.Subscribe(this);
	}

	public void Unsubscribe()
	{
		if (_unsubscriber != null)
		{
			_unsubscriber.Dispose();
			_unsubscriber = null;
		}
	}

	public abstract void OnCompleted();

	public abstract void OnError(ForzaDataException error);

	public abstract void OnError(Exception error);

	public abstract void OnNext(ForzaDataStruct value);
}
