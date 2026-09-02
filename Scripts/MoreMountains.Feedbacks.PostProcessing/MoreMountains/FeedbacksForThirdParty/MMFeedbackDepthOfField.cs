using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackHelp("This feedback allows you to control depth of field focus distance, aperture and focal length over time. It requires you have in your scene an object with a PostProcessVolume with Depth of Field active, and a MMDepthOfFieldShaker component.")]
[FeedbackPath("PostProcess/Depth Of Field")]
public class MMFeedbackDepthOfField : MMFeedback
{
	public static bool FeedbackTypeAuthorized = true;

	[Header("Depth Of Field")]
	[Tooltip("the channel to emit on")]
	public int Channel;

	[Tooltip("the duration of the shake, in seconds")]
	public float ShakeDuration = 2f;

	[Tooltip("whether or not to add to the initial values")]
	public bool RelativeValues = true;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[Header("Focus Distance")]
	[Tooltip("the curve used to animate the focus distance value on")]
	public AnimationCurve ShakeFocusDistance = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapFocusDistanceZero;

	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapFocusDistanceOne = 3f;

	[Header("Aperture")]
	[Tooltip("the curve used to animate the aperture value on")]
	public AnimationCurve ShakeAperture = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(0.1f, 32f)]
	public float RemapApertureZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(0.1f, 32f)]
	public float RemapApertureOne;

	[Header("Focal Length")]
	[Tooltip("the curve used to animate the focal length value on")]
	public AnimationCurve ShakeFocalLength = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(0f, 300f)]
	public float RemapFocalLengthZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(0f, 300f)]
	public float RemapFocalLengthOne;

	public override float FeedbackDuration
	{
		get
		{
			return ApplyTimeMultiplier(ShakeDuration);
		}
		set
		{
			ShakeDuration = value;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			float feedbacksIntensity2 = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
			MMDepthOfFieldShakeEvent.Trigger(ShakeFocusDistance, FeedbackDuration, RemapFocusDistanceZero, RemapFocusDistanceOne, ShakeAperture, RemapApertureZero, RemapApertureOne, ShakeFocalLength, RemapFocalLengthZero, RemapFocalLengthOne, RelativeValues, feedbacksIntensity2, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMDepthOfFieldShakeEvent.Trigger(ShakeFocusDistance, FeedbackDuration, RemapFocusDistanceZero, RemapFocusDistanceOne, ShakeAperture, RemapApertureZero, RemapApertureOne, ShakeFocalLength, RemapFocalLengthZero, RemapFocalLengthOne, RelativeValues, 1f, 0, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
