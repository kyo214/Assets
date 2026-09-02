using System;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes.Internal;
using Doozy.Runtime.UIManager.Input;

namespace Doozy.Runtime.UIManager.Nodes;

[Serializable]
[NodyMenuPath("UI Manager", "Back Button")]
public sealed class BackButtonNode : SimpleNode
{
	public enum Command
	{
		Disable = 0,
		Enable = 1,
		EnableByForce = 2
	}

	public Command NodeCommand = Command.Enable;

	public BackButtonNode()
	{
		AddInputPort().SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
		AddOutputPort().SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
	}

	public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
	{
		base.OnEnter(previousNode, previousPort);
		switch (NodeCommand)
		{
		case Command.Disable:
			BackButton.Disable();
			break;
		case Command.Enable:
			BackButton.Enable();
			break;
		case Command.EnableByForce:
			BackButton.EnableByForce();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		GoToNextNode(base.firstOutputPort);
	}
}
