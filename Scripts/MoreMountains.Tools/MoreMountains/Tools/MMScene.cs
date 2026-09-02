using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreMountains.Tools;

public class MMScene
{
	public static Scene[] GetLoadedScenes()
	{
		int sceneCount = SceneManager.sceneCount;
		List<Scene> list = new List<Scene>(sceneCount);
		for (int i = 0; i < sceneCount; i++)
		{
			Scene sceneAt = SceneManager.GetSceneAt(i);
			if (sceneAt.isLoaded)
			{
				list.Add(sceneAt);
			}
			else
			{
				Debug.LogWarning(sceneAt.name + " NOT LOADED");
			}
		}
		return list.ToArray();
	}

	public static List<string> GetScenesInBuild()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
		{
			string scenePathByBuildIndex = SceneUtility.GetScenePathByBuildIndex(i);
			int num = scenePathByBuildIndex.LastIndexOf("/", StringComparison.Ordinal);
			list.Add(scenePathByBuildIndex.Substring(num + 1, scenePathByBuildIndex.LastIndexOf(".", StringComparison.Ordinal) - num - 1));
		}
		return list;
	}

	public static bool SceneInBuild(string sceneName)
	{
		return GetScenesInBuild().Contains(sceneName);
	}
}
