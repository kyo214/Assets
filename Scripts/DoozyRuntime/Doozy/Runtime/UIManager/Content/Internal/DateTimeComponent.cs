using System;
using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.Common;
using Doozy.Runtime.Global;
using Doozy.Runtime.Mody;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Content.Internal;

public abstract class DateTimeComponent : MonoBehaviour
{
	protected const float k_MinimumUpdateInterval = 0.001f;

	public Timescale TimescaleMode;

	[Space(5f)]
	[SerializeField]
	protected float UpdateInterval;

	public TimerBehaviour OnStartBehaviour;

	public TimerBehaviour OnEnableBehaviour = TimerBehaviour.ResetAndStart;

	public TimerBehaviour OnDisableBehaviour = TimerBehaviour.Finish;

	public TimerBehaviour OnDestroyBehaviour = TimerBehaviour.Cancel;

	[SerializeField]
	private List<FormattedLabel> Labels;

	public ModyEvent OnStart = new ModyEvent();

	public ModyEvent OnStop = new ModyEvent();

	public ModyEvent OnFinish = new ModyEvent();

	public ModyEvent OnCancel = new ModyEvent();

	public ModyEvent OnPause = new ModyEvent();

	public ModyEvent OnResume = new ModyEvent();

	public ModyEvent OnReset = new ModyEvent();

	public ModyEvent OnUpdate = new ModyEvent();

	[SerializeField]
	protected int Years;

	[SerializeField]
	protected int Months;

	[SerializeField]
	protected int Days;

	[SerializeField]
	protected int Hours;

	[SerializeField]
	protected int Minutes;

	[SerializeField]
	protected int Seconds;

	[SerializeField]
	protected int Milliseconds;

	public float updateInterval
	{
		get
		{
			return UpdateInterval;
		}
		set
		{
			UpdateInterval = Mathf.Max(0.001f, value);
			waitRealtime = new WaitForSecondsRealtime(UpdateInterval);
			wait = new WaitForSeconds(UpdateInterval);
		}
	}

	public List<FormattedLabel> labels => Labels ?? (Labels = new List<FormattedLabel>());

	public UnityEvent onStartEvent => OnStart.Event;

	public UnityEvent onStopEvent => OnStop.Event;

	public UnityEvent onFinishEvent => OnFinish.Event;

	public UnityEvent onCancelEvent => OnFinish.Event;

	public UnityEvent onPauseEvent => OnPause.Event;

	public UnityEvent onResumeEvent => OnResume.Event;

	public UnityEvent onResetEvent => OnReset.Event;

	public UnityEvent onUpdateEvent => OnUpdate.Event;

	public bool hasCallbacks
	{
		get
		{
			if (!OnStart.hasCallbacks && !OnStop.hasCallbacks && !OnFinish.hasCallbacks && !OnCancel.hasCallbacks && !OnPause.hasCallbacks && !OnResume.hasCallbacks && !OnReset.hasCallbacks)
			{
				return OnUpdate.hasCallbacks;
			}
			return true;
		}
	}

	public int years => Years;

	public int months => Months;

	public int days => Days;

	public int hours => Hours;

	public int minutes => Minutes;

	public int seconds => Seconds;

	public int milliseconds => Milliseconds;

	public DateTime startTime { get; protected set; }

	public DateTime currentTime { get; protected set; }

	public DateTime endTime { get; protected set; }

	public TimeSpan elapsedTime { get; protected set; }

	public TimeSpan remainingTime { get; protected set; }

	public bool isRunning { get; protected set; }

	public bool isPaused { get; private set; }

	protected bool isFinished => remainingTime.TotalMilliseconds <= 0.0;

	protected double lastTime { get; set; }

	protected double lastUnscaledTime { get; set; }

	protected double lastDeltaTime => Time.timeAsDouble - lastTime;

	protected double lastUnscaledDeltaTime => Time.realtimeSinceStartupAsDouble - lastUnscaledTime;

	protected float previousUpdateInterval { get; set; }

	protected WaitForSecondsRealtime waitRealtime { get; set; }

