using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Common.Extensions;
using Doozy.Runtime.Reactor.Easings;
using Doozy.Runtime.Reactor.Ticker;
using UnityEngine;

namespace Doozy.Runtime.Reactor.Internal;

[Serializable]
public abstract class Reaction
{
	[SerializeField]
	private ReactionSettings Settings;

	private float m_LastProgress;

	private const float MIN_DURATION = 0.001f;

	public ReactionCallback OnPlayCallback;

	public ReactionCallback OnStopCallback;

	public ReactionCallback OnFinishCallback;

	public ReactionCallback OnLoopCallback;

	public ReactionCallback OnPauseCallback;

	public ReactionCallback OnResumeCallback;

	public ReactionCallback OnUpdateCallback;

	public const int k_DefaultIntId = -1234;

	[ClearOnReload(true)]
	internal static readonly ReactionDictionary<object> ReactionByObjectId = new ReactionDictionary<object>();

	[ClearOnReload(true)]
	internal static readonly ReactionDictionary<string> ReactionByStringId = new ReactionDictionary<string>();

	[ClearOnReload(true)]
	internal static readonly ReactionDictionary<int> ReactionByIntId = new ReactionDictionary<int>();

	[ClearOnReload(true)]
	internal static readonly ReactionDictionary<object> ReactionByTargetObject = new ReactionDictionary<object>();

	public ReactionState state { get; internal set; }

	public ReactionState stateBeforePause { get; internal set; }

	public bool isPooled => state == ReactionState.Pooled;

	public bool isIdle => state == ReactionState.Idle;

	public bool isActive => !isPooled & !isIdle;

	public bool isPaused => state == ReactionState.Paused;

	public bool isPlaying => state == ReactionState.Playing;

	public bool inStartDelay => state == ReactionState.StartDelay;

	public bool inLoopDelay => state == ReactionState.LoopDelay;

	public ReactionSettings settings
	{
		get
		{
			return Settings;
		}
		internal set
		{
			Settings = value;
		}
	}

	public float progress => m_LastProgress = Mathf.Clamp01((float)(elapsedDuration / (double)duration));

	public float easedProgress => Settings.CalculateEasedProgress(progress);

	public PlayDirection direction { get; internal set; }

	public Heartbeat heartbeat { get; private set; }

	public float startDelay { get; internal set; }

	public double elapsedStartDelay { get; private set; }

	public float duration { get; internal set; }

	public double elapsedDuration { get; protected set; }

	protected float startDuration { get; set; }

	protected float targetDuration { get; set; }

	protected bool customStartDuration { get; set; }

	public int loops { get; internal set; }

	public int elapsedLoops { get; private set; }

	public float loopDelay { get; internal set; }

	public double elapsedLoopDelay { get; private set; }

	protected float currentCycleEasedProgress => Settings.CalculateEasedProgress(currentCycleProgress);

	protected List<float> cycleDurations { get; set; }

	protected int numberOfCycles { get; set; }

	protected int previousCycleIndex { get; set; }

	protected int currentCycleIndex { get; set; }

	protected float currentCycleDuration
	{
		get
		{
			if (cycleDurations == null || currentCycleIndex != cycleDurations.Count)
			{
				ComputePlayMode();
			}
			return cycleDurations[currentCycleIndex];
		}
	}

	protected float currentCycleElapsedDuration
	{
		get
		{
			if (currentCycleIndex == 0)
			{
				return (float)elapsedDuration;
			}
			float num = cycleDurations.TakeWhile((float t, int i) => currentCycleIndex != i).Sum();
			return Mathf.Clamp((float)(elapsedDuration - (double)num), 0f, targetDuration);
		}
	}

	protected float currentCycleProgress
	{
		get
		{
			float num = Mathf.Clamp01(currentCycleElapsedDuration / currentCycleDuration);
			if (!Mathf.Approximately(0f, num))
			{
				if (!Mathf.Approximately(num, 1f))
				{
					return num;
				}
				return 1f;
			}
			return 0f;
		}
	}

