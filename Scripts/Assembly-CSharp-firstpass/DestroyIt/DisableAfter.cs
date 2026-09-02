using UnityEngine;

namespace DestroyIt;

public class DisableAfter : MonoBehaviour
{
	public float seconds;

	public bool removeScript;

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
			base.gameObject.SetActive(value: false);
			if (removeScript)
			{
				Object.Destroy(this);
			}
		}
	}
}
