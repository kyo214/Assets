using System;
using System.Collections;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[Serializable]
[AddComponentMenu("")]
[ExecuteAlways]
public abstract class MMFeedback : MonoBehaviour
{
	[Tooltip("whether or not this feedback is active")]
	public bool Active = true;

	[Tooltip("the name of this feedback to display in the inspector")]
	public string Label = "MMFeedback";

	[Tooltip("the chance of this feedback happening (in percent : 100 : happens all the time, 0 : never happens, 50 : happens once every two calls, etc)")]
	[Range(0f, 100f)]
	public float Chance = 100f;

	[Tooltip("a number of timing-related values (delay, repeat, etc)")]
	public MMFeedbackTiming Timing;

	[HideInInspector]
	public bool DebugActive;

	protected float _lastPlayTimestamp = -1f;

	protected int _playsLeft;

	protected bool _initialized;

	protected Coroutine _playCoroutine;

	protected Coroutine _infinitePlayCoroutine;

	protected Coroutine _sequenceCoroutine;

	protected Coroutine _repeatedPlayCoroutine;

	protected int _sequenceTrackID;

	protected MMFeedbacks _hostMMFeedbacks;

	protected float _beatInterval;

	protected bool BeatThisFrame;

	protected int LastBeatIndex;

	protected int CurrentSequenceIndex;

	protected float LastBeatTimestamp;

	protected bool _isHostMMFeedbacksNotNull;

	public GameObject Owner { get; set; }

	public virtual IEnumerator Pause => null;

	public virtual bool HoldingPause => false;

	public virtual bool LooperPause => false;

	public virtual bool ScriptDrivenPause { get; set; }

	public virtual float ScriptDrivenPauseAutoResume { get; set; }

	public virtual bool LooperStart => false;

	public virtual bool InCooldown
	{
		get
		{
			if (Timing.CooldownDuration > 0f)
			{
				return FeedbackTime - _lastPlayTimestamp < Timing.CooldownDuration;
			}
			return false;
		}
	}

	public virtual bool IsPlaying { get; set; }

	public float FeedbackTime
	{
		get
		{
			if (Timing.TimescaleMode == TimescaleModes.Scaled)
			{
				return Time.time;
			}
			return Time.unscaledTime;
		}
	}

	public float FeedbackDeltaTime
	{
		get
		{
			if (Timing.TimescaleMode == TimescaleModes.Scaled)
			{
				return Time.deltaTime;
			}
			return Time.unscaledDeltaTime;
		}
	}

	public float TotalDuration
	{
		get
		{
			if (Timing != null && !Timing.ContributeToTotalDuration)
			{
				return 0f;
			}
			float num = 0f;
			if (Timing == null)
			{
				return 0f;
			}
			if (Timing.InitialDelay != 0f)
			{
				num += ApplyTimeMultiplier(Timing.InitialDelay);
			}
			num += FeedbackDuration;
			if (Timing.NumberOfRepeats > 0)
			{
				float num2 = ApplyTimeMultiplier(Timing.DelayBetweenRepeats);
				num += (float)Timing.NumberOfRepeats * FeedbackDuration + (float)Timing.NumberOfRepeats * num2;
			}
			return num;
		}
	}

	public virtual float FeedbackStartedAt => _lastPlayTimestamp;

	public virtual float FeedbackDuration
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public virtual bool FeedbackPlaying
	{
		get
		{
			if (FeedbackStartedAt > 0f)
			{
				return Time.time - FeedbackStartedAt < FeedbackDuration;
			}
			return false;
		}
	}

	public virtual bool NormalPlayDirection => Timing.PlayDirection switch
	{
		MMFeedbackTiming.PlayDirections.FollowMMFeedbacksDirection => _hostMMFeedbacks.Direction == MMFeedbacks.Directions.TopToBottom, 
		MMFeedbackTiming.PlayDirections.AlwaysNormal => true, 
		MMFeedbackTiming.PlayDirections.AlwaysRewind => false, 
		MMFeedbackTiming.PlayDirections.OppositeMMFeedbacksDirection => _hostMMFeedbacks.Direction != MMFeedbacks.Directions.TopToBottom, 
		_ => true, 
	};

	public virtual bool ShouldPlayInThisSequenceDirection => Timing.MMFeedbacksDirectionCondition switch
	{
		MMFeedbackTiming.MMFeedbacksDirectionConditions.Always => true, 
		MMFeedbackTiming.MMFeedbacksDirectionConditions.OnlyWhenForwards => _hostMMFeedbacks.Direction == MMFeedbacks.Directions.TopToBottom, 
		MMFeedbackTiming.MMFeedbacksDirectionConditions.OnlyWhenBackwards => _hostMMFeedbacks.Direction == MMFeedbacks.Directions.BottomToTop, 
		_ => true, 
	};

