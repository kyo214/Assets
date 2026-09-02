using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Toked;

public class LoadSceneManager : GenericSingleton<LoadSceneManager>
{
	public bool onLoadingTransition;

	private const float DEFULT_FAKE_WAITING_TIME = 0.5f;

	public void LoadSceneAsync(string sceneName, bool usingInputToLoad = false, float customWaitTime = 0f)
	{
		if (EventSystem.current != null)
		{
			EventSystem.current.SetSelectedGameObject(null);
		}
		StartCoroutine(DoLoadSceneAsync(sceneName, usingInputToLoad, customWaitTime));
		GC.Collect();
	}

	private IEnumerator DoLoadSceneAsync(string sceneName, bool usingInputToLoad = false, float customWaitTime = 0.5f)
	{
		onLoadingTransition = true;
		if (customWaitTime > 0f)
		{
			yield return new WaitForSecondsRealtime(customWaitTime);
		}
		AsyncOperation asyncScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
		bool canLoadNextScene = false;
		while ((asyncScene != null && !asyncScene.isDone) || !canLoadNextScene)
		{
			yield return null;
			if (asyncScene.progress >= 0.9f && (!usingInputToLoad || InputManager.CheckAnyInput()))
			{
				break;
			}
		}
		yield return asyncScene;
		asyncScene.allowSceneActivation = true;
		onLoadingTransition = false;
	}
}
