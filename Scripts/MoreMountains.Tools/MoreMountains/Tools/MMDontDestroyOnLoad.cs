using UnityEngine;

namespace MoreMountains.Tools;

public class MMDontDestroyOnLoad : MonoBehaviour
{
	protected void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}
}
