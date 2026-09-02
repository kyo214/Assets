using System.Collections;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback lets you control the color and intensity of a Light in your scene for a certain duration (or instantly).")]
[FeedbackPath("Light")]
public class MMF_Light : MMF_Feedback
{
	public enum Modes
	{
		OverTime = 0,
		Instant = 1,
		ShakerEvent = 2
	}

	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Light", true, 37, true, false)]
	[Tooltip("the light to affect when playing the feedback")]
	public Light BoundLight;

	[Tooltip("whether the feedback should affect the light instantly or over a period of time")]
	public Modes Mode;

	[Tooltip("how long the light should change over time")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public float Duration = 0.2f;

	[Tooltip("whether or not that light should be turned off on start")]
	public bool StartsOff = true;

	[Tooltip("whether or not the values should be relative or not")]
	public bool RelativeValues = true;

	[Tooltip("whether or not to reset shaker values after shake")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public bool ResetTargetValuesAfterShake = true;

	[Tooltip("whether or not to broadcast a range to only affect certain shakers")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public bool UseRange;

	[Tooltip("the range of the event, in units")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public float EventRange = 100f;

	[Tooltip("the transform to use to broadcast the event as origin point")]
	[MMFEnumCondition("Mode", new int[] { 2 })]
	public Transform EventOriginTransform;

	[Tooltip("if this is true, calling that feedback will trigger it, even if it's in progress. If it's false, it'll prevent any new Play until the current one is over")]
	public bool AllowAdditivePlays;

	[Tooltip("if this is true, the light will be disabled when this feedbacks is stopped")]
	public bool DisableOnStop = true;

	[Header("Color")]
	[Tooltip("whether or not to modify the color of the light")]
	public bool ModifyColor = true;

	[Tooltip("the colors to apply to the light over time")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public Gradient ColorOverTime;

	[Tooltip("the color to move to in instant mode")]
	[MMFEnumCondition("Mode", new int[] { 1, 2 })]
	public Color InstantColor;

	[Header("Intensity")]
	[Tooltip("the curve to tween the intensity on")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public AnimationCurve IntensityCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the intensity curve's 0 to")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public float RemapIntensityZero;

	[Tooltip("the value to remap the intensity curve's 1 to")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public float RemapIntensityOne = 1f;

	[Tooltip("the value to move the intensity to in instant mode")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public float InstantIntensity;

	[Header("Range")]
	[Tooltip("the range to apply to the light over time")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public AnimationCurve RangeCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the range curve's 0 to")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public float RemapRangeZero;

	[Tooltip("the value to remap the range curve's 0 to")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public float RemapRangeOne = 10f;

	[Tooltip("the value to move the intensity to in instant mode")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public float InstantRange;

	[Header("Shadow Strength")]
	[Tooltip("the range to apply to the light over time")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public AnimationCurve ShadowStrengthCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the shadow strength's curve's 0 to")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public float RemapShadowStrengthZero;

	[Tooltip("the value to remap the shadow strength's curve's 1 to")]
	[MMFEnumCondition("Mode", new int[] { 0, 2 })]
	public float RemapShadowStrengthOne = 1f;

	[Tooltip("the value to move the shadow strength to in instant mode")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public float InstantShadowStrength;

	protected float _initialRange;

	protected float _initialShadowStrength;

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
			Duration = value;
		}
	}

	public override bool HasChannel => true;

	protected override void CustomInitialization(MMF_Player owner)
	{
		base.CustomInitialization(owner);
		if (!(BoundLight == null))
		{
			_initialRange = BoundLight.range;
			_initialShadowStrength = BoundLight.shadowStrength;
			_initialIntensity = BoundLight.intensity;
			if (EventOriginTransform == null)
			{
				EventOriginTransform = owner.transform;
			}
			if (Active && StartsOff)
			{
				Turn(status: false);
			}
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized)
		{
			return;
		}
		float num = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
		Turn(status: true);
		switch (Mode)
		{
		case Modes.Instant:
			BoundLight.intensity = InstantIntensity * num;
			BoundLight.shadowStrength = InstantShadowStrength;
			BoundLight.range = InstantRange;
			if (ModifyColor)
			{
				BoundLight.color = InstantColor;
			}
			break;
		case Modes.OverTime:
			if (AllowAdditivePlays || _coroutine == null)
			{
				_coroutine = Owner.StartCoroutine(LightSequence(num));
			}
			break;
		case Modes.ShakerEvent:
			MMLightShakeEvent.Trigger(FeedbackDuration, RelativeValues, ModifyColor, ColorOverTime, IntensityCurve, RemapIntensityZero, RemapIntensityOne, RangeCurve, RemapRangeZero * num, RemapRangeOne * num, ShadowStrengthCurve, RemapShadowStrengthZero, RemapShadowStrengthOne, feedbacksIntensity, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, UseRange, EventRange, EventOriginTransform.position);
			break;
		}
	}

	protected virtual IEnumerator LightSequence(float intensityMultiplier)
	{
		IsPlaying = true;
		float journey = (NormalPlayDirection ? 0f : FeedbackDuration);
		while (journey >= 0f && journey <= FeedbackDuration && FeedbackDuration > 0f)
		{
			float time = MMFeedbacksHelpers.Remap(journey, 0f, FeedbackDuration, 0f, 1f);
			SetLightValues(time, intensityMultiplier);
			journey += (NormalPlayDirection ? FeedbackDeltaTime : (0f - FeedbackDeltaTime));
			yield return null;
		}
		SetLightValues(FinalNormalizedTime, intensityMultiplier);
		if (StartsOff)
		{
			Turn(status: false);
		}
		IsPlaying = false;
		_coroutine = null;
		yield return null;
	}

	protected virtual void SetLightValues(float time, float intensityMultiplier)
	{
		float num = MMFeedbacksHelpers.Remap(IntensityCurve.Evaluate(time), 0f, 1f, RemapIntensityZero, RemapIntensityOne);
		float num2 = MMFeedbacksHelpers.Remap(RangeCurve.Evaluate(time), 0f, 1f, RemapRangeZero, RemapRangeOne);
		float num3 = MMFeedbacksHelpers.Remap(ShadowStrengthCurve.Evaluate(time), 0f, 1f, RemapShadowStrengthZero, RemapShadowStrengthOne);
		if (RelativeValues)
		{
			num += _initialIntensity;
			num3 += _initialShadowStrength;
			num2 += _initialRange;
		}
		BoundLight.intensity = num * intensityMultiplier;
		BoundLight.range = num2;
		BoundLight.shadowStrength = Mathf.Clamp01(num3);
		if (ModifyColor)
		{
			BoundLight.color = ColorOverTime.Evaluate(time);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			IsPlaying = false;
			if (Active && _coroutine != null)
			{
				Owner.StopCoroutine(_coroutine);
				_coroutine = null;
			}
			if (Active && DisableOnStop)
			{
				Turn(status: false);
			}
		}
	}

	protected virtual void Turn(bool status)
	{
		BoundLight.gameObject.SetActive(status);
		BoundLight.enabled = status;
	}
}
