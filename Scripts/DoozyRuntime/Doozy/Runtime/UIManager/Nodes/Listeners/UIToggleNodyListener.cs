using Doozy.Runtime.Nody;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Nodes.Listeners.Internal;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Nodes.Listeners;

public class UIToggleNodyListener : BaseNodyListener
{
	private UnityAction<UIToggleSignalData> callback { get; }

	public UIToggleNodyListener(FlowNode node, UnityAction<UIToggleSignalData> callback)
		: base(node)
	{
		this.callback = callback;
	}

	protected override void ConnectReceiver()
	{
		UIToggle.stream.ConnectReceiver(base.receiver);
	}

	protected override void DisconnectReceiver()
	{
		UIToggle.stream.DisconnectReceiver(base.receiver);
	}

	protected override void ProcessSignal(Signal signal)
	{
		if (signal.hasValue && signal.valueAsObject is UIToggleSignalData arg)
		{
			callback?.Invoke(arg);
		}
	}
}
