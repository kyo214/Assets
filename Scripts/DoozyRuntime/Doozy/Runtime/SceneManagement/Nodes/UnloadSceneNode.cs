using System;
using Doozy.Runtime.Common;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes.Internal;
using UnityEngine.SceneManagement;

namespace Doozy.Runtime.SceneManagement.Nodes;

[Serializable]
[NodyMenuPath("Scene Management", "Unload Scene")]
public sealed class UnloadSceneNode : SimpleNode
{
	public GetSceneBy GetSceneBy;

	public int SceneBuildIndex;

	public string SceneName = "";

	public bool WaitForSceneToUnload;

	public UnloadSceneNode()
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
		if (WaitForSceneToUnload)
		{
			SingletonBehaviour<SceneDirector>.instance.onSceneUnloaded.AddListener(SceneUnloaded);
		}
		Run();
		if (!WaitForSceneToUnload)
		{
			GoToNextNode(base.firstOutputPort);
		}
	}

	private void SceneUnloaded(Scene unloadedScene)
	{
		switch (GetSceneBy)
		{
		case GetSceneBy.Name:
			if (!unloadedScene.name.Equals(SceneName))
			{
				return;
			}
			break;
		case GetSceneBy.BuildIndex:
			if (!unloadedScene.name.Equals(SceneManager.GetSceneByBuildIndex(SceneBuildIndex).name))
			{
				return;
			}
			break;
		}
		SingletonBehaviour<SceneDirector>.instance.onSceneUnloaded.RemoveListener(SceneUnloaded);
		GoToNextNode(base.firstOutputPort);
	}

	private void Run()
	{
		switch (GetSceneBy)
		{
		case GetSceneBy.Name:
			SceneDirector.UnloadSceneAsync(SceneName);
			break;
		case GetSceneBy.BuildIndex:
			SceneDirector.UnloadSceneAsync(SceneBuildIndex);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}
}
