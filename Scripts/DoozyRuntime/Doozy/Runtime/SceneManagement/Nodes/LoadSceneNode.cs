using System;
using System.Collections.Generic;
using Doozy.Runtime.Global;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Nody.Nodes.Internal;
using Doozy.Runtime.Reactor;
using UnityEngine.SceneManagement;

namespace Doozy.Runtime.SceneManagement.Nodes;

[Serializable]
[NodyMenuPath("Scene Management", "Load Scene")]
public sealed class LoadSceneNode : SimpleNode
{
	public GetSceneBy GetSceneBy;

	public LoadSceneMode LoadSceneMode;

	public bool AllowSceneActivation = true;

	public float SceneActivationDelay = 0.2f;

	public int SceneBuildIndex;

	public string SceneName = "";

	public bool WaitForSceneToLoad = true;

	public bool PreventLoadingSameScene = true;

	public bool ConnectProgressor;

	public ProgressorId ProgressorId = new ProgressorId();

	public LoadSceneNode()
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
		if (!WaitForSceneToLoad)
		{
			GoToNextNode(base.firstOutputPort);
		}
	}

	private void Run()
	{
		if (PreventLoadingSameScene)
		{
			switch (GetSceneBy)
			{
			case GetSceneBy.Name:
				if (SceneLoader.IsSceneLoaded(SceneName))
				{
					GoToNextNode(base.firstOutputPort);
					return;
				}
				break;
			case GetSceneBy.BuildIndex:
				if (SceneLoader.IsSceneLoaded(SceneBuildIndex))
				{
					GoToNextNode(base.firstOutputPort);
					return;
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		SceneLoader sceneLoader = SceneLoader.GetLoader().SetLoadSceneMode(LoadSceneMode).SetLoadSceneBy(GetSceneBy)
			.SetSceneName(SceneName)
			.SetSceneBuildIndex(SceneBuildIndex)
			.SetAllowSceneActivation(AllowSceneActivation)
			.SetSceneActivationDelay(SceneActivationDelay)
			.SetSelfDestructAfterSceneLoaded(selfDestruct: true);
		if (ConnectProgressor)
		{
			IEnumerable<Progressor> progressors = Progressor.GetProgressors(ProgressorId.Category, ProgressorId.Name);
			if (progressors != null)
			{
				foreach (Progressor item in progressors)
				{
					sceneLoader.AddProgressor(item);
				}
			}
		}
		if (WaitForSceneToLoad)
		{
			if (AllowSceneActivation)
			{
				sceneLoader.OnSceneActivated.Event.AddListener(() =>
				{
					Coroutiner.ExecuteLater(() =>
					{
						GoToNextNode(base.firstOutputPort);
					}, 1);
				});
			}
			else
			{
				sceneLoader.OnSceneLoaded.Event.AddListener(() =>
				{
					Coroutiner.ExecuteLater(() =>
					{
						GoToNextNode(base.firstOutputPort);
					}, 1);
				});
			}
		}
		sceneLoader.LoadSceneAsync();
	}
}
