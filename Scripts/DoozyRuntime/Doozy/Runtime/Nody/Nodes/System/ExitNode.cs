using System;
using Doozy.Runtime.Nody.Nodes.Internal;
using UnityEngine;

namespace Doozy.Runtime.Nody.Nodes.System;

[Serializable]
public sealed class ExitNode : SystemNode
{
	public override int minNumberOfInputPorts => 1;

	public override int minNumberOfOutputPorts => 0;

	public ExitNode()
		: base(SystemNodeType.Exit)
	{
		AddInputPort();
		base.lastInputPort.SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
	}

	public override FlowNode Clone()
	{
		return UnityEngine.Object.Instantiate(this);
	}
}
