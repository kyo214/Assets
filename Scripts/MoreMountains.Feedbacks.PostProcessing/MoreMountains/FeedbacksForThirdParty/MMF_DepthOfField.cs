using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackHelp("This feedback allows you to control depth of field focus distance, aperture and focal length over time. It requires you have in your scene an object with a PostProcessVolume with Depth of Field active, and a MMDepthOfFieldShaker component.")]
[FeedbackPath("PostProcess/Depth Of Field")]
public class MMF_DepthOfField : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Depth Of Field", true, 51, false, false)]
	[Tooltip("the duration of the shake, in seconds")]
	public float ShakeDuration = 2f;

	[Tooltip("whether or not to add to the initial values")]
	public bool RelativeValues = true;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[MMFInspectorGroup("Focus Distance", true, 52, false, false)]
	[Tooltip("the curve used to animate the focus distance value on")]
	public AnimationCurve ShakeFocusDistance = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapFocusDistanceZero = 4f;

	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapFocusDistanceOne = 50f;

	[MMFInspectorGroup("Aperture", true, 53, false, false)]
	[Tooltip("the curve used to animate the aperture value on")]
	public AnimationCurve ShakeAperture = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(0.1f, 32f)]
	public float RemapApertureZero = 0.6f;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(0.1f, 32f)]
	public float RemapApertureOne = 0.2f;

	[MMFInspectorGroup("Focal Length", true, 54, false, false)]
	[Tooltip("the curve used to animate the focal length value on")]
	public AnimationCurve ShakeFocalLength = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(0f, 300f)]
	public float RemapFocalLengthZero = 27.5f;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(0f, 300f)]
	public float RemapFocalLengthOne = 27.5f;

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

	public override bool HasChannel => true;

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
