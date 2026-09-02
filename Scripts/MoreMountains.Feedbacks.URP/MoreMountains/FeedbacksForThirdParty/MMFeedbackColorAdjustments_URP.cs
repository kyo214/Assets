using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackPath("PostProcess/Color Adjustments URP")]
[FeedbackHelp("This feedback allows you to control color adjustments' post exposure, hue shift, saturation and contrast over time. It requires you have in your scene an object with a Volume with Color Adjustments active, and a MMColorAdjustmentsShaker_URP component.")]
public class MMFeedbackColorAdjustments_URP : MMFeedback
{
	public static bool FeedbackTypeAuthorized = true;

	[Header("Color Grading")]
	[Tooltip("the channel to emit on")]
	public int Channel;

	[Tooltip("the duration of the shake, in seconds")]
	public float ShakeDuration = 1f;

	[Tooltip("whether or not to add to the initial intensity")]
	public bool RelativeIntensity = true;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[Header("Post Exposure")]
	[Tooltip("the curve used to animate the focus distance value on")]
	public AnimationCurve ShakePostExposure = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapPostExposureZero;

	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapPostExposureOne = 1f;

	[Header("Hue Shift")]
	[Tooltip("the curve used to animate the aperture value on")]
	public AnimationCurve ShakeHueShift = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-180f, 180f)]
	public float RemapHueShiftZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-180f, 180f)]
	public float RemapHueShiftOne = 180f;

	[Header("Saturation")]
	[Tooltip("the curve used to animate the focal length value on")]
	public AnimationCurve ShakeSaturation = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-100f, 100f)]
	public float RemapSaturationZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-100f, 100f)]
	public float RemapSaturationOne = 100f;

	[Header("Contrast")]
	[Tooltip("the curve used to animate the focal length value on")]
	public AnimationCurve ShakeContrast = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-100f, 100f)]
	public float RemapContrastZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-100f, 100f)]
	public float RemapContrastOne = 100f;

	[Header("Color Filter")]
	[Tooltip("the selected color filter mode :None : nothing will happen,gradient : evaluates the color over time on that gradient, from left to right,interpolate : lerps from the current color to the destination one ")]
	public MMColorAdjustmentsShaker_URP.ColorFilterModes ColorFilterMode;

	[Tooltip("the gradient to use to animate the color filter over time")]
	[MMFEnumCondition("ColorFilterMode", new int[] { 1 })]
	[GradientUsage(true)]
	public Gradient ColorFilterGradient;

	[Tooltip("the destination color when in interpolate mode")]
	[MMFEnumCondition("ColorFilterMode", new int[] { 2 })]
	public Color ColorFilterDestination = Color.yellow;

	[Tooltip("the curve to use when interpolating towards the destination color")]
	[MMFEnumCondition("ColorFilterMode", new int[] { 2 })]
	public AnimationCurve ColorFilterCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

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
			float attenuation = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
			MMColorAdjustmentsShakeEvent_URP.Trigger(ShakePostExposure, RemapPostExposureZero, RemapPostExposureOne, ShakeHueShift, RemapHueShiftZero, RemapHueShiftOne, ShakeSaturation, RemapSaturationZero, RemapSaturationOne, ShakeContrast, RemapContrastZero, RemapContrastOne, ColorFilterMode, ColorFilterGradient, ColorFilterDestination, ColorFilterCurve, FeedbackDuration, RelativeIntensity, attenuation, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMColorAdjustmentsShakeEvent_URP.Trigger(ShakePostExposure, RemapPostExposureZero, RemapPostExposureOne, ShakeHueShift, RemapHueShiftZero, RemapHueShiftOne, ShakeSaturation, RemapSaturationZero, RemapSaturationOne, ShakeContrast, RemapContrastZero, RemapContrastOne, ColorFilterMode, ColorFilterGradient, ColorFilterDestination, ColorFilterCurve, FeedbackDuration, RelativeIntensity, 1f, Channel, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