	public bool hasObjectId { get; internal set; }

	public object objectId { get; internal set; }

	public string stringId { get; internal set; }

	public bool hasStringId { get; internal set; }

	public int intId { get; internal set; }

	public bool hasIntId { get; internal set; }

	public object targetObject { get; internal set; }

	public bool hasTargetObject { get; internal set; }

	public void ResetCallbacks()
	{
		OnUpdateCallback = null;
		OnPlayCallback = null;
		OnStopCallback = null;
		OnFinishCallback = null;
		OnLoopCallback = null;
		OnPauseCallback = null;
		OnResumeCallback = null;
	}

	protected Reaction()
	{
		cycleDurations = new List<float>(100);
		Settings = new ReactionSettings();
		this.SetRuntimeHeartbeat();
	}

	public virtual void Reset()
	{
		if (isActive)
		{
			Stop(silent: true);
		}
		ClearIds();
		this.ClearCallbacks();
		if (Settings == null)
		{
			Settings = new ReactionSettings();
		}
		Settings.Reset();
	}

	private void ClearIds()
	{
		objectId = null;
		stringId = null;
		intId = -1234;
		targetObject = null;
	}

	public void Reverse()
	{
		if (isActive)
		{
			if (inStartDelay)
			{
				Stop();
			}
			else
			{
				direction = (PlayDirection)((float)direction * -1f);
			}
		}
	}

	public void Rewind()
	{
		elapsedDuration = ((direction == PlayDirection.Forward) ? 0f : targetDuration);
	}

	public void Pause(bool silent = false)
	{
		if (isActive)
		{
			stateBeforePause = state;
			state = ReactionState.Paused;
			if (!silent)
			{
				OnPauseCallback?.Invoke();
			}
		}
	}

	public void Resume(bool silent = false)
	{
		if (isPaused)
		{
			state = stateBeforePause;
			if (isActive & !heartbeat.isActive)
			{
				heartbeat.RegisterToTickService();
			}
			if (!silent)
			{
				OnResumeCallback?.Invoke();
			}
		}
	}

	public void Play(PlayDirection playDirection)
	{
		Play(playDirection == PlayDirection.Reverse);
	}

