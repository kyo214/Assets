using UnityEngine;

public class GenericSingleton<T> : MonoBehaviour where T : Component
{
	private static T instance;

	public bool singeltonDestroyOnLoad;

	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = Object.FindObjectOfType<T>();
				if (instance == null)
				{
					instance = new GameObject
					{
						name = typeof(T).Name
					}.AddComponent<T>();
				}
			}
			return instance;
		}
	}

	public static T InstanceNoCallback => instance;

	public virtual void Awake()
	{
		if (instance == null)
		{
			instance = this as T;
			if (!singeltonDestroyOnLoad && base.transform.parent == null)
			{
				Object.DontDestroyOnLoad(base.gameObject);
			}
		}
		else
		{
			Debug.Log("destroyed : " + instance.GetType());
			Object.Destroy(base.gameObject);
		}
	}
}
