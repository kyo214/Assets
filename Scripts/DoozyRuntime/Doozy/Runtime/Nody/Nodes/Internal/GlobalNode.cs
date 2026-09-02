using System;

namespace Doozy.Runtime.Nody.Nodes.Internal;

[Serializable]
public abstract class GlobalNode : FlowNode
{
	protected GlobalNode()
		: base(NodeType.Global)
	{
	}

	public override void OnExit()
	{
		NodeState = NodeState.Running;
	}
}
