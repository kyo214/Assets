using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Fusion;

public static class FusionUnitySceneManagerUtils
{
	public static int GetSceneBuildIndex(string nameOrPath)
	{
		if (nameOrPath.IndexOf('/') >= 0)
		{
			return SceneUtility.GetBuildIndexByScenePath(nameOrPath);
		}
		for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
		{
			string scenePathByBuildIndex = SceneUtility.GetScenePathByBuildIndex(i);
			GetFileNameWithoutExtensionPosition(scenePathByBuildIndex, out var index, out var length);
			if (length == nameOrPath.Length && string.Compare(scenePathByBuildIndex, index, nameOrPath, 0, length, ignoreCase: true) == 0)
			{
				return i;
			}
		}
		return -1;
	}

	public static int GetSceneIndex(IList<string> scenePathsOrNames, string nameOrPath)
	{
		if (nameOrPath.IndexOf('/') >= 0)
		{
			return scenePathsOrNames.IndexOf(nameOrPath);
		}
		for (int i = 0; i < scenePathsOrNames.Count; i++)
		{
			string text = scenePathsOrNames[i];
			GetFileNameWithoutExtensionPosition(text, out var index, out var length);
			if (length == nameOrPath.Length && string.Compare(text, index, nameOrPath, 0, length, ignoreCase: true) == 0)
			{
				return i;
			}
		}
		return -1;
	}

	public static void GetFileNameWithoutExtensionPosition(string nameOrPath, out int index, out int length)
	{
		int num = nameOrPath.LastIndexOf('/');
		if (num >= 0)
		{
			index = num + 1;
		}
		else
		{
			index = 0;
		}
		int num2 = nameOrPath.LastIndexOf('.');
		if (num2 > index)
		{
			length = num2 - index;
		}
		else
		{
			length = nameOrPath.Length - index;
		}
	}
}
