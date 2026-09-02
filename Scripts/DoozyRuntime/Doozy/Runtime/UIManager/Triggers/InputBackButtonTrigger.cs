using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Input;
using UnityEngine;

namespace Doozy.Runtime.UIManager.Triggers;

[AddComponentMenu("UI/Triggers/BackButton")]
public class InputBackButtonTrigger : SignalProvider
{
	public SignalEvent OnTrigger = new SignalEvent();

	private SignalReceiver receiver { get; set; }

	public InputBackButtonTrigger()
		: base(ProviderType.Global, "Input", "BackButton", typeof(InputBackButtonTrigger))
	{
	}

	protected override void Awake()
	{
		base.Awake();
		receiver = new SignalReceiver().SetOnSignalCallback(ProcessSignal);
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		BackButton.stream.ConnectReceiver(receiver);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		BackButton.stream.DisconnectReceiver(receiver);
	}

	private void ProcessSignal(Signal signal)
	{
		SendSignal(signal);
		OnTrigger?.Invoke(signal);
	}
}
