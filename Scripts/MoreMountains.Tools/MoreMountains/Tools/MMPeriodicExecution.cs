using UnityEngine;
using UnityEngine.Events;

namespace MoreMountains.Tools;

public class MMPeriodicExecution : MonoBehaviour
{
	[MMVector(new string[] { "Min", "Max" })]
	public Vector2 RandomIntervalDuration = new Vector2(1f, 3f);

	public UnityEvent OnRandomInterval;

	protected float _lastUpdateAt;

	protected float _currentInterval;

	protected virtual void Start()
	{
		DetermineNewInterval();
	}

	protected virtual void Update()
	{
		if (Time.time - _lastUpdateAt > _currentInterval)
		{
			OnRandomInterval?.Invoke();
			_lastUpdateAt = Time.time;
			DetermineNewInterval();
		}
	}

	protected virtual void DetermineNewInterval()
	{
		_currentInterval = Random.Range(RandomIntervalDuration.x, RandomIntervalDuration.y);
	}
}
