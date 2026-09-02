using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fusion;

public abstract class NetworkSceneManagerBase : Behaviour, INetworkSceneManager
{
	protected delegate void FinishedLoadingDelegate(IEnumerable<NetworkObject> sceneObjects);

	private static WeakReference<NetworkSceneManagerBase> s_currentlyLoading = new WeakReference<NetworkSceneManagerBase>(null);

	[InlineHelp]
	[ToggleLeft]
	[MultiPropertyDrawersFix]
	public bool ShowHierarchyWindowOverlay = true;

	private IEnumerator _runningCoroutine;

	private bool _currentSceneOutdated;

	private SceneRef _currentScene;

	public NetworkRunner Runner { get; private set; }

	protected virtual void OnEnable()
	{
	}

	protected virtual void OnDisable()
	{
	}

	protected virtual void LateUpdate()
	{
		if (!Runner)
		{
			return;
		}
		if (Runner.CurrentScene != _currentScene)
		{
			_currentSceneOutdated = true;
		}
		if (!_currentSceneOutdated || _runningCoroutine != null)
		{
			return;
		}
		if (s_currentlyLoading.TryGetTarget(out var target))
		{
			if ((bool)target)
			{
				return;
			}
			s_currentlyLoading.SetTarget(null);
		}
		SceneRef currentScene = _currentScene;
		_currentScene = Runner.CurrentScene;
		_currentSceneOutdated = false;
		_runningCoroutine = SwitchSceneWrapper(currentScene, _currentScene);
		StartCoroutine(_runningCoroutine);
	}

	public static bool IsScenePathOrNameEqual(Scene scene, string nameOrPath)
	{
		if (!(scene.path == nameOrPath))
		{
			return scene.name == nameOrPath;
		}
		return true;
	}

	public static bool TryGetScenePathFromBuildSettings(SceneRef sceneRef, out string path)
	{
		if (sceneRef.IsValid)
		{
			path = SceneUtility.GetScenePathByBuildIndex(sceneRef);
			if (!string.IsNullOrEmpty(path))
			{
				return true;
			}
		}
		path = string.Empty;
		return false;
	}

	public virtual bool TryGetScenePath(SceneRef sceneRef, out string path)
	{
		return TryGetScenePathFromBuildSettings(sceneRef, out path);
	}

	public virtual bool TryGetSceneRef(string nameOrPath, out SceneRef sceneRef)
	{
		int sceneBuildIndex = FusionUnitySceneManagerUtils.GetSceneBuildIndex(nameOrPath);
		if (sceneBuildIndex >= 0)
		{
			sceneRef = sceneBuildIndex;
			return true;
		}
		sceneRef = default;
		return false;
	}

	public bool IsScenePathOrNameEqual(Scene scene, SceneRef sceneRef)
	{
		if (TryGetScenePath(sceneRef, out var path))
		{
			return IsScenePathOrNameEqual(scene, path);
		}
		return false;
	}

	public List<NetworkObject> FindNetworkObjects(Scene scene, bool disable = true, bool addVisibilityNodes = false)
	{
		List<NetworkObject> list = new List<NetworkObject>();
		GameObject[] rootGameObjects = scene.GetRootGameObjects();
		List<NetworkObject> list2 = new List<NetworkObject>();
		GameObject[] array = rootGameObjects;
		foreach (GameObject gameObject in array)
		{
			list.Clear();
			gameObject.GetComponentsInChildren(includeInactive: true, list);
			foreach (NetworkObject item in list)
			{
				if (item.Flags.IsSceneObject() && (item.gameObject.activeInHierarchy || item.Flags.IsActivatedByUser()))
				{
					list2.Add(item);
				}
			}
			if (addVisibilityNodes)
			{
				RunnerVisibilityNode.AddVisibilityNodes(gameObject, Runner);
			}
		}
		if (disable)
		{
			foreach (NetworkObject item2 in list2)
			{
				item2.gameObject.SetActive(value: false);
			}
		}
		return list2;
	}

	void INetworkSceneManager.Initialize(NetworkRunner runner)
	{
		Initialize(runner);
	}

	void INetworkSceneManager.Shutdown(NetworkRunner runner)
	{
		Shutdown(runner);
	}

	bool INetworkSceneManager.IsReady(NetworkRunner runner)
	{
		if (_runningCoroutine != null)
		{
			return false;
		}
		if (_currentSceneOutdated)
		{
			return false;
		}
		if (runner.CurrentScene != _currentScene)
		{
			return false;
		}
		return true;
	}

	protected virtual void Initialize(NetworkRunner runner)
	{
		Runner = runner;
	}

	protected virtual void Shutdown(NetworkRunner runner)
	{
		try
		{
			if (_runningCoroutine != null)
			{
				LogWarn($"There is an ongoing scene load ({_currentScene}), stopping and disposing coroutine.");
				StopCoroutine(_runningCoroutine);
				(_runningCoroutine as IDisposable)?.Dispose();
			}
		}
		finally
		{
			Runner = null;
			_runningCoroutine = null;
			_currentScene = SceneRef.None;
			_currentSceneOutdated = false;
		}
	}

	protected abstract IEnumerator SwitchScene(SceneRef prevScene, SceneRef newScene, FinishedLoadingDelegate finished);

	[Conditional("FUSION_NETWORK_SCENE_MANAGER_TRACE")]
	protected void LogTrace(string msg)
	{
	}

	protected void LogError(string msg)
	{
		Log.Error("[NetworkSceneManager] " + ((this != null) ? base.name : "<destroyed>") + ": " + msg);
	}

	protected void LogWarn(string msg)
	{
		Log.Warn("[NetworkSceneManager] " + ((this != null) ? base.name : "<destroyed>") + ": " + msg);
	}

	private IEnumerator SwitchSceneWrapper(SceneRef prevScene, SceneRef newScene)
	{
		bool finishCalled = false;
		Dictionary<Guid, NetworkObject> sceneObjects = new Dictionary<Guid, NetworkObject>();
		Exception error = null;
		FinishedLoadingDelegate finished = (IEnumerable<NetworkObject> objects) =>
		{
			finishCalled = true;
			foreach (NetworkObject @object in objects)
			{
				sceneObjects.Add(@object.NetworkGuid, @object);
			}
		};
		try
		{
			s_currentlyLoading.SetTarget(this);
			Runner.InvokeSceneLoadStart();
			IEnumerator coro = SwitchScene(prevScene, newScene, finished);
			bool next = true;
			while (next)
			{
				try
				{
					next = coro.MoveNext();
				}
				catch (Exception ex)
				{
					error = ex;
					break;
				}
				if (next)
				{
					yield return coro.Current;
				}
			}
		}
		finally
		{
			NetworkSceneManagerBase networkSceneManagerBase = this;
			s_currentlyLoading.SetTarget(null);
			networkSceneManagerBase._runningCoroutine = null;
		}
		if (error != null)
		{
			LogError($"Failed to switch scenes: {error}");
			yield break;
		}
		if (!finishCalled)
		{
			LogError("Failed to switch scenes: SwitchScene implementation did not invoke finished delegate");
			yield break;
		}
		Runner.RegisterSceneObjects(sceneObjects.Values);
		Runner.InvokeSceneLoadDone();
	}
}
