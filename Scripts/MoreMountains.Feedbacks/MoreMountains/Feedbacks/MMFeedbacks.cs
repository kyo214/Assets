using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("More Mountains/Feedbacks/MMFeedbacks")]
public class MMFeedbacks : MonoBehaviour
{
	public enum Directions
	{
		TopToBottom = 0,
		BottomToTop = 1
	}

	public enum SafeModes
	{
		Nope = 0,
		EditorOnly = 1,
		RuntimeOnly = 2,
		Full = 3
	}

	public enum InitializationModes
	{
		Script = 0,
		Awake = 1,
		Start = 2
	}

	public List<MMFeedback> Feedbacks = new List<MMFeedback>();

	[Tooltip("the chosen initialization modes. If you use Script, you'll have to initialize manually by calling the Initialization method and passing it an owner. Otherwise, you can have this component initialize itself at Awake or Start, and in this case the owner will be the MMFeedbacks itself")]
	public InitializationModes InitializationMode = InitializationModes.Start;

	[Tooltip("the selected safe mode")]
	public SafeModes SafeMode = SafeModes.Full;

	[Tooltip("the selected direction these feedbacks should play in")]
	public Directions Direction;

	[Tooltip("whether or not this MMFeedbacks should invert its direction when all feedbacks have played")]
	public bool AutoChangeDirectionOnEnd;

	[Tooltip("whether or not to play this feedbacks automatically on Start")]
	public bool AutoPlayOnStart;

	[Tooltip("whether or not to play this feedbacks automatically on Enable")]
	public bool AutoPlayOnEnable;

	[Tooltip("if this is true, all feedbacks within that player will work on the specified ForcedTimescaleMode, regardless of their individual settings")]
	public bool ForceTimescaleMode;

	[Tooltip("the time scale mode all feedbacks on this player should work on, if ForceTimescaleMode is true")]
	[MMFCondition("ForceTimescaleMode", true)]
	public TimescaleModes ForcedTimescaleMode = TimescaleModes.Unscaled;

	[Tooltip("a time multiplier that will be applied to all feedback durations (initial delay, duration, delay between repeats...)")]
	public float DurationMultiplier = 1f;

	[Tooltip("if this is true, more editor-only, detailed info will be displayed per feedback in the duration slot")]
	public bool DisplayFullDurationDetails;

	[Tooltip("the timescale at which the player itself will operate. This notably impacts sequencing and pauses duration evaluation.")]
	public TimescaleModes PlayerTimescaleMode = TimescaleModes.Unscaled;

	[Tooltip("a duration, in seconds, during which triggering a new play of this MMFeedbacks after it's been played once will be impossible")]
	public float CooldownDuration;

	[Tooltip("a duration, in seconds, to delay the start of this MMFeedbacks' contents play")]
	public float InitialDelay;

	[Tooltip("if this is true, you'll be able to trigger a new Play while this feedback is already playing, otherwise you won't be able to")]
	public bool CanPlayWhileAlreadyPlaying = true;

	[Tooltip("the chance of this sequence happening (in percent : 100 : happens all the time, 0 : never happens, 50 : happens once every two calls, etc)")]
	[Range(0f, 100f)]
	public float ChanceToPlay = 100f;

	[Tooltip("the intensity at which to play this feedback. That value will be used by most feedbacks to tune their amplitude. 1 is normal, 0.5 is half power, 0 is no effect.Note that what this value controls depends from feedback to feedback, don't hesitate to check the code to see what it does exactly.")]
	public float FeedbacksIntensity = 1f;

	[Tooltip("a number of UnityEvents that can be triggered at the various stages of this MMFeedbacks")]
	public MMFeedbacksEvents Events;

	[Tooltip("a global switch used to turn all feedbacks on or off globally")]
	public static bool GlobalMMFeedbacksActive = true;

	[HideInInspector]
	public bool DebugActive;

	protected float _startTime;

	protected float _holdingMax;

	protected float _lastStartAt = float.MinValue;

	protected bool _pauseFound;

	protected float _totalDuration;

	protected bool _shouldStop;

