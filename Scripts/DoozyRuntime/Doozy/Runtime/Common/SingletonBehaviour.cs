using Doozy.Runtime.Common.Attributes;
using UnityEngine;

namespace Doozy.Runtime.Common;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	[ClearOnReload]
	private static T s_instance;

	[ClearOnReload(false)]
	protected static bool applicationIsQuitting { get; set; }

	public static T instance
	{
		get
		{
			if (applicationIsQuitting)
			{
				return null;
			}
			if (s_instance != null)
			{
				return s_instance;
			}
			s_instance = Object.FindObjectOfType<T>();
			if (s_instance != null)
			{
				return s_instance;
			}
			s_instance = new GameObject(typeof(T).Name).AddComponent<T>();
			return s_instance;
		}
	}

	protected virtual void OnApplicationQuit()
	{
		applicationIsQuitting = true;
	}

	protected virtual void OnDestroy()
	{
	}

	protected virtual void Awake()
	{
		if (s_instance != null && s_instance != this)
		{
			Debug.Log("There cannot be two '" + typeof(T).Name + "' active at the same time. Destroying the '" + base.gameObject.name + "' GameObject!");
			Object.Destroy(base.gameObject);
		}
		else
		{
			s_instance = GetComponent<T>();
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}
}
