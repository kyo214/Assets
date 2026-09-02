using UnityEngine;

namespace Doozy.Runtime.Common;

public class BasePathFinder<T> : ScriptableObject where T : ScriptableObject
{
	private static string s_foundPath = string.Empty;

	private static bool debugMode => false;

	public static string path => "Path cannot be returned outside the Unity Editor";

	private static string CleanPath(string rawPath)
	{
		return rawPath.Replace('\\', '/');
	}
}
