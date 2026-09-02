using UnityEngine;

namespace Doozy.Runtime.Common.Extensions;

public static class GameObjectExtensions
{
	public static T GetOrAddComponent<T>(this GameObject target) where T : MonoBehaviour
	{
		T component = target.GetComponent<T>();
		if (component == null)
		{
			target.AddComponent<T>();
		}
		return component;
	}

	public static bool HasComponent<T>(this GameObject target) where T : MonoBehaviour
	{
		return target.GetComponent<T>() != null;
	}
}
