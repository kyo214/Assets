using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback lets you trigger a one time play on a target FloatController.")]
[FeedbackPath("GameObject/FloatController")]
public class MMFeedbackFloatController : MMFeedback
{
	public enum Modes
	{
		OneTime = 0,
		ToDestination = 1
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Float Controller")]
	[Tooltip("the mode this controller is in")]
	public Modes Mode;

	[Tooltip("the float controller to trigger a one time play on")]
	public FloatController TargetFloatController;

	[Tooltip("whether this should revert to original at the end")]
	public bool RevertToInitialValueAfterEnd;

	[Tooltip("the duration of the One Time shake")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float OneTimeDuration = 1f;

	[Tooltip("the amplitude of the One Time shake (this will be multiplied by the curve's height)")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float OneTimeAmplitude = 1f;

	[Tooltip("the low value to remap the normalized curve value to")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float OneTimeRemapMin;

	[Tooltip("the high value to remap the normalized curve value to")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public float OneTimeRemapMax = 1f;

	[Tooltip("the curve to apply to the one time shake")]
	[MMFEnumCondition("Mode", new int[] { 0 })]
	public AnimationCurve OneTimeCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to move this float controller to")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public float ToDestinationValue = 1f;

	[Tooltip("the duration over which to move the value")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public float ToDestinationDuration = 1f;

	[Tooltip("the curve over which to move the value in ToDestination mode")]
	[MMFEnumCondition("Mode", new int[] { 1 })]
	public AnimationCurve ToDestinationCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	protected float _oneTimeDurationStorage;

	protected float _oneTimeAmplitudeStorage;

	protected float _oneTimeRemapMinStorage;

	protected float _oneTimeRemapMaxStorage;

	protected AnimationCurve _oneTimeCurveStorage;

	protected float _toDestinationValueStorage;

	protected float _toDestinationDurationStorage;

	protected AnimationCurve _toDestinationCurveStorage;

	protected bool _revertToInitialValueAfterEndStorage;

	public override float FeedbackDuration
	{
		get
		{
			if (Mode != Modes.OneTime)
			{
				return ApplyTimeMultiplier(ToDestinationDuration);
			}
			return ApplyTimeMultiplier(OneTimeDuration);
		}
		set
		{
			OneTimeDuration = value;
			ToDestinationDuration = value;
		}
	}

	protected override void CustomInitialization(GameObject owner)
	{
		if (Active && TargetFloatController != null)
		{
			_oneTimeDurationStorage = TargetFloatController.OneTimeDuration;
			_oneTimeAmplitudeStorage = TargetFloatController.OneTimeAmplitude;
			_oneTimeCurveStorage = TargetFloatController.OneTimeCurve;
			_oneTimeRemapMinStorage = TargetFloatController.OneTimeRemapMin;
			_oneTimeRemapMaxStorage = TargetFloatController.OneTimeRemapMax;
			_toDestinationCurveStorage = TargetFloatController.ToDestinationCurve;
			_toDestinationDurationStorage = TargetFloatController.ToDestinationDuration;
			_toDestinationValueStorage = TargetFloatController.ToDestinationValue;
			_revertToInitialValueAfterEndStorage = TargetFloatController.RevertToInitialValueAfterEnd;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (!Active || !FeedbackTypeAuthorized || TargetFloatController == null)
		{
			return;
		}
		float num = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
		TargetFloatController.RevertToInitialValueAfterEnd = RevertToInitialValueAfterEnd;
		if (Mode == Modes.OneTime)
		{
			TargetFloatController.OneTimeDuration = FeedbackDuration;
			TargetFloatController.OneTimeAmplitude = OneTimeAmplitude;
			TargetFloatController.OneTimeCurve = OneTimeCurve;
			if (NormalPlayDirection)
			{
				TargetFloatController.OneTimeRemapMin = OneTimeRemapMin * num;
				TargetFloatController.OneTimeRemapMax = OneTimeRemapMax * num;
			}
			else
			{
				TargetFloatController.OneTimeRemapMin = OneTimeRemapMax * num;
				TargetFloatController.OneTimeRemapMax = OneTimeRemapMin * num;
			}
			TargetFloatController.OneTime();
		}
		if (Mode == Modes.ToDestination)
		{
			TargetFloatController.ToDestinationCurve = ToDestinationCurve;
			TargetFloatController.ToDestinationDuration = FeedbackDuration;
			TargetFloatController.ToDestinationValue = ToDestinationValue;
			TargetFloatController.ToDestination();
		}
	}

	protected override void CustomReset()
	{
		base.CustomReset();
		if (Active && FeedbackTypeAuthorized && TargetFloatController != null)
		{
			TargetFloatController.OneTimeDuration = _oneTimeDurationStorage;
			TargetFloatController.OneTimeAmplitude = _oneTimeAmplitudeStorage;
			TargetFloatController.OneTimeCurve = _oneTimeCurveStorage;
			TargetFloatController.OneTimeRemapMin = _oneTimeRemapMinStorage;
			TargetFloatController.OneTimeRemapMax = _oneTimeRemapMaxStorage;
			TargetFloatController.ToDestinationCurve = _toDestinationCurveStorage;
			TargetFloatController.ToDestinationDuration = _toDestinationDurationStorage;
			TargetFloatController.ToDestinationValue = _toDestinationValueStorage;
			TargetFloatController.RevertToInitialValueAfterEnd = _revertToInitialValueAfterEndStorage;
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && TargetFloatController != null)
		{
			TargetFloatController.Stop();
		}
	}
}
