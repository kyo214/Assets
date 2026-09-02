using Doozy.Runtime.Nody;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Containers;
using Doozy.Runtime.UIManager.Nodes.Listeners.Internal;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Nodes.Listeners;

public class UIViewNodyListener : BaseNodyListener
{
	private UnityAction<UIViewSignalData> callback { get; }

	public UIViewNodyListener(FlowNode node, UnityAction<UIViewSignalData> callback)
		: base(node)
	{
		this.callback = callback;
	}

	protected override void ConnectReceiver()
	{
		UIView.stream.ConnectReceiver(base.receiver);
	}

	protected override void DisconnectReceiver()
	{
		UIView.stream.DisconnectReceiver(base.receiver);
	}

	protected override void ProcessSignal(Signal signal)
	{
		if (signal.hasValue && signal.valueAsObject is UIViewSignalData arg)
		{
			callback?.Invoke(arg);
		}
	}
}
