using System;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes.Internal;
using Doozy.Runtime.Signals;

namespace Doozy.Runtime.UIManager.Nodes;

[Serializable]
[NodyMenuPath("UI Manager", "Signal")]
public sealed class SignalNode : SimpleNode
{
	public SignalPayload Payload;

	public SignalNode()
	{
		AddInputPort().SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
		AddOutputPort().SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
	}

	public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
	{
		base.OnEnter(previousNode, previousPort);
		Payload?.SendSignal();
		GoToNextNode(base.firstOutputPort);
	}
}
