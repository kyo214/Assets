using System;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes.Internal;

namespace Doozy.Runtime.SceneManagement.Nodes;

[Serializable]
[NodyMenuPath("Scene Management", "Activate Loaded Scenes")]
public sealed class ActivateLoadedScenesNode : SimpleNode
{
	public ActivateLoadedScenesNode()
	{
		AddInputPort().SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
		AddOutputPort().SetCanBeDeleted(canBeDeleted: false).SetCanBeReordered(canBeReordered: false);
		base.canBeDeleted = true;
		base.runUpdate = false;
		base.runFixedUpdate = false;
		base.runLateUpdate = false;
		base.passthrough = true;
		base.clearGraphHistory = false;
	}

	public override void OnEnter(FlowNode previousNode = null, FlowPort previousPort = null)
	{
		base.OnEnter(previousNode, previousPort);
		Run();
		GoToNextNode(base.firstOutputPort);
	}

	private void Run()
	{
		SceneLoader.ActivateLoadedScenes();
	}
}
