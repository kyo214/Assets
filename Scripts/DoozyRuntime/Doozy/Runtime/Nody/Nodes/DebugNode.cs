using System;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Nody.Nodes.Internal;
using UnityEngine;

namespace Doozy.Runtime.Nody.Nodes;

[Serializable]
[NodyMenuPath("Utils", "Debug")]
public sealed class DebugNode : SimpleNode
{
	[SerializeField]
	private string Message;

	public override int minNumberOfInputPorts => 1;

	public override int minNumberOfOutputPorts => 1;

	public string message
	{
		get
		{
			return Message;
		}
		set
		{
			Message = value;
		}
	}

	public DebugNode()
	{
		AddInputPort();
		base.lastInputPort.SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
		AddOutputPort();
		base.lastOutputPort.SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
	}

	public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
	{
		base.OnEnter(previousNode, previousPort);
		if (!Message.IsNullOrEmpty())
		{
			Debug.Log(Message);
		}
		GoToNextNode(base.firstOutputPort);
	}

	public override FlowNode Clone()
	{
		return UnityEngine.Object.Instantiate(this);
	}
}