	public bool IsPlaying { get; protected set; }

	public float ElapsedTime
	{
		get
		{
			if (!IsPlaying)
			{
				return 0f;
			}
			return GetTime() - _lastStartAt;
		}
	}

	public int TimesPlayed { get; protected set; }

	public bool InScriptDrivenPause { get; set; }

	public bool ContainsLoop { get; set; }

	public bool ShouldRevertOnNextPlay { get; set; }

	public virtual float TotalDuration
	{
		get
		{
			float num = 0f;
			foreach (MMFeedback feedback in Feedbacks)
			{
				if (feedback != null && feedback.Active && num < feedback.TotalDuration)
				{
					num = feedback.TotalDuration;
				}
			}
			return InitialDelay + num;
		}
	}

	public virtual float GetTime()
	{
		if (PlayerTimescaleMode != TimescaleModes.Scaled)
		{
			return Time.unscaledTime;
		}
		return Time.time;
	}

	public virtual float GetDeltaTime()
	{
		if (PlayerTimescaleMode != TimescaleModes.Scaled)
		{
			return Time.unscaledDeltaTime;
		}
		return Time.deltaTime;
	}

	protected virtual void Awake()
	{
		if (AutoPlayOnEnable)
		{
			MMFeedbacksEnabler mMFeedbacksEnabler = GetComponent<MMFeedbacksEnabler>();
			if (mMFeedbacksEnabler == null)
			{
				mMFeedbacksEnabler = base.gameObject.AddComponent<MMFeedbacksEnabler>();
			}
			mMFeedbacksEnabler.TargetMMFeedbacks = this;
		}
		if (InitializationMode == InitializationModes.Awake && Application.isPlaying)
		{
			Initialization(base.gameObject);
		}
		CheckForLoops();
	}

	protected virtual void Start()
	{
		if (InitializationMode == InitializationModes.Start && Application.isPlaying)
		{
			Initialization(base.gameObject);
		}
		if (AutoPlayOnStart && Application.isPlaying)
		{
			PlayFeedbacks();
		}
		CheckForLoops();
	}

	protected virtual void OnEnable()
	{
		if (AutoPlayOnEnable && Application.isPlaying)
		{
			PlayFeedbacks();
		}
	}

	public virtual void Initialization()
	{
		Initialization(base.gameObject);
	}

	public virtual void Initialization(GameObject owner)
	{
		if (SafeMode == SafeModes.RuntimeOnly || SafeMode == SafeModes.Full)
		{
			AutoRepair();
		}
		IsPlaying = false;
		TimesPlayed = 0;
		_lastStartAt = float.MinValue;
		for (int i = 0; i < Feedbacks.Count; i++)
		{
			if (Feedbacks[i] != null)
			{
				Feedbacks[i].Initialization(owner);
			}
		}
	}

	public virtual void PlayFeedbacks()
	{
		PlayFeedbacksInternal(base.transform.position, FeedbacksIntensity);
	}

	public virtual async Task PlayFeedbacksTask(Vector3 position, float feedbacksIntensity = 1f, bool forceRevert = false)
	{
		PlayFeedbacks(position, feedbacksIntensity, forceRevert);
		while (IsPlaying)
		{
			await Task.Yield();
		}
	}

	public virtual void PlayFeedbacks(Vector3 position, float feedbacksIntensity = 1f, bool forceRevert = false)
	{
		PlayFeedbacksInternal(position, feedbacksIntensity, forceRevert);
	}

	public virtual void PlayFeedbacksInReverse()
	{
		PlayFeedbacksInternal(base.transform.position, FeedbacksIntensity, forceRevert: true);
	}

	public virtual void PlayFeedbacksInReverse(Vector3 position, float feedbacksIntensity = 1f, bool forceRevert = false)
	{
		PlayFeedbacksInternal(position, feedbacksIntensity, forceRevert);
	}

	public virtual void PlayFeedbacksOnlyIfReversed()
	{
		if ((Direction == Directions.BottomToTop && !ShouldRevertOnNextPlay) || (Direction == Directions.TopToBottom && ShouldRevertOnNextPlay))
		{
			PlayFeedbacks();
		}
	}

