using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMLensDistortionShaker_HDRP")]
public class MMLensDistortionShaker_HDRP : MMShaker
{
	[Header("Intensity")]
	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeIntensity;

	[Tooltip("the curve used to animate the intensity value on")]
	public AnimationCurve ShakeIntensity = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.2f, 1f), new Keyframe(0.25f, -1f), new Keyframe(0.35f, 0.7f), new Keyframe(0.4f, -0.7f), new Keyframe(0.6f, 0.3f), new Keyframe(0.65f, -0.3f), new Keyframe(0.8f, 0.1f), new Keyframe(0.85f, -0.1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-100f, 100f)]
	public float RemapIntensityZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-100f, 100f)]
	public float RemapIntensityOne = 0.5f;
}
