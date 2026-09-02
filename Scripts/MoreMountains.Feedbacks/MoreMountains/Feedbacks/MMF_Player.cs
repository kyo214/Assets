using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("More Mountains/Feedbacks/MMF Player")]
[DisallowMultipleComponent]
public class MMF_Player : MMFeedbacks
{
	[SerializeReference]
	public List<MMF_Feedback> FeedbacksList;

	public bool KeepPlayModeChanges;

	[Tooltip("if this is true, the inspector won't refresh while the feedback plays, this saves on performance but feedback inspectors' progress bars for example won't look as smooth")]
	public bool PerformanceMode;

	[Tooltip("if this is true, StopFeedbacks will be called on all feedbacks on Disable")]
	public bool ForceStopFeedbacksOnDisable = true;

	protected Type _t;

	public override float TotalDuration
	{
		get
		{
			float num = 0f;
			if (FeedbacksList == null)
			{
				return InitialDelay;
			}
			foreach (MMF_Feedback feedbacks in FeedbacksList)
			{
				if (feedbacks != null && feedbacks.Active && num < feedbacks.TotalDuration)
				{
					num = feedbacks.TotalDuration;
				}
			}
			return InitialDelay + num;
		}
	}

	public bool SkippingToTheEnd { get; protected set; }

	protected override void Awake()
	{
		if (AutoPlayOnEnable)
		{
			MMF_PlayerEnabler mMF_PlayerEnabler = GetComponent<MMF_PlayerEnabler>();
			if (mMF_PlayerEnabler == null)
			{
				mMF_PlayerEnabler = base.gameObject.AddComponent<MMF_PlayerEnabler>();
			}
			mMF_PlayerEnabler.TargetMmfPlayer = this;
		}
		if (InitializationMode == InitializationModes.Awake && Application.isPlaying)
		{
			Initialization();
		}
		CheckForLoops();
	}

	protected override void Start()
	{
		if (InitializationMode == InitializationModes.Start && Application.isPlaying)
		{
			Initialization();
		}
		if (AutoPlayOnStart && Application.isPlaying)
		{
			PlayFeedbacks();
		}
		CheckForLoops();
	}

	protected virtual void InitializeList()
	{
		if (FeedbacksList == null)
		{
			FeedbacksList = new List<MMF_Feedback>();
		}
	}

	protected override void OnEnable()
	{
		if (AutoPlayOnEnable && Application.isPlaying)
		{
			PlayFeedbacks();
		}
		foreach (MMF_Feedback feedbacks in FeedbacksList)
		{
			feedbacks.CacheRequiresSetup();
		}
	}

	public override void Initialization()
	{
		SkippingToTheEnd = false;
		base.IsPlaying = false;
		_lastStartAt = float.MinValue;
		int count = FeedbacksList.Count;
		for (int i = 0; i < count; i++)
		{
			if (FeedbacksList[i] != null)
			{
				FeedbacksList[i].Initialization(this);
			}
		}
	}

	public override void PlayFeedbacks()
	{
		PlayFeedbacksInternal(base.transform.position, FeedbacksIntensity);
	}

	public override void PlayFeedbacks(Vector3 position, float feedbacksIntensity = 1f, bool forceRevert = false)
	{
		PlayFeedbacksInternal(position, feedbacksIntensity, forceRevert);
	}

	public override void PlayFeedbacksInReverse()
	{
		PlayFeedbacksInternal(base.transform.position, FeedbacksIntensity, forceRevert: true);
	}

	public override void PlayFeedbacksInReverse(Vector3 position, float feedbacksIntensity = 1f, bool forceRevert = false)
	{
		PlayFeedbacksInternal(position, feedbacksIntensity, forceRevert);
	}

	public override void PlayFeedbacksOnlyIfReversed()
	{
		if ((Direction == Directions.BottomToTop && !base.ShouldRevertOnNextPlay) || (Direction == Directions.TopToBottom && base.ShouldRevertOnNextPlay))
		{
			PlayFeedbacks();
		}
	}

	public override void PlayFeedbacksOnlyIfReversed(Vector3 position, float feedbacksIntensity = 1f, bool forceRevert = false)
	{
		if ((Direction == Directions.BottomToTop && !base.ShouldRevertOnNextPlay) || (Direction == Directions.TopToBottom && base.ShouldRevertOnNextPlay))
		{
			PlayFeedbacks(position, feedbacksIntensity, forceRevert);
		}
	}

