using System;

namespace Doozy.Runtime.Nody.Nodes.Internal;

[Serializable]
public abstract class SimpleNode : FlowNode
{
	protected SimpleNode()
		: base(NodeType.Simple)
	{
	}
}
