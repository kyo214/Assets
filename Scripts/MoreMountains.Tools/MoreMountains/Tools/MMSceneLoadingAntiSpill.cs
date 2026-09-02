using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace MoreMountains.Tools;

public class MMSceneLoadingAntiSpill
{
	protected Scene _antiSpillScene;

	protected Scene _destinationScene;

	protected UnityAction<Scene, Scene> _onActiveSceneChangedCallback;

	protected string _sceneToLoadName;

	protected List<GameObject> _spillSceneRoots = new List<GameObject>(50);

	public virtual void PrepareAntiFill(string sceneToLoadName)
	{
		_antiSpillScene = SceneManager.CreateScene("AntiSpill_" + sceneToLoadName);
		_destinationScene = default;
		_sceneToLoadName = sceneToLoadName;
		if (_onActiveSceneChangedCallback != null)
		{
			SceneManager.activeSceneChanged -= _onActiveSceneChangedCallback;
		}
		_onActiveSceneChangedCallback = OnActiveSceneChanged;
		SceneManager.activeSceneChanged += _onActiveSceneChangedCallback;
		SceneManager.SetActiveScene(_antiSpillScene);
	}

	protected virtual void OnActiveSceneChanged(Scene from, Scene to)
	{
		if (from == _antiSpillScene)
		{
			SceneManager.activeSceneChanged -= _onActiveSceneChangedCallback;
			_onActiveSceneChangedCallback = null;
			EmptyAntiSpillScene();
		}
	}

	protected virtual void EmptyAntiSpillScene()
	{
		if (!_antiSpillScene.IsValid() || !_antiSpillScene.isLoaded)
		{
			return;
		}
		_spillSceneRoots.Clear();
		_antiSpillScene.GetRootGameObjects(_spillSceneRoots);
		_destinationScene = SceneManager.GetSceneByName(_sceneToLoadName);
		if (_spillSceneRoots.Count > 0 && _destinationScene.IsValid() && _destinationScene.isLoaded)
		{
			foreach (GameObject spillSceneRoot in _spillSceneRoots)
			{
				SceneManager.MoveGameObjectToScene(spillSceneRoot, _destinationScene);
			}
		}
		SceneManager.UnloadSceneAsync(_antiSpillScene);
	}
}