	public override void PlayFeedbacksOnlyIfNormalDirection()
	{
		if (Direction == Directions.TopToBottom)
		{
			PlayFeedbacks();
		}
	}

	public override void PlayFeedbacksOnlyIfNormalDirection(Vector3 position, float feedbacksIntensity = 1f, bool forceRevert = false)
	{
		if (Direction == Directions.TopToBottom)
		{
			PlayFeedbacks(position, feedbacksIntensity, forceRevert);
		}
	}

	public override IEnumerator PlayFeedbacksCoroutine(Vector3 position, float feedbacksIntensity = 1f, bool forceRevert = false)
	{
		PlayFeedbacks(position, feedbacksIntensity, forceRevert);
		while (base.IsPlaying)
		{
			yield return null;
		}
	}

	protected override void PlayFeedbacksInternal(Vector3 position, float feedbacksIntensity, bool forceRevert = false)
	{
		if ((base.IsPlaying && !CanPlayWhileAlreadyPlaying) || !EvaluateChance() || (CooldownDuration > 0f && GetTime() - _lastStartAt < CooldownDuration))
		{
			return;
		}
		SkippingToTheEnd = false;
		if (MMFeedbacks.GlobalMMFeedbacksActive && base.gameObject.activeInHierarchy)
		{
			if (base.ShouldRevertOnNextPlay)
			{
				Revert();
				base.ShouldRevertOnNextPlay = false;
			}
			if (forceRevert)
			{
				Direction = ((Direction != Directions.BottomToTop) ? Directions.BottomToTop : Directions.TopToBottom);
			}
			ResetFeedbacks();
			base.enabled = true;
			base.IsPlaying = true;
			_startTime = GetTime();
			_lastStartAt = _startTime;
			_totalDuration = TotalDuration;
			if (Time.frameCount < 2)
			{
				StartCoroutine(FrameOnePlayCo(position, feedbacksIntensity, forceRevert));
			}
			else if (InitialDelay > 0f)
			{
				StartCoroutine(HandleInitialDelayCo(position, feedbacksIntensity, forceRevert));
			}
			else
			{
				PreparePlay(position, feedbacksIntensity, forceRevert);
			}
		}
	}

	protected virtual IEnumerator FrameOnePlayCo(Vector3 position, float feedbacksIntensity, bool forceRevert = false)
	{
		yield return null;
		_startTime = GetTime();
		_lastStartAt = _startTime;
		base.IsPlaying = true;
		yield return MMFeedbacksCoroutine.WaitForUnscaled(InitialDelay);
		PreparePlay(position, feedbacksIntensity, forceRevert);
	}

	protected override void PreparePlay(Vector3 position, float feedbacksIntensity, bool forceRevert = false)
	{
		Events.TriggerOnPlay(this);
		_holdingMax = 0f;
		_pauseFound = false;
		int count = FeedbacksList.Count;
		for (int i = 0; i < count; i++)
		{
			if (FeedbacksList[i] != null)
			{
				if (FeedbacksList[i].Pause != null && FeedbacksList[i].Active && FeedbacksList[i].ShouldPlayInThisSequenceDirection)
				{
					_pauseFound = true;
				}
				if (FeedbacksList[i].HoldingPause && FeedbacksList[i].Active && FeedbacksList[i].ShouldPlayInThisSequenceDirection)
				{
					_pauseFound = true;
				}
			}
		}
		if (!_pauseFound)
		{
			PlayAllFeedbacks(position, feedbacksIntensity, forceRevert);
		}
		else
		{
			StartCoroutine(PausedFeedbacksCo(position, feedbacksIntensity));
		}
	}

	protected override void PlayAllFeedbacks(Vector3 position, float feedbacksIntensity, bool forceRevert = false)
	{
		int count = FeedbacksList.Count;
		for (int i = 0; i < count; i++)
		{
			if (FeedbackCanPlay(FeedbacksList[i]))
			{
				FeedbacksList[i].Play(position, feedbacksIntensity);
			}
		}
	}

	protected override IEnumerator HandleInitialDelayCo(Vector3 position, float feedbacksIntensity, bool forceRevert = false)
	{
		base.IsPlaying = true;
		yield return MMFeedbacksCoroutine.WaitForUnscaled(InitialDelay);
		PreparePlay(position, feedbacksIntensity, forceRevert);
	}

