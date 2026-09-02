using UnityEngine;

namespace DestroyIt;

public class PoolAfter : MonoBehaviour
{
	public float seconds;

	public bool reenableChildren;

	public bool removeWhenPooled;

	public bool resetToPrefab;

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
		_timeLeft -= Time.deltaTime;
		if (!(_timeLeft <= 0f))
		{
			return;
		}
		if (resetToPrefab)
		{
			GameObject gameObject = ObjectPool.Instance.SpawnFromOriginal(base.gameObject.name);
			if (gameObject != null)
			{
				ObjectPool.Instance.PoolObject(gameObject);
			}
			Object.Destroy(base.gameObject);
			_isInitialized = false;
		}
		else
		{
			if (removeWhenPooled)
			{
				Object.Destroy(this);
			}
			ObjectPool.Instance.PoolObject(base.gameObject, reenableChildren);
		}
	}
}
