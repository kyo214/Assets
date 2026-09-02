using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Fusion;

public class NetworkSceneManagerDefault : NetworkSceneManagerBase
{
	[Header("Single Peer Options")]
	public int PostLoadDelayFrames = 1;

	protected virtual YieldInstruction LoadSceneAsync(SceneRef sceneRef, LoadSceneParameters parameters, Action<Scene> loaded)
	{
		if (!TryGetScenePath(sceneRef, out var scenePath))
		{
			throw new InvalidOperationException($"Not going to load {sceneRef}: unable to find the scene name");
		}
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scenePath, parameters);
		bool alreadyHandled = false;
		UnityAction<Scene, LoadSceneMode> sceneLoadedHandler = (Scene scene, LoadSceneMode loadSceneMode) =>
		{
			if (NetworkSceneManagerBase.IsScenePathOrNameEqual(scene, scenePath))
			{
				alreadyHandled = true;
				loaded(scene);
			}
		};
		SceneManager.sceneLoaded += sceneLoadedHandler;
		asyncOperation.completed += (AsyncOperation asyncOperation2) =>
		{
			SceneManager.sceneLoaded -= sceneLoadedHandler;
		};
		return asyncOperation;
	}

	protected virtual YieldInstruction UnloadSceneAsync(Scene scene)
	{
		return SceneManager.UnloadSceneAsync(scene);
	}

	protected override IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
	{
		if (base.Runner.Config.PeerMode == NetworkProjectConfig.PeerModes.Single)
		{
			return SwitchSceneSinglePeer(prevScene, newScene, finished);
		}
		return SwitchSceneMultiplePeer(prevScene, newScene, finished);
	}

	protected virtual IEnumerator SwitchSceneMultiplePeer(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
	{
		Scene activeScene = SceneManager.GetActiveScene();
		bool num = prevScene == default(SceneRef) && IsScenePathOrNameEqual(activeScene, newScene);
		LoadSceneParameters loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Additive, NetworkProjectConfig.ConvertPhysicsMode(base.Runner.Config.PhysicsEngine));
		Scene sceneToUnload = base.Runner.MultiplePeerUnityScene;
		GameObject[] tempSceneSpawnedPrefabs = (base.Runner.IsMultiplePeerSceneTemp ? sceneToUnload.GetRootGameObjects() : Array.Empty<GameObject>());
		if (num && NetworkRunner.GetRunnerForScene(activeScene) == null && SceneManager.sceneCount > 1)
		{
			yield return UnloadSceneAsync(activeScene);
		}
		if (SceneManager.sceneCount == 1 && tempSceneSpawnedPrefabs.Length == 0)
		{
			loadSceneParameters.loadSceneMode = LoadSceneMode.Single;
		}
		else if (sceneToUnload.IsValid() && base.Runner.TryMultiplePeerAssignTempScene())
		{
			yield return UnloadSceneAsync(sceneToUnload);
		}
		Scene loadedScene = default;
		yield return LoadSceneAsync(newScene, loadSceneParameters, (Scene scene) =>
		{
			loadedScene = scene;
		});
		if (!loadedScene.IsValid())
		{
			throw new InvalidOperationException($"Failed to load scene {newScene}: async op failed");
		}
		List<NetworkObject> sceneObjects = FindNetworkObjects(loadedScene, disable: true, addVisibilityNodes: true);
		Scene multiplePeerUnityScene = base.Runner.MultiplePeerUnityScene;
		base.Runner.MultiplePeerUnityScene = loadedScene;
		if (multiplePeerUnityScene.IsValid())
		{
			if (tempSceneSpawnedPrefabs.Length != 0)
			{
				GameObject[] array = tempSceneSpawnedPrefabs;
				for (int num2 = 0; num2 < array.Length; num2++)
				{
					SceneManager.MoveGameObjectToScene(array[num2], loadedScene);
				}
			}
			yield return UnloadSceneAsync(multiplePeerUnityScene);
		}
		finished(sceneObjects);
	}

	protected virtual IEnumerator SwitchSceneSinglePeer(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished)
	{
		Scene activeScene = SceneManager.GetActiveScene();
		Scene loadedScene;
		if (prevScene == default(SceneRef) && IsScenePathOrNameEqual(activeScene, newScene))
		{
			loadedScene = activeScene;
		}
		else
		{
			LoadSceneParameters parameters = new LoadSceneParameters(LoadSceneMode.Single);
			loadedScene = default;
			yield return LoadSceneAsync(newScene, parameters, (Scene scene) =>
			{
				loadedScene = scene;
			});
			if (!loadedScene.IsValid())
			{
				throw new InvalidOperationException($"Failed to load scene {newScene}: async op failed");
			}
		}
		int i = PostLoadDelayFrames;
		while (i > 0)
		{
			yield return null;
			int num = i - 1;
			i = num;
		}
		List<NetworkObject> sceneObjects = FindNetworkObjects(loadedScene);
		finished(sceneObjects);
	}
}
