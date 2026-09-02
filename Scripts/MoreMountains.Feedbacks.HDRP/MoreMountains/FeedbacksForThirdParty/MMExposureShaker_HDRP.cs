using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMExposureShaker_HDRP")]
public class MMExposureShaker_HDRP : MMShaker
{
	[Header("Intensity")]
	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeIntensity;

	[Tooltip("the curve used to animate the intensity value on")]
	public AnimationCurve ShakeFixedExposure = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapFixedExposureZero = 8.5f;

	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapFixedExposureOne = 6f;
}
