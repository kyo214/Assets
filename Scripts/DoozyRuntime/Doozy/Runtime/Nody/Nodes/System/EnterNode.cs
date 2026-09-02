using System;
using Doozy.Runtime.Nody.Nodes.Internal;
using UnityEngine;

namespace Doozy.Runtime.Nody.Nodes.System;

[Serializable]
public sealed class EnterNode : SystemNode
{
	public override int minNumberOfInputPorts => 0;

	public override int minNumberOfOutputPorts => 1;

	public EnterNode()
		: base(SystemNodeType.Enter)
	{
		AddOutputPort();
		base.lastOutputPort.SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
	}

	public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
	{
		base.OnEnter(previousNode, previousPort);
		GoToNextNode(base.firstOutputPort);
	}

	public override FlowNode Clone()
	{
		return UnityEngine.Object.Instantiate(this);
	}
}
