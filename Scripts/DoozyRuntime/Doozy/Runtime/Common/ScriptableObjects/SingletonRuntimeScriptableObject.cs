using Doozy.Runtime.Common.Attributes;
using UnityEngine;

namespace Doozy.Runtime.Common.ScriptableObjects;

public class SingletonRuntimeScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
	[ClearOnReload]
	private static T s_instance;

	private static string fileName => typeof(T).Name ?? "";

	private static string assetFileName => fileName + ".asset";

	private static string assetFolderPath => BasePathFinder<RuntimePath>.path + "/Data/Resources/";

	private static string assetFilePath => assetFolderPath + "/" + assetFileName;

	public static T instance
	{
		get
		{
			if (s_instance != null)
			{
				return s_instance;
			}
			s_instance = Resources.Load<T>(fileName);
			if (s_instance != null)
			{
				return s_instance;
			}
			s_instance = ScriptableObject.CreateInstance<T>();
			return s_instance;
		}
	}
}