	protected override void Update()
	{
		if (_shouldStop)
		{
			if (HasFeedbackStillPlaying())
			{
				return;
			}
			base.IsPlaying = false;
			Events.TriggerOnComplete(this);
			ApplyAutoRevert();
			base.enabled = false;
			_shouldStop = false;
		}
		if (base.IsPlaying)
		{
			if (!_pauseFound && GetTime() - _startTime > _totalDuration)
			{
				_shouldStop = true;
			}
		}
		else
		{
			base.enabled = false;
		}
	}

	protected override IEnumerator PausedFeedbacksCo(Vector3 position, float feedbacksIntensity)
	{
		base.IsPlaying = true;
		int i = ((Direction != Directions.TopToBottom) ? (FeedbacksList.Count - 1) : 0);
		for (int count = FeedbacksList.Count; i >= 0 && i < count; i += ((Direction == Directions.TopToBottom) ? 1 : (-1)))
		{
			if (!base.IsPlaying || FeedbacksList[i] == null)
			{
				yield break;
			}
			if ((FeedbacksList[i].Active && FeedbacksList[i].ScriptDrivenPause) || base.InScriptDrivenPause)
			{
				base.InScriptDrivenPause = true;
				bool inAutoResume = FeedbacksList[i].ScriptDrivenPauseAutoResume > 0f;
				float scriptDrivenPauseStartedAt = GetTime();
				float autoResumeDuration = FeedbacksList[i].ScriptDrivenPauseAutoResume;
				while (base.InScriptDrivenPause)
				{
					if (inAutoResume && GetTime() - scriptDrivenPauseStartedAt > autoResumeDuration)
					{
						ResumeFeedbacks();
					}
					yield return null;
				}
			}
			if (FeedbacksList[i].Active && (FeedbacksList[i].HoldingPause || FeedbacksList[i].LooperPause) && FeedbacksList[i].ShouldPlayInThisSequenceDirection)
			{
				Events.TriggerOnPause(this);
				while (GetTime() - _lastStartAt < _holdingMax)
				{
					yield return null;
				}
				_holdingMax = 0f;
				_lastStartAt = GetTime();
			}
			if (FeedbackCanPlay(FeedbacksList[i]))
			{
				FeedbacksList[i].Play(position, feedbacksIntensity);
			}
			if (FeedbacksList[i].Pause != null && FeedbacksList[i].Active && FeedbacksList[i].ShouldPlayInThisSequenceDirection)
			{
				bool flag = true;
				if (FeedbacksList[i].Chance < 100f && UnityEngine.Random.Range(0f, 100f) > FeedbacksList[i].Chance)
				{
					flag = false;
				}
				if (flag)
				{
					yield return FeedbacksList[i].Pause;
					Events.TriggerOnResume(this);
					_lastStartAt = GetTime();
					_holdingMax = 0f;
				}
			}
			if (FeedbacksList[i].Active && FeedbacksList[i].Pause == null && FeedbacksList[i].ShouldPlayInThisSequenceDirection && !FeedbacksList[i].Timing.ExcludeFromHoldingPauses)
			{
				float totalDuration = FeedbacksList[i].TotalDuration;
				_holdingMax = Mathf.Max(totalDuration, _holdingMax);
			}
			if (!FeedbacksList[i].LooperPause || !FeedbacksList[i].Active || !FeedbacksList[i].ShouldPlayInThisSequenceDirection || ((FeedbacksList[i] as MMF_Looper).NumberOfLoopsLeft <= 0 && !(FeedbacksList[i] as MMF_Looper).InInfiniteLoop))
			{
				continue;
			}
			while (HasFeedbackStillPlaying())
			{
				yield return null;
			}
			bool loopAtLastPause = (FeedbacksList[i] as MMF_Looper).LoopAtLastPause;
			bool loopAtLastLoopStart = (FeedbacksList[i] as MMF_Looper).LoopAtLastLoopStart;
			int num = 0;
			int j = ((Direction == Directions.TopToBottom) ? (i - 1) : (i + 1));
			for (int count2 = FeedbacksList.Count; j >= 0 && j <= count2; j += ((Direction != Directions.TopToBottom) ? 1 : (-1)))
			{
				if (j == 0)
				{
					num = j - 1;
					break;
				}
				if (j == count2)
				{
					num = j;
					break;
				}
				if (((FeedbacksList[j].Pause != null && FeedbacksList[j].FeedbackDuration > 0f) & loopAtLastPause) && FeedbacksList[j].Active)
				{
					num = j;
					break;
				}
				if ((FeedbacksList[j].LooperStart & loopAtLastLoopStart) && FeedbacksList[j].Active)
				{
					num = j;
					break;
				}
			}
			i = num;
		}
		float unscaledTimeAtEnd = GetTime();
		while (GetTime() - unscaledTimeAtEnd < _holdingMax)
		{
			yield return null;
		}
		while (HasFeedbackStillPlaying())
		{
			yield return null;
		}
		base.IsPlaying = false;
		Events.TriggerOnComplete(this);
		ApplyAutoRevert();
	}

