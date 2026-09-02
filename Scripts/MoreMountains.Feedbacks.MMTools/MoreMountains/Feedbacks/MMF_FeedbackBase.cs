using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

public abstract class MMF_FeedbackBase : MMF_Feedback
{
	public enum Modes
	{
		OverTime = 0,
		Instant = 1
	}

	[MMFInspectorGroup("Mode", true, 64, false, false)]
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

	[Tooltip("if this is true, this feedback will only play if its target is active in hierarchy")]
	public bool OnlyPlayIfTargetIsActive;

	protected List<MMF_FeedbackBaseTarget> _targets;

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

	protected override void CustomInitialization(MMF_Player owner)
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
		_targets = new List<MMF_FeedbackBaseTarget>();
		FillTargets();
		InitializeTargets();
	}

	public override void OnValidate()
	{
		base.OnValidate();
		PrepareTargets();
	}

	protected abstract void FillTargets();

	protected virtual void InitializeTargets()
	{
		if (_targets.Count == 0)
		{
			return;
		}
		foreach (MMF_FeedbackBaseTarget target in _targets)
		{
			target.Target.Initialization(Owner.gameObject);
			target.InitialLevel = target.Target.Level;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !CanPlay())
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
				_coroutine = Owner.StartCoroutine(UpdateValueSequence(feedbacksIntensity));
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
		foreach (MMF_FeedbackBaseTarget target in _targets)
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
			journey += (NormalPlayDirection ? FeedbackDeltaTime : (0f - FeedbackDeltaTime));
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
		foreach (MMF_FeedbackBaseTarget target in _targets)
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
				Owner.StopCoroutine(_coroutine);
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
		foreach (MMF_FeedbackBaseTarget target in _targets)
		{
			if (target.Target.TargetComponent.gameObject != null)
			{
				target.Target.TargetComponent.gameObject.SetActive(status);
			}
		}
	}

	protected virtual bool CanPlay()
	{
		if (_targets.Count == 0)
		{
			return false;
		}
		foreach (MMF_FeedbackBaseTarget target in _targets)
		{
			if (OnlyPlayIfTargetIsActive && !target.Target.TargetComponent.gameObject.activeInHierarchy)
			{
				return false;
			}
		}
		return true;
	}
}