	public virtual void Play(bool inReverse = false)
	{
		if (isActive)
		{
			switch (direction)
			{
			case PlayDirection.Forward:
				if (inReverse)
				{
					if (inStartDelay)
					{
						Stop();
					}
					else
					{
						Reverse();
					}
					return;
				}
				break;
			case PlayDirection.Reverse:
				if (!inReverse)
				{
					if (inStartDelay)
					{
						Stop();
					}
					else
					{
						Reverse();
					}
					return;
				}
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
		if (isActive)
		{
			Stop(silent: true);
		}
		ResetElapsedValues();
		RefreshSettings();
		direction = ((!inReverse) ? PlayDirection.Forward : PlayDirection.Reverse);
		customStartDuration = false;
		startDuration = 0f;
		targetDuration = duration;
		elapsedDuration = ((direction == PlayDirection.Forward) ? startDuration : targetDuration);
		m_LastProgress = progress;
		ComputePlayMode();
		OnPlayCallback?.Invoke();
		if ((startDelay <= 0f) & (duration <= 0.001f))
		{
			switch (direction)
			{
			case PlayDirection.Forward:
				SetProgressAtOne();
				break;
			case PlayDirection.Reverse:
				SetProgressAtZero();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			OnStopCallback?.Invoke();
			OnFinishCallback?.Invoke();
		}
		state = (((startDelay > 0f) & (direction == PlayDirection.Forward)) ? ReactionState.StartDelay : ReactionState.Playing);
		heartbeat.RegisterToTickService();
	}

	public virtual void PlayFromToProgress(float fromProgress, float toProgress)
	{
		fromProgress = GetAdjustedProgress(fromProgress, settings.playMode);
		toProgress = GetAdjustedProgress(toProgress, settings.playMode);
		if (isActive)
		{
			Stop(silent: true);
		}
		ResetElapsedValues();
		RefreshSettings();
		direction = ((fromProgress <= toProgress) ? PlayDirection.Forward : PlayDirection.Reverse);
		customStartDuration = true;
		float durationAtProgress = GetDurationAtProgress(fromProgress, duration);
		float durationAtProgress2 = GetDurationAtProgress(toProgress, duration);
		startDuration = ((direction == PlayDirection.Forward) ? durationAtProgress : durationAtProgress2);
		targetDuration = ((direction == PlayDirection.Forward) ? durationAtProgress2 : durationAtProgress);
		elapsedDuration = ((direction == PlayDirection.Forward) ? startDuration : targetDuration);
		m_LastProgress = progress;
		ComputePlayMode();
		OnPlayCallback?.Invoke();
		if (duration <= 0.001f)
		{
			switch (direction)
			{
			case PlayDirection.Forward:
				SetProgressAtOne();
				break;
			case PlayDirection.Reverse:
				SetProgressAtZero();
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			OnStopCallback?.Invoke();
			OnFinishCallback?.Invoke();
		}
		state = ReactionState.Playing;
		heartbeat.RegisterToTickService();
	}

	public virtual void PlayToProgress(float toProgress)
	{
		PlayFromToProgress(m_LastProgress, toProgress);
	}

	public virtual void PlayFromProgress(float fromProgress)
	{
		PlayFromToProgress(fromProgress, m_LastProgress);
	}

	protected float GetDurationAtProgress(float targetProgress, float totalDuration)
	{
		targetProgress = Mathf.Clamp01(targetProgress);
		totalDuration = Mathf.Max(0f, totalDuration);
		totalDuration = ((totalDuration == 0f) ? 1f : totalDuration);
		return Mathf.Clamp(totalDuration * targetProgress, 0f, totalDuration).Round(4);
	}

	private static float GetAdjustedProgress(float progress, PlayMode playMode)
	{
		progress = progress.Clamp01();
		switch (playMode)
		{
		case PlayMode.Normal:
			return progress;
		case PlayMode.PingPong:
			if (progress == 0f)
			{
				return 0f;
			}
			if (progress.Approximately(0.5f))
			{
				return 1f;
			}
			if (progress.Approximately(1f))
			{
				return 0f;
			}
			if (progress < 0.5f)
			{
				return progress * 2f;
			}
			if (progress > 0.5f)
			{
				return (1f - progress) * 2f;
			}
			return progress;
		case PlayMode.Spring:
			return progress;
		case PlayMode.Shake:
			return progress;
		default:
			throw new ArgumentOutOfRangeException("playMode", playMode, null);
		}
	}

	public virtual void SetProgressAt(float targetProgress)
	{
		targetProgress = GetAdjustedProgress(targetProgress, settings.playMode);
		if (isActive)
		{
			Stop(silent: true);
		}
		ResetElapsedValues();
		RefreshSettings();
		direction = PlayDirection.Forward;
		EaseMode easeMode = settings.easeMode;
		Ease ease = settings.ease;
		AnimationCurve curve = settings.curve;
		settings.easeMode = EaseMode.Ease;
		settings.ease = Ease.Linear;
		elapsedDuration = Mathf.Clamp01(targetProgress) * duration;
		m_LastProgress = progress;
		if (heartbeat.isActive)
		{
			heartbeat.UnregisterFromTickService();
		}
		if (settings.playMode != PlayMode.Normal)
		{
			ComputePlayMode();
		}
		UpdateCurrentCycleIndex();
		UpdateCurrentValue();
		OnUpdateCallback?.Invoke();
		settings.ease = ease;
		settings.curve = curve;
		settings.easeMode = easeMode;
	}

	public void SetProgressAtOne()
	{
		SetProgressAt(1f);
	}

	public void SetProgressAtZero()
	{
		SetProgressAt(0f);
	}

	internal void UpdateReaction()
	{
		if (isPooled)
		{
			if (heartbeat.isActive)
			{
				heartbeat.UnregisterFromTickService();
			}
			return;
		}
		if (isIdle & heartbeat.isActive)
		{
			heartbeat.UnregisterFromTickService();
		}
		if (IsPaused() || InStartDelay() || InLoopDelay())
		{
			return;
		}
		elapsedDuration = ((elapsedDuration < 0.0) ? 0.0 : elapsedDuration);
		elapsedDuration = Mathf.Clamp((float)elapsedDuration, startDuration, targetDuration);
		elapsedDuration = ((elapsedDuration > (double)duration) ? ((double)duration) : elapsedDuration);
		m_LastProgress = progress;
		UpdateCurrentCycleIndex();
		UpdateCurrentValue();
		OnUpdateCallback?.Invoke();
		switch (direction)
		{
		case PlayDirection.Forward:
			if (elapsedDuration < (double)targetDuration)
			{
				elapsedDuration += heartbeat.deltaTime * (double)direction;
				return;
			}
			break;
		case PlayDirection.Reverse:
			if (elapsedDuration > (double)startDuration)
			{
				elapsedDuration += heartbeat.deltaTime * (double)direction;
				return;
			}
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
		elapsedLoops++;
		if (loops < 0 || (loops != 0 && elapsedLoops <= loops))
		{
			if (!customStartDuration)
			{
				duration = Mathf.Max(0.001f, Settings.GetDuration());
				startDuration = 0f;
				targetDuration = duration;
				ComputePlayMode();
			}
			elapsedDuration = ((direction == PlayDirection.Forward) ? startDuration : targetDuration);
			m_LastProgress = progress;
			loopDelay = Settings.GetLoopDelay();
			if (loopDelay > 0f)
			{
				state = ReactionState.LoopDelay;
				return;
			}
			OnLoopCallback?.Invoke();
			state = ReactionState.Playing;
		}
		else
		{
			elapsedDuration = ((direction == PlayDirection.Forward) ? targetDuration : startDuration);
			elapsedDuration = elapsedDuration.Round(4);
			m_LastProgress = progress;
			UpdateCurrentCycleIndex();
			UpdateCurrentValue();
			OnUpdateCallback?.Invoke();
			Finish();
		}
	}

	private bool IsPaused()
	{
		if (!isPaused)
		{
			return false;
		}
		heartbeat.lastUpdateTime = heartbeat.timeSinceStartup;
		return true;
	}

	private bool InStartDelay()
	{
		if (!inStartDelay)
		{
			return false;
		}
		elapsedStartDelay += heartbeat.deltaTime;
		elapsedStartDelay = Mathf.Clamp((float)elapsedStartDelay, 0f, startDelay);
		if ((double)startDelay - elapsedStartDelay > 0.0)
		{
			return true;
		}
		state = ReactionState.Playing;
		elapsedStartDelay = 0.0;
		return false;
	}

	private bool InLoopDelay()
	{
		if (!inLoopDelay)
		{
			return false;
		}
		elapsedLoopDelay += heartbeat.deltaTime;
		elapsedLoopDelay = Mathf.Clamp((float)elapsedLoopDelay, 0f, loopDelay);
		if ((double)loopDelay - elapsedLoopDelay > 0.0)
		{
			return true;
		}
		OnLoopCallback?.Invoke();
		state = ReactionState.Playing;
		elapsedLoopDelay = 0.0;
		return false;
	}

	public abstract void UpdateCurrentValue();

	public virtual void Stop(bool silent = false, bool recycle = false)
	{
		if (heartbeat.isActive)
		{
			heartbeat.UnregisterFromTickService();
		}
		if (!isPooled)
		{
			if (!silent)
			{
				OnStopCallback?.Invoke();
			}
			state = ReactionState.Idle;
			if (recycle)
			{
				Recycle();
			}
		}
	}

	public virtual void Finish(bool silent = false, bool endAnimation = false, bool recycle = false)
	{
		if (isActive)
		{
			Stop(silent);
			if (!silent)
			{
				OnFinishCallback?.Invoke();
			}
			if (endAnimation)
			{
				SetProgressAtOne();
			}
			if (recycle)
			{
				Recycle();
			}
		}
	}

	public void SetHeartbeat(Heartbeat h)
	{
		heartbeat = h ?? new RuntimeHeartbeat();
		heartbeat.AddOnTickCallback(UpdateReaction);
	}

	private void ResetElapsedValues()
	{
		elapsedStartDelay = 0.0;
		elapsedDuration = 0.0;
		elapsedLoops = 0;
		elapsedLoopDelay = 0.0;
	}

	public void RefreshSettings()
	{
		settings.Validate();
		startDelay = Settings.GetStartDelay();
		duration = Settings.GetDuration();
		duration = ((float.IsNaN(duration) || float.IsInfinity(duration)) ? 0f : duration);
		duration = Mathf.Max(0.001f, duration);
		loops = Settings.GetLoops();
		loopDelay = Settings.GetLoopDelay();
		ComputePlayMode();
	}

	public void ComputePlayMode()
	{
		switch (Settings.playMode)
		{
		case PlayMode.Normal:
			ComputeNormal();
			break;
		case PlayMode.PingPong:
			ComputePingPong();
			break;
		case PlayMode.Spring:
			ComputeSpring();
			break;
		case PlayMode.Shake:
			ComputeShake();
			break;
		default:
			throw new ArgumentOutOfRangeException();
		}
	}

	private void UpdateCurrentCycleIndex()
	{
		previousCycleIndex = currentCycleIndex;
		switch (direction)
		{
		case PlayDirection.Forward:
		{
			float num3 = 0f;
			for (int i = 0; i < cycleDurations.Count; i++)
			{
				currentCycleIndex = i;
				num3 += cycleDurations[i];
				if (elapsedDuration <= (double)num3)
				{
					break;
				}
			}
			break;
		}
		case PlayDirection.Reverse:
		{
			float num = duration;
			for (int num2 = cycleDurations.Count - 1; num2 >= 0; num2--)
			{
				currentCycleIndex = num2;
				num -= cycleDurations[num2];
				if (elapsedDuration > (double)num)
				{
					break;
				}
			}
			break;
		}
		}
	}

	private void EnsureCycleDurationsListCapacity(int requiredCapacity)
	{
		if (cycleDurations == null)
		{
			cycleDurations = new List<float>(requiredCapacity);
		}
		else if (requiredCapacity > cycleDurations.Capacity)
		{
			cycleDurations.Capacity = requiredCapacity;
		}
	}

	protected virtual void ComputeNormal()
	{
		currentCycleIndex = 0;
		numberOfCycles = 1;
		EnsureCycleDurationsListCapacity(numberOfCycles);
		if (cycleDurations.Count != numberOfCycles)
		{
			cycleDurations.Clear();
			cycleDurations.Add(duration);
		}
		else
		{
			cycleDurations[0] = duration;
		}
	}

	protected virtual void ComputePingPong()
	{
		currentCycleIndex = 0;
		numberOfCycles = 2;
		float num = duration / 2f;
		EnsureCycleDurationsListCapacity(numberOfCycles);
		if (cycleDurations.Count != numberOfCycles)
		{
			cycleDurations.Clear();
			cycleDurations.Add(num);
			cycleDurations.Add(num);
		}
		else
		{
			cycleDurations[0] = num;
			cycleDurations[1] = num;
		}
	}

	protected virtual void ComputeSpring()
	{
		currentCycleIndex = 0;
		numberOfCycles = Mathf.Max(1, settings.vibration + (int)((float)settings.vibration * duration));
		if (numberOfCycles % 2 != 0)
		{
			numberOfCycles++;
		}
		EnsureCycleDurationsListCapacity(numberOfCycles);
		if (cycleDurations.Count != numberOfCycles)
		{
			cycleDurations.Clear();
			for (int i = 0; i < numberOfCycles; i++)
			{
				cycleDurations.Add(0f);
			}
		}
		float num = 0f;
		for (int j = 0; j < numberOfCycles; j++)
		{
			cycleDurations[j] = duration * ((float)(j + 1) / (float)numberOfCycles);
			cycleDurations[j] = cycleDurations[j].Round(4);
			num += cycleDurations[j];
		}
		float num2 = duration / num;
		for (int k = 0; k < numberOfCycles; k++)
		{
			cycleDurations[k] *= num2;
		}
	}

	protected virtual void ComputeShake()
	{
		currentCycleIndex = 0;
		numberOfCycles = Mathf.Max(1, settings.vibration + (int)((float)settings.vibration * duration));
		if (numberOfCycles % 2 == 0)
		{
			numberOfCycles++;
		}
		EnsureCycleDurationsListCapacity(numberOfCycles);
		if (cycleDurations.Count != numberOfCycles)
		{
			cycleDurations.Clear();
			for (int i = 0; i < numberOfCycles; i++)
			{
				cycleDurations.Add(0f);
			}
		}
		float num = 0f;
		for (int j = 0; j < numberOfCycles; j++)
		{
			if (settings.fadeOutShake)
			{
				float num2 = (float)(j + 1) / (float)numberOfCycles;
				cycleDurations[j] = EaseFactory.GetEase(Ease.OutExpo).Evaluate(num2) * duration;
			}
			else
			{
				cycleDurations[j] = duration / (float)numberOfCycles;
			}
			num += cycleDurations[j];
		}
		float num3 = duration / num;
		for (int k = 0; k < numberOfCycles; k++)
		{
			cycleDurations[k] *= num3;
		}
		float num4 = 0f;
		for (int l = 0; l < numberOfCycles - 1; l++)
		{
			num4 += cycleDurations[l];
		}
		cycleDurations[numberOfCycles - 1] = duration - num4;
	}

	public void Recycle()
	{
		this.AddToPool();
	}

	public static T Get<T>() where T : Reaction
	{
		return ReactionPool.Get<T>();
	}

	public static void StopAllReactionsByObjectId(object id, bool silent = false)
	{
		foreach (Reaction reaction in ReactionByObjectId.GetReactions(id))
		{
			reaction.Stop(silent);
		}
	}

	public static void StopAllReactionsByStringId(string id, bool silent = false)
	{
		foreach (Reaction reaction in ReactionByStringId.GetReactions(id))
		{
			reaction.Stop(silent);
		}
	}

	public static void StopAllReactionsByIntId(int id, bool silent = false)
	{
		foreach (Reaction reaction in ReactionByIntId.GetReactions(id))
		{
			reaction.Stop(silent);
		}
	}

	public static void StopAllReactionsByTargetObject(object target, bool silent = false)
	{
		foreach (Reaction reaction in ReactionByTargetObject.GetReactions(target))
		{
			reaction.Stop(silent);
		}
	}

	public override string ToString()
	{
		return "[" + ((heartbeat != null) ? heartbeat.GetType().Name : "No Heartbeat") + "] [" + GetType().Name + "] " + $"[{state}] > " + $"[{direction}] > " + $"[{elapsedDuration.Round(3):0.000} / {duration} seconds] " + string.Format("[{0}: {1:0.00} {2:000}%]", "progress", progress.Round(2), (progress.Round(2) * 100f).Round(0));
	}
}