	protected WaitForSeconds wait { get; set; }

	protected Coroutine updateCoroutine { get; set; }

	protected virtual void Awake()
	{
		startTime = DateTime.Now;
		currentTime = DateTime.Now;
		endTime = DateTime.Now;
		isRunning = false;
		isPaused = false;
		updateInterval = UpdateInterval;
	}

	protected void Start()
	{
		RunBehaviour(OnStartBehaviour);
	}

	protected virtual void OnEnable()
	{
		updateInterval = UpdateInterval;
		RunBehaviour(OnEnableBehaviour);
	}

	protected virtual void OnDisable()
	{
		switch (OnDisableBehaviour)
		{
		case TimerBehaviour.Disabled:
		case TimerBehaviour.Stop:
		case TimerBehaviour.StopAndReset:
		case TimerBehaviour.Pause:
		case TimerBehaviour.Reset:
		case TimerBehaviour.Finish:
		case TimerBehaviour.Cancel:
			RunBehaviour(OnDisableBehaviour);
			break;
		case TimerBehaviour.Start:
		case TimerBehaviour.ResetAndStart:
		case TimerBehaviour.Resume:
			Debug.LogWarning($"[{base.name}][{GetType().Name}] OnDisable Behaviour is set to '{OnDisableBehaviour}'. " + "This doesn't make sense. Doing nothing.");
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	protected virtual void OnDestroy()
	{
		switch (OnDisableBehaviour)
		{
		case TimerBehaviour.Disabled:
		case TimerBehaviour.Stop:
		case TimerBehaviour.Finish:
		case TimerBehaviour.Cancel:
			RunBehaviour(OnDisableBehaviour);
			break;
		case TimerBehaviour.Start:
		case TimerBehaviour.ResetAndStart:
		case TimerBehaviour.StopAndReset:
		case TimerBehaviour.Pause:
		case TimerBehaviour.Resume:
		case TimerBehaviour.Reset:
			Debug.LogWarning($"[{base.name}][{GetType().Name}] OnDisable Behaviour is set to '{OnDisableBehaviour}'. " + "This doesn't make sense. Doing nothing.");
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		RunBehaviour(OnDestroyBehaviour);
		StopUpdateCoroutine();
	}

	protected virtual void OnApplicationPause(bool pauseStatus)
	{
		RunBehaviour(pauseStatus ? TimerBehaviour.Pause : TimerBehaviour.Resume);
	}

	protected virtual IEnumerator TimeUpdateCoroutine()
	{
		if (waitRealtime == null)
		{
			waitRealtime = new WaitForSecondsRealtime(UpdateInterval);
		}
		if (wait == null)
		{
			wait = new WaitForSeconds(UpdateInterval);
		}
		previousUpdateInterval = UpdateInterval;
		while (isRunning)
		{
			if (isPaused)
			{
				yield return null;
				lastTime = Time.timeAsDouble;
				lastUnscaledTime = (float)Time.realtimeSinceStartupAsDouble;
				continue;
			}
			if (Math.Abs(previousUpdateInterval - UpdateInterval) > 0.001f)
			{
				waitRealtime = new WaitForSecondsRealtime(UpdateInterval);
				wait = new WaitForSeconds(UpdateInterval);
				previousUpdateInterval = UpdateInterval;
			}
			switch (TimescaleMode)
			{
			case Timescale.Independent:
				yield return waitRealtime;
				break;
			case Timescale.Dependent:
				yield return wait;
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			UpdateCurrentTime();
			OnUpdate.Execute();
			if (!(currentTime < endTime))
			{
				isRunning = false;
				OnFinish?.Execute();
			}
		}
	}

	protected virtual void SetStartTime()
	{
		startTime = DateTime.Now;
		UpdateLastTime();
	}

	protected virtual void SetEndTime()
	{
		endTime = startTime.AddYears(Years).AddMonths(Months).AddDays(Days)
			.AddHours(Hours)
			.AddMinutes(Minutes)
			.AddSeconds(Seconds)
			.AddMilliseconds(Milliseconds);
	}

	protected virtual void UpdateCurrentTime()
	{
		switch (TimescaleMode)
		{
		case Timescale.Independent:
			currentTime = currentTime.AddMilliseconds(lastUnscaledDeltaTime * 1000.0);
			break;
		case Timescale.Dependent:
			currentTime = currentTime.AddMilliseconds(lastDeltaTime * 1000.0);
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		elapsedTime = currentTime.Subtract(startTime);
		remainingTime = endTime.Subtract(currentTime);
		UpdateLastTime();
	}

	public virtual void UpdateLabels()
	{
		for (int i = 0; i < labels.Count; i++)
		{
			if (!(labels[i].Label == null))
			{
				labels[i].SetText(currentTime);
			}
		}
	}

	public virtual void ResetTimer()
	{
		updateInterval = UpdateInterval;
		StopUpdateCoroutine();
		isRunning = false;
		isPaused = false;
		OnReset?.Execute();
		SetStartTime();
		SetEndTime();
		currentTime = startTime;
		elapsedTime = TimeSpan.Zero;
		remainingTime = endTime - startTime;
	}

	public virtual void StartTimer()
	{
		if (isPaused)
		{
			ResumeTimer();
		}
		else if (!isRunning)
		{
			SetStartTime();
			SetEndTime();
			currentTime = startTime;
			elapsedTime = TimeSpan.Zero;
			remainingTime = endTime - startTime;
			OnStart?.Execute();
			isRunning = true;
			UpdateCurrentTime();
			if (base.isActiveAndEnabled)
			{
				StartUpdateCoroutine();
			}
		}
	}

	public virtual void StopTimer()
	{
		StopUpdateCoroutine();
		if (isRunning)
		{
			OnStop?.Execute();
			isRunning = false;
			isPaused = false;
		}
	}

	public virtual void PauseTimer()
	{
		if (isRunning && !isPaused)
		{
			OnPause?.Execute();
			isPaused = true;
		}
	}

	public virtual void ResumeTimer()
	{
		if (isRunning && isPaused)
		{
			OnResume?.Execute();
			isPaused = false;
		}
	}

	public virtual void FinishTimer()
	{
		StopUpdateCoroutine();
		currentTime = endTime;
		UpdateCurrentTime();
		StopTimer();
		OnFinish?.Execute();
	}

	public virtual void CancelTimer()
	{
		StopUpdateCoroutine();
		if (isRunning)
		{
			OnCancel?.Execute();
		}
		isRunning = false;
		isPaused = false;
	}

	protected virtual void RunBehaviour(TimerBehaviour behaviour)
	{
		switch (behaviour)
		{
		case TimerBehaviour.Start:
			StartTimer();
			break;
		case TimerBehaviour.Stop:
			StopTimer();
			break;
		case TimerBehaviour.ResetAndStart:
			ResetTimer();
			StartTimer();
			break;
		case TimerBehaviour.StopAndReset:
			StopTimer();
			ResetTimer();
			break;
		case TimerBehaviour.Pause:
			PauseTimer();
			break;
		case TimerBehaviour.Resume:
			ResumeTimer();
			break;
		case TimerBehaviour.Reset:
			ResetTimer();
			break;
		case TimerBehaviour.Finish:
			FinishTimer();
			break;
		case TimerBehaviour.Cancel:
			CancelTimer();
			break;
		default:
			throw new ArgumentOutOfRangeException("behaviour", behaviour, null);
		case TimerBehaviour.Disabled:
			break;
		}
	}

	protected void UpdateLastTime()
	{
		lastTime = Time.timeAsDouble;
		lastUnscaledTime = Time.realtimeSinceStartupAsDouble;
	}

	protected void StartUpdateCoroutine()
	{
		StopUpdateCoroutine();
		updateCoroutine = Coroutiner.Start(TimeUpdateCoroutine());
	}

	protected void StopUpdateCoroutine()
	{
		if (updateCoroutine != null)
		{
			Coroutiner.Stop(updateCoroutine);
		}
	}
}
