using UnityEngine;

namespace MoreMountains.Tools;

public class MMPersistentSingleton<T> : MonoBehaviour where T : Component
{
	[Header("Persistent Singleton")]
	[Tooltip("if this is true, this singleton will auto detach if it finds itself parented on awake")]
	public bool AutomaticallyUnparentOnAwake = true;

	protected static T _instance;

	protected bool _enabled;

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

	protected virtual void Awake()
	{
		InitializeSingleton();
	}

	protected virtual void InitializeSingleton()
	{
		if (Application.isPlaying)
		{
			if (AutomaticallyUnparentOnAwake)
			{
				base.transform.SetParent(null);
			}
			if (_instance == null)
			{
				_instance = this as T;
				Object.DontDestroyOnLoad(base.transform.gameObject);
				_enabled = true;
			}
			else if (this != _instance)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