	protected virtual IEnumerator SkipToTheEndCo()
	{
		SkippingToTheEnd = true;
		Events.TriggerOnSkip(this);
		int count = FeedbacksList.Count;
		for (int i = 0; i < count; i++)
		{
			if (FeedbacksList[i] != null && FeedbacksList[i].Active)
			{
				FeedbacksList[i].SkipToTheEnd(base.transform.position);
			}
		}
		yield return null;
		yield return null;
		SkippingToTheEnd = false;
		StopFeedbacks();
	}

	public override void StopFeedbacks()
	{
		StopFeedbacks(true);
	}

	public override void StopFeedbacks(bool stopAllFeedbacks = true)
	{
		StopFeedbacks(base.transform.position, 1f, stopAllFeedbacks);
	}

	public override void StopFeedbacks(Vector3 position, float feedbacksIntensity = 1f, bool stopAllFeedbacks = true)
	{
		if (stopAllFeedbacks)
		{
			int count = FeedbacksList.Count;
			for (int i = 0; i < count; i++)
			{
				FeedbacksList[i].Stop(position, feedbacksIntensity);
			}
		}
		base.IsPlaying = false;
		StopAllCoroutines();
	}

	public override void ResetFeedbacks()
	{
		int count = FeedbacksList.Count;
		for (int i = 0; i < count; i++)
		{
			if (FeedbacksList[i] != null && FeedbacksList[i].Active)
			{
				FeedbacksList[i].ResetFeedback();
			}
		}
		base.IsPlaying = false;
	}

	public override void Revert()
	{
		Events.TriggerOnRevert(this);
		Direction = ((Direction != Directions.BottomToTop) ? Directions.BottomToTop : Directions.TopToBottom);
	}

	public override void PauseFeedbacks()
	{
		Events.TriggerOnPause(this);
		base.InScriptDrivenPause = true;
	}

	public virtual void SkipToTheEnd()
	{
		StartCoroutine(SkipToTheEndCo());
	}

	public override void ResumeFeedbacks()
	{
		Events.TriggerOnResume(this);
		base.InScriptDrivenPause = false;
	}

	public virtual void AddFeedback(MMF_Feedback newFeedback)
	{
		InitializeList();
		newFeedback.Owner = this;
		newFeedback.UniqueID = Guid.NewGuid().GetHashCode();
		FeedbacksList.Add(newFeedback);
		newFeedback.CacheRequiresSetup();
		newFeedback.InitializeCustomAttributes();
	}

	public new MMF_Feedback AddFeedback(Type feedbackType)
	{
		InitializeList();
		MMF_Feedback mMF_Feedback = (MMF_Feedback)Activator.CreateInstance(feedbackType);
		mMF_Feedback.Label = FeedbackPathAttribute.GetFeedbackDefaultName(feedbackType);
		mMF_Feedback.Owner = this;
		mMF_Feedback.UniqueID = Guid.NewGuid().GetHashCode();
		FeedbacksList.Add(mMF_Feedback);
		mMF_Feedback.InitializeCustomAttributes();
		mMF_Feedback.CacheRequiresSetup();
		return mMF_Feedback;
	}

	public override void RemoveFeedback(int id)
	{
		if (FeedbacksList.Count >= id)
		{
			FeedbacksList.RemoveAt(id);
		}
	}

	public override bool HasFeedbackStillPlaying()
	{
		int count = FeedbacksList.Count;
		for (int i = 0; i < count; i++)
		{
			if (FeedbacksList[i].IsPlaying && !FeedbacksList[i].Timing.ExcludeFromHoldingPauses)
			{
				return true;
			}
		}
		return false;
	}