	public virtual void PlayFeedbacksOnlyIfReversed(Vector3 position, float feedbacksIntensity = 1f, bool forceRevert = false)
	{
		if ((Direction == Directions.BottomToTop && !ShouldRevertOnNextPlay) || (Direction == Directions.TopToBottom && ShouldRevertOnNextPlay))
		{
			PlayFeedbacks(position, feedbacksIntensity, forceRevert);
		}
	}

	public virtual void PlayFeedbacksOnlyIfNormalDirection()
	{
		if (Direction == Directions.TopToBottom)
		{
			PlayFeedbacks();
		}
	}

	public virtual void PlayFeedbacksOnlyIfNormalDirection(Vector3 position, float feedbacksIntensity = 1f, bool forceRevert = false)
	{
		if (Direction == Directions.TopToBottom)
		{
			PlayFeedbacks(position, feedbacksIntensity, forceRevert);
		}
	}

	public virtual IEnumerator PlayFeedbacksCoroutine(Vector3 position, float feedbacksIntensity = 1f, bool forceRevert = false)
	{
		PlayFeedbacks(position, feedbacksIntensity, forceRevert);
		while (IsPlaying)
		{
			yield return null;
		}
	}

	protected virtual void PlayFeedbacksInternal(Vector3 position, float feedbacksIntensity, bool forceRevert = false)
	{
		if ((!IsPlaying || CanPlayWhileAlreadyPlaying) && EvaluateChance() && (!(CooldownDuration > 0f) || !(GetTime() - _lastStartAt < CooldownDuration)) && GlobalMMFeedbacksActive && base.gameObject.activeInHierarchy)
		{
			if (ShouldRevertOnNextPlay)
			{
				Revert();
				ShouldRevertOnNextPlay = false;
			}
			if (forceRevert)
			{
				Direction = ((Direction != Directions.BottomToTop) ? Directions.BottomToTop : Directions.TopToBottom);
			}
			ResetFeedbacks();
			base.enabled = true;
			TimesPlayed++;
			IsPlaying = true;
			_startTime = GetTime();
			_lastStartAt = _startTime;
			_totalDuration = TotalDuration;
			if (InitialDelay > 0f)
			{
				StartCoroutine(HandleInitialDelayCo(position, feedbacksIntensity, forceRevert));
			}
			else
			{
				PreparePlay(position, feedbacksIntensity, forceRevert);
			}
		}
	}

