using System;
using Doozy.Runtime.Nody.Nodes.Internal;
using UnityEngine;

namespace Doozy.Runtime.Nody.Nodes;

[Serializable]
[NodyMenuPath("System", "Application Quit")]
public sealed class ApplicationQuitNode : SimpleNode
{
	public ApplicationQuitNode()
	{
		AddInputPort().SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
	}

	public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
	{
		base.OnEnter(previousNode, previousPort);
		Application.Quit();
	}
}
