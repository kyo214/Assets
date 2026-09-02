using System;
using UnityEngine;

namespace Doozy.Runtime.Nody.Nodes.Internal;

[Serializable]
public abstract class SystemNode : FlowNode
{
	[SerializeField]
	private SystemNodeType SystemNodeType;

	public SystemNodeType systemNodeType => SystemNodeType;

	protected SystemNode(SystemNodeType type)
		: base(NodeType.System)
	{
		base.canBeDeleted = false;
		SystemNodeType = type;
	}
}
