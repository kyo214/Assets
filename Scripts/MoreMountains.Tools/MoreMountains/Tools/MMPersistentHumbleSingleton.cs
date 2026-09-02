using UnityEngine;

namespace MoreMountains.Tools;

public class MMPersistentHumbleSingleton<T> : MonoBehaviour where T : Component
{
	protected static T _instance;

	[MMReadOnly]
	public float InitializationTime;

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
						hideFlags = HideFlags.HideAndDontSave,
						name = typeof(T).Name + "_AutoCreated"
					}.AddComponent<T>();
				}
			}
			return _instance;
		}
	}

	protected virtual void Awake()
	{
		InitializeSingleton();
	}

	protected virtual void InitializeSingleton()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		InitializationTime = Time.time;
		Object.DontDestroyOnLoad(base.gameObject);
		T[] array = Object.FindObjectsOfType<T>();
		foreach (T val in array)
		{
			if (val != this && val.GetComponent<MMPersistentHumbleSingleton<T>>().InitializationTime < InitializationTime)
			{
				Object.Destroy(val.gameObject);
			}
		}
		if (_instance == null)
		{
			_instance = this as T;
		}
	}
}
