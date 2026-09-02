using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMDepthOfFieldShaker")]
public class MMDepthOfFieldShaker : MMShaker
{
	public bool RelativeValues = true;

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
}
