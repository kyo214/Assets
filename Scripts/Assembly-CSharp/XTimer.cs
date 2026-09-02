using UnityEngine;

public class XTimer : MonoBehaviour
{
	public float interval;

	public float initInterval;

	public bool isRunning;

	public bool initialized;

	public bool ended;

	public bool isPaused;

	public bool isCountdown;

	public bool debugMode;

	[SerializeField]
	private bool isUnscaledDeltaTime;

	private void Awake()
	{
		base.enabled = false;
		if ((bool)NetworkGameManager.Instance && !LobbyManager.Instance && NetworkGameManager.Instance.arrPlayerController.Count == 1)
		{
			isUnscaledDeltaTime = false;
		}
	}

	public void StartDuration(float newInterval)
	{
		base.enabled = true;
		isRunning = true;
		initInterval = newInterval;
		interval = newInterval;
		initialized = true;
		ended = false;
		isCountdown = true;
		isPaused = false;
	}

	public void StartTimer()
	{
		base.enabled = true;
		isCountdown = false;
		isRunning = true;
		initialized = true;
		ended = false;
		interval = 0f;
		isPaused = false;
	}

	public void StopDuration()
	{
		isRunning = false;
		interval = 0f;
		isPaused = false;
		if (debugMode)
		{
			Debug.Log("stopDuration");
		}
		base.enabled = false;
	}

	public void CancelDuration()
	{
		isRunning = false;
		interval = 0f;
		isPaused = false;
		ended = false;
		base.enabled = false;
	}

	public void PauseDuration()
	{
		isRunning = false;
		isPaused = true;
		if (debugMode)
		{
			Debug.Log("pauseDuration = " + interval);
		}
		base.enabled = false;
	}

	public void ResumeDuration()
	{
		base.enabled = true;
		if (interval > 0f)
		{
			isRunning = true;
		}
		isPaused = false;
		if (debugMode)
		{
			Debug.Log("resumeDuration = " + interval);
		}
	}

	public bool isCompleted()
	{
		bool result = false;
		if (ended && !isRunning)
		{
			ended = false;
			result = true;
			isPaused = false;
			if (debugMode)
			{
				Debug.Log("completeDuration");
			}
			base.enabled = false;
		}
		return result;
	}

	private void FixedUpdate()
	{
		if (!isRunning || !initialized)
		{
			return;
		}
		float num = Time.fixedDeltaTime;
		if (isUnscaledDeltaTime)
		{
			num = Time.fixedUnscaledDeltaTime;
		}
		if (isCountdown)
		{
			interval -= num;
			if (interval <= 0f)
			{
				isRunning = false;
				ended = true;
				isPaused = false;
			}
		}
		else
		{
			interval += num;
		}
	}
}
