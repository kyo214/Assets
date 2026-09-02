using System.Collections;
using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you target (almost) any property, on any object in your scene. It also works on scriptable objects. Drag an object, select a property, and setup your feedback to update that property over time.")]
[FeedbackPath("GameObject/Property")]
public class MMF_Property : MMF_Feedback
{
	public enum Modes
	{
		OverTime = 0,
		Instant = 1
	}

	[Header("Target Property")]
	[Tooltip("the receiver to write the level to")]
	public MMPropertyReceiver Target;

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

	[Header("Level")]
	[Tooltip("the curve to tween the intensity on")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public MMTweenType LevelCurve = new MMTweenType(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(1f, 0f)));

	[Tooltip("the value to remap the intensity curve's 0 to")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float RemapLevelZero;

	[Tooltip("the value to remap the intensity curve's 1 to")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float RemapLevelOne = 1f;

	[Tooltip("the value to move the intensity to in instant mode")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public float InstantLevel;

	protected float _initialIntensity;

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
		Target.Initialization(Owner.gameObject);
		_initialIntensity = Target.Level;
		if (Active && StartsOff)
		{
			Turn(status: false);
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active)
		{
			return;
		}
		Turn(status: true);
		float intensityMultiplier = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
		switch (Mode)
		{
		case Modes.Instant:
			Target.SetLevel(InstantLevel);
			break;
		case Modes.OverTime:
			if (AllowAdditivePlays || _coroutine == null)
			{
				_coroutine = Owner.StartCoroutine(UpdateValueSequence(intensityMultiplier));
			}
			break;
		}
	}

	protected virtual IEnumerator UpdateValueSequence(float intensityMultiplier)
	{
		float journey = (NormalPlayDirection ? 0f : FeedbackDuration);
		while (journey >= 0f && journey <= FeedbackDuration && FeedbackDuration > 0f)
		{
			float time = MMFeedbacksHelpers.Remap(journey, 0f, FeedbackDuration, 0f, 1f);
			SetValues(time, intensityMultiplier);
			journey += (NormalPlayDirection ? FeedbackDeltaTime : (0f - FeedbackDeltaTime));
			yield return null;
		}
		SetValues(FinalNormalizedTime, intensityMultiplier);
		if (StartsOff)
		{
			Turn(status: false);
		}
		_coroutine = null;
		yield return null;
	}

	protected virtual void SetValues(float time, float intensityMultiplier)
	{
		float num = MMTween.Tween(time, 0f, 1f, RemapLevelZero, RemapLevelOne, LevelCurve);
		num *= intensityMultiplier;
		if (RelativeValues)
		{
			num += _initialIntensity;
		}
		Target.SetLevel(num);
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
				SetValues(_initialIntensity, 1f);
			}
			if (StartsOff)
			{
				Turn(status: false);
			}
		}
	}

	protected virtual void Turn(bool status)
	{
		if (Target.TargetComponent.gameObject != null)
		{
			Target.TargetComponent.gameObject.SetActive(status);
		}
	}
}
