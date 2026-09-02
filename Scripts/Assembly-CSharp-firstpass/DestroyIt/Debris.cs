using UnityEngine;

namespace DestroyIt;

public class Debris
{
	public Rigidbody Rigidbody { get; set; }

	public GameObject GameObject { get; set; }

	public bool IsActive
	{
		get
		{
			if (Rigidbody != null)
			{
				return GameObject.activeSelf;
			}
			return false;
		}
	}

	public void Disable()
	{
		if (Rigidbody != null)
		{
			GameObject.SetActive(value: false);
		}
	}
}
