using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Listeners.Internal;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Listeners;

[AddComponentMenu("Signals/Listeners/Signal Listener")]
public class SignalListener : BaseListener
{
	[SerializeField]
	protected StreamId StreamId;

	public SignalEvent OnSignal = new SignalEvent();

	public StreamId streamId => StreamId;

	public SignalStream stream { get; private set; }

	public UnityAction<Signal> signalCallback { get; }

	protected virtual void OnEnable()
	{
		ConnectReceiver();
	}

	protected virtual void OnDisable()
	{
		DisconnectReceiver();
	}

	protected override void ConnectReceiver()
	{
		stream = SignalStream.Get(streamId.Category, streamId.Name).ConnectReceiver(base.receiver);
	}

	protected override void DisconnectReceiver()
	{
		stream.DisconnectReceiver(base.receiver);
	}

	protected override void ProcessSignal(Signal signal)
	{
		signalCallback?.Invoke(signal);
		Callback?.Execute();
		OnSignal?.Invoke(signal);
	}
}
