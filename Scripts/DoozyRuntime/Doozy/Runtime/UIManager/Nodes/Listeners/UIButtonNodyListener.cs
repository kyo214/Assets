using Doozy.Runtime.Nody;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Components;
using Doozy.Runtime.UIManager.Nodes.Listeners.Internal;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Nodes.Listeners;

public class UIButtonNodyListener : BaseNodyListener
{
	private UnityAction<UIButtonSignalData> callback { get; }

	public UIButtonNodyListener(FlowNode node, UnityAction<UIButtonSignalData> callback)
		: base(node)
	{
		this.callback = callback;
	}

	protected override void ConnectReceiver()
	{
		UIButton.stream.ConnectReceiver(base.receiver);
	}

	protected override void DisconnectReceiver()
	{
		UIButton.stream.DisconnectReceiver(base.receiver);
	}

	protected override void ProcessSignal(Signal signal)
	{
		if (signal.hasValue && signal.valueAsObject is UIButtonSignalData arg)
		{
			callback?.Invoke(arg);
		}
	}
}