	protected override void CheckForLoops()
	{
		base.ContainsLoop = false;
		int count = FeedbacksList.Count;
		for (int i = 0; i < count; i++)
		{
			if (FeedbacksList[i] != null && FeedbacksList[i].LooperPause && FeedbacksList[i].Active)
			{
				base.ContainsLoop = true;
				break;
			}
		}
	}

	protected bool FeedbackCanPlay(MMF_Feedback feedback)
	{
		if (feedback.Timing.MMFeedbacksDirectionCondition == MMFeedbackTiming.MMFeedbacksDirectionConditions.Always)
		{
			return true;
		}
		if ((Direction == Directions.TopToBottom && feedback.Timing.MMFeedbacksDirectionCondition == MMFeedbackTiming.MMFeedbacksDirectionConditions.OnlyWhenForwards) || (Direction == Directions.BottomToTop && feedback.Timing.MMFeedbacksDirectionCondition == MMFeedbackTiming.MMFeedbacksDirectionConditions.OnlyWhenBackwards))
		{
			return true;
		}
		return false;
	}

	protected override void ApplyAutoRevert()
	{
		if (AutoChangeDirectionOnEnd)
		{
			base.ShouldRevertOnNextPlay = true;
		}
	}

	public override float ApplyTimeMultiplier(float duration)
	{
		return duration * DurationMultiplier;
	}

	public virtual void ProxyDestroy(GameObject gameObjectToDestroy)
	{
		UnityEngine.Object.Destroy(gameObjectToDestroy);
	}

	public virtual void ProxyDestroy(GameObject gameObjectToDestroy, float delay)
	{
		UnityEngine.Object.Destroy(gameObjectToDestroy, delay);
	}

	public virtual void ProxyDestroyImmediate(GameObject gameObjectToDestroy)
	{
		UnityEngine.Object.DestroyImmediate(gameObjectToDestroy);
	}

	public virtual T GetFeedbackOfType<T>() where T : MMF_Feedback
	{
		_t = typeof(T);
		foreach (MMF_Feedback feedbacks in FeedbacksList)
		{
			if (feedbacks.GetType() == _t)
			{
				return (T)feedbacks;
			}
		}
		return null;
	}

	public virtual List<T> GetFeedbacksOfType<T>() where T : MMF_Feedback
	{
		_t = typeof(T);
		List<T> list = new List<T>();
		foreach (MMF_Feedback feedbacks in FeedbacksList)
		{
			if (feedbacks.GetType() == _t)
			{
				list.Add((T)feedbacks);
			}
		}
		return list;
	}

	public virtual T GetFeedbackOfType<T>(string searchedLabel) where T : MMF_Feedback
	{
		_t = typeof(T);
		foreach (MMF_Feedback feedbacks in FeedbacksList)
		{
			if (feedbacks.GetType() == _t && feedbacks.Label == searchedLabel)
			{
				return (T)feedbacks;
			}
		}
		return null;
	}

	public virtual List<T> GetFeedbacksOfType<T>(string searchedLabel) where T : MMF_Feedback
	{
		_t = typeof(T);
		List<T> list = new List<T>();
		foreach (MMF_Feedback feedbacks in FeedbacksList)
		{
			if (feedbacks.GetType() == _t && feedbacks.Label == searchedLabel)
			{
				list.Add((T)feedbacks);
			}
		}
		return list;
	}

	protected override void OnDisable()
	{
		if (base.IsPlaying)
		{
			if (ForceStopFeedbacksOnDisable)
			{
				StopFeedbacks();
			}
			StopAllCoroutines();
			for (int num = FeedbacksList.Count - 1; num >= 0; num--)
			{
				FeedbacksList[num].OnDisable();
			}
		}
	}

	protected override void OnValidate()
	{
		RefreshCache();
	}

	public virtual void RefreshCache()
	{
		if (FeedbacksList == null)
		{
			return;
		}
		DurationMultiplier = Mathf.Clamp(DurationMultiplier, 0f, float.MaxValue);
		for (int num = FeedbacksList.Count - 1; num >= 0; num--)
		{
			if (FeedbacksList[num] == null)
			{
				FeedbacksList.RemoveAt(num);
			}
			else
			{
				FeedbacksList[num].Owner = this;
				FeedbacksList[num].CacheRequiresSetup();
				FeedbacksList[num].OnValidate();
			}
		}
	}

	protected override void OnDestroy()
	{
		base.IsPlaying = false;
		foreach (MMF_Feedback feedbacks in FeedbacksList)
		{
			feedbacks.OnDestroy();
		}
	}
}
