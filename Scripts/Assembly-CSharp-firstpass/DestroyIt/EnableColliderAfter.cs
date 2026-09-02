using UnityEngine;

namespace DestroyIt;

public class EnableColliderAfter : MonoBehaviour
{
	public float seconds;

	private float timeLeft;

	private bool isInitialized;

	private void Start()
	{
		timeLeft = seconds;
		isInitialized = true;
	}

	private void OnEnable()
	{
		timeLeft = seconds;
	}

	private void Update()
	{
		if (!isInitialized)
		{
			return;
		}
		timeLeft -= Time.deltaTime;
		if (timeLeft <= 0f)
		{
			Collider[] components = base.gameObject.GetComponents<Collider>();
			for (int i = 0; i < components.Length; i++)
			{
				components[i].enabled = true;
			}
			Object.Destroy(this);
		}
	}
}
