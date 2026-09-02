namespace Doozy.Runtime.Signals;

public interface ISignalReceiver
{
	StreamConnection streamConnection { get; }

	ProviderId providerId { get; }

	SignalProvider providerReference { get; }

	StreamId streamId { get; }

	SignalStream stream { get; }

	bool isConnected { get; }

	void Connect();

	void Disconnect();

	void OnSignal(Signal signal);
}
