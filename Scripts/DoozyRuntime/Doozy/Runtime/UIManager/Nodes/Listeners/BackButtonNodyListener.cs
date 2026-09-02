using Doozy.Runtime.Nody;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Input;
using Doozy.Runtime.UIManager.Nodes.Listeners.Internal;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Nodes.Listeners;

public class BackButtonNodyListener : BaseNodyListener
{
	private UnityAction<Signal> callback { get; }

	public BackButtonNodyListener(FlowNode node, UnityAction<Signal> callback)
		: base(node)
	{
		this.callback = callback;
	}

	protected override void ConnectReceiver()
	{
		BackButton.stream.ConnectReceiver(base.receiver);
	}

	protected override void DisconnectReceiver()
	{
		BackButton.stream.DisconnectReceiver(base.receiver);
	}

	protected override void ProcessSignal(Signal signal)
	{
		callback?.Invoke(signal);
	}
}
