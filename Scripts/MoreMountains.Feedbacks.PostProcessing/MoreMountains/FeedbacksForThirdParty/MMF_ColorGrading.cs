using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackPath("PostProcess/Color Grading")]
[FeedbackHelp("This feedback allows you to control color grading post exposure, hue shift, saturation and contrast over time. It requires you have in your scene an object with a PostProcessVolume with Color Grading active, and a MMColorGradingShaker component.")]
public class MMF_ColorGrading : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Color Grading", true, 46, false, false)]
	[Tooltip("the duration of the shake, in seconds")]
	public float ShakeDuration = 1f;

	[Tooltip("whether or not to add to the initial intensity")]
	public bool RelativeIntensity = true;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[MMFInspectorGroup("Post Exposure", true, 47, false, false)]
	[Tooltip("the curve used to animate the focus distance value on")]
	public AnimationCurve ShakePostExposure = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapPostExposureZero;

	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapPostExposureOne = 1f;

	[MMFInspectorGroup("Hue Shift", true, 48, false, false)]
	[Tooltip("the curve used to animate the aperture value on")]
	public AnimationCurve ShakeHueShift = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-180f, 180f)]
	public float RemapHueShiftZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-180f, 180f)]
	public float RemapHueShiftOne = 180f;

	[MMFInspectorGroup("Saturation", true, 49, false, false)]
	[Tooltip("the curve used to animate the focal length value on")]
	public AnimationCurve ShakeSaturation = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-100f, 100f)]
	public float RemapSaturationZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-100f, 100f)]
	public float RemapSaturationOne = 100f;

	[MMFInspectorGroup("Contrast", true, 50, false, false)]
	[Tooltip("the curve used to animate the focal length value on")]
	public AnimationCurve ShakeContrast = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-100f, 100f)]
	public float RemapContrastZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-100f, 100f)]
	public float RemapContrastOne = 100f;

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
			MMColorGradingShakeEvent.Trigger(ShakePostExposure, RemapPostExposureZero, RemapPostExposureOne, ShakeHueShift, RemapHueShiftZero, RemapHueShiftOne, ShakeSaturation, RemapSaturationZero, RemapSaturationOne, ShakeContrast, RemapContrastZero, RemapContrastOne, FeedbackDuration, RelativeIntensity, feedbacksIntensity2, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMColorGradingShakeEvent.Trigger(ShakePostExposure, RemapPostExposureZero, RemapPostExposureOne, ShakeHueShift, RemapHueShiftZero, RemapHueShiftOne, ShakeSaturation, RemapSaturationZero, RemapSaturationOne, ShakeContrast, RemapContrastZero, RemapContrastOne, FeedbackDuration, relativeValues: false, 1f, 0, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