	protected virtual void PreparePlay(Vector3 position, float feedbacksIntensity, bool forceRevert = false)
	{
		Events.TriggerOnPlay(this);
		_holdingMax = 0f;
		_pauseFound = false;
		for (int i = 0; i < Feedbacks.Count; i++)
		{
			if (Feedbacks[i] != null)
			{
				if (Feedbacks[i].Pause != null && Feedbacks[i].Active && Feedbacks[i].ShouldPlayInThisSequenceDirection)
				{
					_pauseFound = true;
				}
				if (Feedbacks[i].HoldingPause && Feedbacks[i].Active && Feedbacks[i].ShouldPlayInThisSequenceDirection)
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

	protected virtual void PlayAllFeedbacks(Vector3 position, float feedbacksIntensity, bool forceRevert = false)
	{
		for (int i = 0; i < Feedbacks.Count; i++)
		{
			if (FeedbackCanPlay(Feedbacks[i]))
			{
				Feedbacks[i].Play(position, feedbacksIntensity);
			}
		}
	}

	protected virtual IEnumerator HandleInitialDelayCo(Vector3 position, float feedbacksIntensity, bool forceRevert = false)
	{
		IsPlaying = true;
		yield return MMFeedbacksCoroutine.WaitFor(InitialDelay);
		PreparePlay(position, feedbacksIntensity, forceRevert);
	}

	protected virtual void Update()
	{
		if (_shouldStop)
		{
			if (HasFeedbackStillPlaying())
			{
				return;
			}
			IsPlaying = false;
			Events.TriggerOnComplete(this);
			ApplyAutoRevert();
			base.enabled = false;
			_shouldStop = false;
		}
		if (IsPlaying)
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

	public virtual bool HasFeedbackStillPlaying()
	{
		int count = Feedbacks.Count;
		for (int i = 0; i < count; i++)
		{
			if (Feedbacks[i].IsPlaying)
			{
				return true;
			}
		}
		return false;
	}

	protected virtual IEnumerator PausedFeedbacksCo(Vector3 position, float feedbacksIntensity)
	{
		IsPlaying = true;
		for (int i = ((Direction != Directions.TopToBottom) ? (Feedbacks.Count - 1) : 0); i >= 0 && i < Feedbacks.Count; i += ((Direction == Directions.TopToBottom) ? 1 : (-1)))
		{
			if (!IsPlaying || Feedbacks[i] == null)
			{
				yield break;
			}
			if ((Feedbacks[i].Active && Feedbacks[i].ScriptDrivenPause) || InScriptDrivenPause)
			{
				InScriptDrivenPause = true;
				bool inAutoResume = Feedbacks[i].ScriptDrivenPauseAutoResume > 0f;
				float scriptDrivenPauseStartedAt = GetTime();
				float autoResumeDuration = Feedbacks[i].ScriptDrivenPauseAutoResume;
				while (InScriptDrivenPause)
				{
					if (inAutoResume && GetTime() - scriptDrivenPauseStartedAt > autoResumeDuration)
					{
						ResumeFeedbacks();
					}
					yield return null;
				}
			}
			if (Feedbacks[i].Active && (Feedbacks[i].HoldingPause || Feedbacks[i].LooperPause) && Feedbacks[i].ShouldPlayInThisSequenceDirection)
			{
				Events.TriggerOnPause(this);
				while (GetTime() - _lastStartAt < _holdingMax)
				{
					yield return null;
				}
				_holdingMax = 0f;
				_lastStartAt = GetTime();
			}
			if (FeedbackCanPlay(Feedbacks[i]))
			{
				Feedbacks[i].Play(position, feedbacksIntensity);
			}
			if (Feedbacks[i].Pause != null && Feedbacks[i].Active && Feedbacks[i].ShouldPlayInThisSequenceDirection)
			{
				bool flag = true;
				if (Feedbacks[i].Chance < 100f && UnityEngine.Random.Range(0f, 100f) > Feedbacks[i].Chance)
				{
					flag = false;
				}
				if (flag)
				{
					yield return Feedbacks[i].Pause;
					Events.TriggerOnResume(this);
					_lastStartAt = GetTime();
					_holdingMax = 0f;
				}
			}
			if (Feedbacks[i].Active && Feedbacks[i].Pause == null && Feedbacks[i].ShouldPlayInThisSequenceDirection && !Feedbacks[i].Timing.ExcludeFromHoldingPauses)
			{
				float totalDuration = Feedbacks[i].TotalDuration;
				_holdingMax = Mathf.Max(totalDuration, _holdingMax);
			}
			if (!Feedbacks[i].LooperPause || !Feedbacks[i].Active || !Feedbacks[i].ShouldPlayInThisSequenceDirection || ((Feedbacks[i] as MMFeedbackLooper).NumberOfLoopsLeft <= 0 && !(Feedbacks[i] as MMFeedbackLooper).InInfiniteLoop))
			{
				continue;
			}
			bool loopAtLastPause = (Feedbacks[i] as MMFeedbackLooper).LoopAtLastPause;
			bool loopAtLastLoopStart = (Feedbacks[i] as MMFeedbackLooper).LoopAtLastLoopStart;
			int num = 0;
			for (int j = ((Direction == Directions.TopToBottom) ? (i - 1) : (i + 1)); j >= 0 && j <= Feedbacks.Count; j += ((Direction != Directions.TopToBottom) ? 1 : (-1)))
			{
				if (j == 0)
				{
					num = j - 1;
					break;
				}
				if (j == Feedbacks.Count)
				{
					num = j;
					break;
				}
				if (((Feedbacks[j].Pause != null && Feedbacks[j].FeedbackDuration > 0f) & loopAtLastPause) && Feedbacks[j].Active)
				{
					num = j;
					break;
				}
				if ((Feedbacks[j].LooperStart & loopAtLastLoopStart) && Feedbacks[j].Active)
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
		IsPlaying = false;
		Events.TriggerOnComplete(this);
		ApplyAutoRevert();
	}

	public virtual void StopFeedbacks()
	{
		StopFeedbacks(true);
	}

	public virtual void StopFeedbacks(bool stopAllFeedbacks = true)
	{
		StopFeedbacks(base.transform.position, 1f, stopAllFeedbacks);
	}

	public virtual void StopFeedbacks(Vector3 position, float feedbacksIntensity = 1f, bool stopAllFeedbacks = true)
	{
		if (stopAllFeedbacks)
		{
			for (int i = 0; i < Feedbacks.Count; i++)
			{
				Feedbacks[i].Stop(position, feedbacksIntensity);
			}
		}
		IsPlaying = false;
		StopAllCoroutines();
	}

	public virtual void ResetFeedbacks()
	{
		for (int i = 0; i < Feedbacks.Count; i++)
		{
			if (Feedbacks[i] != null && Feedbacks[i].Active)
			{
				Feedbacks[i].ResetFeedback();
			}
		}
		IsPlaying = false;
	}

	public virtual void Revert()
	{
		Events.TriggerOnRevert(this);
		Direction = ((Direction != Directions.BottomToTop) ? Directions.BottomToTop : Directions.TopToBottom);
	}

	public virtual void PauseFeedbacks()
	{
		Events.TriggerOnPause(this);
		InScriptDrivenPause = true;
	}

	public virtual void ResumeFeedbacks()
	{
		Events.TriggerOnResume(this);
		InScriptDrivenPause = false;
	}

	public virtual MMFeedback AddFeedback(Type feedbackType)
	{
		MMFeedback obj = base.gameObject.AddComponent(feedbackType) as MMFeedback;
		obj.hideFlags = HideFlags.HideInInspector;
		obj.Label = FeedbackPathAttribute.GetFeedbackDefaultName(feedbackType);
		AutoRepair();
		return obj;
	}

	public virtual void RemoveFeedback(int id)
	{
		UnityEngine.Object.DestroyImmediate(Feedbacks[id]);
		Feedbacks.RemoveAt(id);
		AutoRepair();
	}

	protected virtual bool EvaluateChance()
	{
		if (ChanceToPlay == 0f)
		{
			return false;
		}
		if (ChanceToPlay != 100f && UnityEngine.Random.Range(0f, 100f) > ChanceToPlay)
		{
			return false;
		}
		return true;
	}

	protected virtual void CheckForLoops()
	{
		ContainsLoop = false;
		for (int i = 0; i < Feedbacks.Count; i++)
		{
			if (Feedbacks[i] != null && Feedbacks[i].LooperPause && Feedbacks[i].Active)
			{
				ContainsLoop = true;
				break;
			}
		}
	}

	protected bool FeedbackCanPlay(MMFeedback feedback)
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

	protected virtual void ApplyAutoRevert()
	{
		if (AutoChangeDirectionOnEnd)
		{
			ShouldRevertOnNextPlay = true;
		}
	}

	public virtual float ApplyTimeMultiplier(float duration)
	{
		return duration * DurationMultiplier;
	}

	public virtual void AutoRepair()
	{
		new List<Component>();
		foreach (Component item in base.gameObject.GetComponents<Component>().ToList())
		{
			if (!(item is MMFeedback))
			{
				continue;
			}
			bool flag = false;
			for (int i = 0; i < Feedbacks.Count; i++)
			{
				if (Feedbacks[i] == (MMFeedback)item)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				Feedbacks.Add((MMFeedback)item);
			}
		}
	}

	protected virtual void OnDisable()
	{
	}

	protected virtual void OnValidate()
	{
		DurationMultiplier = Mathf.Clamp(DurationMultiplier, 0f, float.MaxValue);
	}

	protected virtual void OnDestroy()
	{
		IsPlaying = false;
	}
}
