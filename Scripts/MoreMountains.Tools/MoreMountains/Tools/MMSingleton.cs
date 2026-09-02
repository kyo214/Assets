using UnityEngine;

namespace MoreMountains.Tools;

public class MMSingleton<T> : MonoBehaviour where T : Component
{
	protected static T _instance;

	public static bool HasInstance => _instance != null;

	public static T Current => _instance;

	public static T Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Object.FindObjectOfType<T>();
				if (_instance == null)
				{
					_instance = new GameObject
					{
						name = typeof(T).Name + "_AutoCreated"
					}.AddComponent<T>();
				}
			}
			return _instance;
		}
	}

	public static T TryGetInstance()
	{
		if (!HasInstance)
		{
			return null;
		}
		return _instance;
	}

	protected virtual void Awake()
	{
		InitializeSingleton();
	}

	protected virtual void InitializeSingleton()
	{
		if (Application.isPlaying)
		{
			_instance = this as T;
		}
	}
}
