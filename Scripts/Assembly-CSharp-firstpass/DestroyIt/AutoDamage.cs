using UnityEngine;

namespace DestroyIt;

public class AutoDamage : MonoBehaviour
{
	public int startAtHitPoints = 30;

	public float damageIntervalSeconds = 0.5f;

	public int damagePerInterval = 5;

	private bool _isInitialized;

	private Destructible _destructible;

	private bool _autoDamageStarted;

	private void Start()
	{
		_destructible = base.gameObject.GetComponent<Destructible>();
		if (_destructible == null)
		{
			Debug.LogWarning("No Destructible object found! AutoDamage removed.");
			Object.Destroy(this);
		}
		_isInitialized = true;
	}

	private void Update()
	{
		if (_isInitialized && !(_destructible == null) && !_autoDamageStarted && _destructible.currentHitPoints <= (float)startAtHitPoints)
		{
			InvokeRepeating("ApplyDamage", 0f, damageIntervalSeconds);
			_autoDamageStarted = true;
		}
	}

	private void ApplyDamage()
	{
		if (!(_destructible == null))
		{
			_destructible.ApplyDamage(damagePerInterval);
		}
	}
}
