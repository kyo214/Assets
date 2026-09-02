using UnityEngine;

namespace DestroyIt;

public class EnableGravityAfter : MonoBehaviour
{
	public float seconds;

	private float _timeLeft;

	private bool _isInitialized;

	private void Start()
	{
		_timeLeft = seconds;
		_isInitialized = true;
	}

	private void OnEnable()
	{
		_timeLeft = seconds;
	}

	private void Update()
	{
		if (!_isInitialized)
		{
			return;
		}
		if (GetComponent<Rigidbody>() == null)
		{
			Object.Destroy(this);
			return;
		}
		_timeLeft -= Time.deltaTime;
		if (_timeLeft <= 0f)
		{
			GetComponent<Rigidbody>().useGravity = true;
			Object.Destroy(this);
		}
	}
}
