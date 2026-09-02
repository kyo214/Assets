using UnityEngine;

namespace DestroyIt;

public class ResetDestructible : MonoBehaviour
{
	[Tooltip("The game object prefab you want to reset this destructible object to after it has been destroyed. (Usually a Pristine version of this destroyed object.)")]
	public GameObject resetToPrefab;

	[Tooltip("The minimum amount of time to wait (in seconds) before resetting the destructible object. (3600 seconds = 1 hour)")]
	public float minWaitSeconds = 30f;

	[Tooltip("The maximum amount of time to wait (in seconds) before resetting the destructible object. (3600 seconds = 1 hour)")]
	public float maxWaitSeconds = 45f;

	private float _timeLeft;

	private bool _isInitialized;

	private void Start()
	{
		if (resetToPrefab == null)
		{
			Debug.LogError("ResetDestructible Script: You need to assign a prefab to the [resetToPrefab] field.");
			Object.Destroy(this);
		}
		else
		{
			_timeLeft = ((maxWaitSeconds <= minWaitSeconds) ? 0f : Random.Range(minWaitSeconds, maxWaitSeconds));
			Debug.Log($"[{base.gameObject.name}] will be reset in approximately {Mathf.RoundToInt(_timeLeft)} seconds.");
			_isInitialized = true;
		}
	}

	private void Update()
	{
		if (_isInitialized)
		{
			_timeLeft -= Time.deltaTime;
			if (_timeLeft <= 0f)
			{
				Object.Instantiate(resetToPrefab, base.transform.position, base.transform.rotation, base.transform.parent).transform.localScale = base.transform.localScale;
				Debug.Log("[" + base.gameObject.name + "] has been reset to [" + resetToPrefab.name + "].");
				Object.Destroy(base.gameObject);
			}
		}
	}
}
