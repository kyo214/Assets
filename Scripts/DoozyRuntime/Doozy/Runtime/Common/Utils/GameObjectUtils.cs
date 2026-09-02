using UnityEngine;

namespace Doozy.Runtime.Common.Utils;

public static class GameObjectUtils
{
	public static T AddToScene<T>(bool isSingleton, bool selectGameObjectAfterCreation) where T : MonoBehaviour
	{
		return AddToScene<T>(ObjectNames.NicifyVariableName(typeof(T).Name), isSingleton, selectGameObjectAfterCreation);
	}

	public static T AddToScene<T>(string gameObjectName, bool isSingleton, bool selectGameObjectAfterCreation) where T : MonoBehaviour
	{
		T val = Object.FindObjectOfType<T>();
		if ((val != null) & isSingleton)
		{
			return val;
		}
		val = new GameObject(gameObjectName, typeof(T)).GetComponent<T>();
		_ = val.gameObject;
		return val;
	}
}