	protected virtual float FinalNormalizedTime
	{
		get
		{
			if (!NormalPlayDirection)
			{
				return 0f;
			}
			return 1f;
		}
	}

	protected virtual void OnEnable()
	{
		_hostMMFeedbacks = base.gameObject.GetComponent<MMFeedbacks>();
		_isHostMMFeedbacksNotNull = _hostMMFeedbacks != null;
	}

	public virtual void Initialization(GameObject owner)
	{
		_initialized = true;
		Owner = owner;
		_playsLeft = Timing.NumberOfRepeats + 1;
		_hostMMFeedbacks = base.gameObject.GetComponent<MMFeedbacks>();
		SetInitialDelay(Timing.InitialDelay);
		SetDelayBetweenRepeats(Timing.DelayBetweenRepeats);
		SetSequence(Timing.Sequence);
		CustomInitialization(owner);
	}

	public virtual void Play(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active)
		{
			return;
		}
		if (!_initialized)
		{
			Debug.LogWarning("The " + this?.ToString() + " feedback is being played without having been initialized. Call Initialization() first.");
		}
		if (!InCooldown)
		{
			if (Timing.InitialDelay > 0f)
			{
				_playCoroutine = StartCoroutine(PlayCoroutine(position, feedbacksIntensity));
				return;
			}
			_lastPlayTimestamp = FeedbackTime;
			RegularPlay(position, feedbacksIntensity);
		}
	}

	protected virtual IEnumerator PlayCoroutine(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Timing.TimescaleMode == TimescaleModes.Scaled)
		{
			yield return MMFeedbacksCoroutine.WaitFor(Timing.InitialDelay);
		}
		else
		{
			yield return MMFeedbacksCoroutine.WaitForUnscaled(Timing.InitialDelay);
		}
		_lastPlayTimestamp = FeedbackTime;
		RegularPlay(position, feedbacksIntensity);
	}

	protected virtual void RegularPlay(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Chance != 0f && (Chance == 100f || !(UnityEngine.Random.Range(0f, 100f) > Chance)) && (!Timing.UseIntensityInterval || (!(feedbacksIntensity < Timing.IntensityIntervalMin) && !(feedbacksIntensity >= Timing.IntensityIntervalMax))))
		{
			if (Timing.RepeatForever)
			{
				_infinitePlayCoroutine = StartCoroutine(InfinitePlay(position, feedbacksIntensity));
			}
			else if (Timing.NumberOfRepeats > 0)
			{
				_repeatedPlayCoroutine = StartCoroutine(RepeatedPlay(position, feedbacksIntensity));
			}
			else if (Timing.Sequence == null)
			{
				CustomPlayFeedback(position, feedbacksIntensity);
			}
			else
			{
				_sequenceCoroutine = StartCoroutine(SequenceCoroutine(position, feedbacksIntensity));
			}
		}
	}

	protected virtual IEnumerator InfinitePlay(Vector3 position, float feedbacksIntensity = 1f)
	{
		while (true)
		{
			_lastPlayTimestamp = FeedbackTime;
			if (Timing.Sequence == null)
			{
				CustomPlayFeedback(position, feedbacksIntensity);
				if (Timing.TimescaleMode == TimescaleModes.Scaled)
				{
					yield return MMFeedbacksCoroutine.WaitFor(Timing.DelayBetweenRepeats);
				}
				else
				{
					yield return MMFeedbacksCoroutine.WaitForUnscaled(Timing.DelayBetweenRepeats);
				}
				continue;
			}
			_sequenceCoroutine = StartCoroutine(SequenceCoroutine(position, feedbacksIntensity));
			float seconds = ApplyTimeMultiplier(Timing.DelayBetweenRepeats) + Timing.Sequence.Length;
			if (Timing.TimescaleMode == TimescaleModes.Scaled)
			{
				yield return MMFeedbacksCoroutine.WaitFor(seconds);
			}
			else
			{
				yield return MMFeedbacksCoroutine.WaitForUnscaled(seconds);
			}
		}
	}

	protected virtual IEnumerator RepeatedPlay(Vector3 position, float feedbacksIntensity = 1f)
	{
		while (_playsLeft > 0)
		{
			_lastPlayTimestamp = FeedbackTime;
			_playsLeft--;
			if (Timing.Sequence == null)
			{
				CustomPlayFeedback(position, feedbacksIntensity);
				if (Timing.TimescaleMode == TimescaleModes.Scaled)
				{
					yield return MMFeedbacksCoroutine.WaitFor(Timing.DelayBetweenRepeats);
				}
				else
				{
					yield return MMFeedbacksCoroutine.WaitForUnscaled(Timing.DelayBetweenRepeats);
				}
				continue;
			}
			_sequenceCoroutine = StartCoroutine(SequenceCoroutine(position, feedbacksIntensity));
			float seconds = ApplyTimeMultiplier(Timing.DelayBetweenRepeats) + Timing.Sequence.Length;
			if (Timing.TimescaleMode == TimescaleModes.Scaled)
			{
				yield return MMFeedbacksCoroutine.WaitFor(seconds);
			}
			else
			{
				yield return MMFeedbacksCoroutine.WaitForUnscaled(seconds);
			}
		}
		_playsLeft = Timing.NumberOfRepeats + 1;
	}

	protected virtual IEnumerator SequenceCoroutine(Vector3 position, float feedbacksIntensity = 1f)
	{
		yield return null;
		float timeStartedAt = FeedbackTime;
		float lastFrame = FeedbackTime;
		BeatThisFrame = false;
		LastBeatIndex = 0;
		CurrentSequenceIndex = 0;
		LastBeatTimestamp = 0f;
		if (Timing.Quantized)
		{
			while (CurrentSequenceIndex < Timing.Sequence.QuantizedSequence[0].Line.Count)
			{
				_beatInterval = 60f / (float)Timing.TargetBPM;
				if (FeedbackTime - LastBeatTimestamp >= _beatInterval || LastBeatTimestamp == 0f)
				{
					BeatThisFrame = true;
					LastBeatIndex = CurrentSequenceIndex;
					LastBeatTimestamp = FeedbackTime;
					for (int i = 0; i < Timing.Sequence.SequenceTracks.Count; i++)
					{
						if (Timing.Sequence.QuantizedSequence[i].Line[CurrentSequenceIndex].ID == Timing.TrackID)
						{
							CustomPlayFeedback(position, feedbacksIntensity);
						}
					}
					CurrentSequenceIndex++;
				}
				yield return null;
			}
			yield break;
		}
		while (FeedbackTime - timeStartedAt < Timing.Sequence.Length)
		{
			foreach (MMSequenceNote item in Timing.Sequence.OriginalSequence.Line)
			{
				if (item.ID == Timing.TrackID && item.Timestamp >= lastFrame && item.Timestamp <= FeedbackTime - timeStartedAt)
				{
					CustomPlayFeedback(position, feedbacksIntensity);
				}
			}
			lastFrame = FeedbackTime - timeStartedAt;
			yield return null;
		}
	}

	public virtual void Stop(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (_playCoroutine != null)
		{
			StopCoroutine(_playCoroutine);
		}
		if (_infinitePlayCoroutine != null)
		{
			StopCoroutine(_infinitePlayCoroutine);
		}
		if (_repeatedPlayCoroutine != null)
		{
			StopCoroutine(_repeatedPlayCoroutine);
		}
		if (_sequenceCoroutine != null)
		{
			StopCoroutine(_sequenceCoroutine);
		}
		_lastPlayTimestamp = 0f;
		_playsLeft = Timing.NumberOfRepeats + 1;
		if (Timing.InterruptsOnStop)
		{
			CustomStopFeedback(position, feedbacksIntensity);
		}
	}

	public virtual void ResetFeedback()
	{
		_playsLeft = Timing.NumberOfRepeats + 1;
		CustomReset();
	}

	public virtual void SetSequence(MMSequence newSequence)
	{
		Timing.Sequence = newSequence;
		if (!(Timing.Sequence != null))
		{
			return;
		}
		for (int i = 0; i < Timing.Sequence.SequenceTracks.Count; i++)
		{
			if (Timing.Sequence.SequenceTracks[i].ID == Timing.TrackID)
			{
				_sequenceTrackID = i;
			}
		}
	}

	public virtual void SetDelayBetweenRepeats(float delay)
	{
		Timing.DelayBetweenRepeats = delay;
	}

	public virtual void SetInitialDelay(float delay)
	{
		Timing.InitialDelay = delay;
	}

	protected virtual float ApplyDirection(float normalizedTime)
	{
		if (!NormalPlayDirection)
		{
			return 1f - normalizedTime;
		}
		return normalizedTime;
	}

	protected virtual float ApplyTimeMultiplier(float duration)
	{
		if (_isHostMMFeedbacksNotNull)
		{
			return _hostMMFeedbacks.ApplyTimeMultiplier(duration);
		}
		return duration;
	}

	protected virtual void CustomInitialization(GameObject owner)
	{
	}

	protected abstract void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f);

	protected virtual void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
	}

	protected virtual void CustomReset()
	{
	}
}
