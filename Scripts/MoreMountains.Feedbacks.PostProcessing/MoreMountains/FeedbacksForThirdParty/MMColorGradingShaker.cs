using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMColorGradingShaker")]
public class MMColorGradingShaker : MMShaker
{
	public bool RelativeValues = true;

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
}
