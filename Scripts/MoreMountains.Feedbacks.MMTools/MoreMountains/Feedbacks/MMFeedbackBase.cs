using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

public abstract class MMFeedbackBase : MMFeedback
{
	public enum Modes
	{
		OverTime = 0,
		Instant = 1
	}

	[Header("Mode")]
	[Tooltip("whether the feedback should affect the target property instantly or over a period of time")]
	public Modes Mode;

	[Tooltip("how long the target property should change over time")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float Duration = 0.2f;

	[Tooltip("whether or not that target property should be turned off on start")]
	public bool StartsOff;

	[Tooltip("whether or not the values should be relative or not")]
	public bool RelativeValues = true;

	[Tooltip("if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")]
	public bool AllowAdditivePlays;

	[Tooltip("if this is true, the target object will be disabled on stop")]
	public bool DisableOnStop;

	protected List<MMFeedbackBaseTarget> _targets;

	protected Coroutine _coroutine;

	public override float FeedbackDuration
	{
		get
		{
			if (Mode != Modes.Instant)
			{
				return ApplyTimeMultiplier(Duration);
			}
			return 0f;
		}
		set
		{
			if (Mode != Modes.Instant)
			{
				Duration = value;
			}
		}
	}

	protected override void CustomInitialization(GameObject owner)
	{
		base.CustomInitialization(owner);
		PrepareTargets();
		if (Active && StartsOff)
		{
			Turn(status: false);
		}
	}

	protected virtual void PrepareTargets()
	{
		_targets = new List<MMFeedbackBaseTarget>();
		FillTargets();
		InitializeTargets();
	}

	protected virtual void OnValidate()
	{
		PrepareTargets();
	}

	protected abstract void FillTargets();

	protected virtual void InitializeTargets()
	{
		if (_targets.Count == 0)
		{
			return;
		}
		foreach (MMFeedbackBaseTarget target in _targets)
		{
			target.Target.Initialization(base.gameObject);
			target.InitialLevel = target.Target.Level;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active)
		{
			return;
		}
		Turn(status: true);
		switch (Mode)
		{
		case Modes.Instant:
			Instant();
			break;
		case Modes.OverTime:
			if (AllowAdditivePlays || _coroutine == null)
			{
				_coroutine = StartCoroutine(UpdateValueSequence(feedbacksIntensity));
			}
			break;
		}
	}

	protected virtual void Instant()
	{
		if (_targets.Count == 0)
		{
			return;
		}
		foreach (MMFeedbackBaseTarget target in _targets)
		{
			target.Target.SetLevel(target.InstantLevel);
		}
	}

	protected virtual IEnumerator UpdateValueSequence(float feedbacksIntensity)
	{
		float journey = (NormalPlayDirection ? 0f : FeedbackDuration);
		IsPlaying = true;
		while (journey >= 0f && journey <= FeedbackDuration && FeedbackDuration > 0f)
		{
			float time = MMFeedbacksHelpers.Remap(journey, 0f, FeedbackDuration, 0f, 1f);
			SetValues(time, feedbacksIntensity);
			journey += (NormalPlayDirection ? base.FeedbackDeltaTime : (0f - base.FeedbackDeltaTime));
			yield return null;
		}
		SetValues(FinalNormalizedTime, feedbacksIntensity);
		if (StartsOff)
		{
			Turn(status: false);
		}
		IsPlaying = false;
		_coroutine = null;
		yield return null;
	}

	protected virtual void SetValues(float time, float feedbacksIntensity)
	{
		if (_targets.Count == 0)
		{
			return;
		}
		float num = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
		foreach (MMFeedbackBaseTarget target in _targets)
		{
			float num2 = MMTween.Tween(time, 0f, 1f, target.RemapLevelZero, target.RemapLevelOne, target.LevelCurve);
			if (RelativeValues)
			{
				num2 += target.InitialLevel;
			}
			target.Target.SetLevel(num2 * num);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		base.CustomStopFeedback(position, feedbacksIntensity);
		if (Active)
		{
			if (_coroutine != null)
			{
				StopCoroutine(_coroutine);
				_coroutine = null;
			}
			IsPlaying = false;
			if (DisableOnStop)
			{
				Turn(status: false);
			}
		}
	}

	protected virtual void Turn(bool status)
	{
		if (_targets.Count == 0)
		{
			return;
		}
		foreach (MMFeedbackBaseTarget target in _targets)
		{
			if (target.Target.TargetComponent.gameObject != null)
			{
				target.Target.TargetComponent.gameObject.SetActive(status);
			}
		}
	}
}
