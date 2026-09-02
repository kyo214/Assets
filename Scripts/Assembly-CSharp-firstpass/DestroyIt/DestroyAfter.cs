using UnityEngine;

namespace DestroyIt;

public class DestroyAfter : MonoBehaviour
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
		if (isInitialized)
		{
			timeLeft -= Time.deltaTime;
			if (timeLeft <= 0f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}
}
