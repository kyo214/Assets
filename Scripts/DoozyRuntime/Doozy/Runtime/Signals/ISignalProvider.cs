namespace Doozy.Runtime.Signals;

public interface ISignalProvider
{
	ProviderAttributes attributes { get; }

	SignalStream stream { get; }

	bool isConnected { get; }

	void OpenStream();

	void CloseStream();

	bool SendSignal();

	bool SendSignal<T>(T signalValue);
}
