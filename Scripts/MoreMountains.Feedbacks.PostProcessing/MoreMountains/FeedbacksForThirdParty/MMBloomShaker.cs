using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMBloomShaker")]
public class MMBloomShaker : MMShaker
{
	public bool RelativeValues = true;

	[Header("Intensity")]
	[Tooltip("the curve used to animate the intensity value on")]
	public AnimationCurve ShakeIntensity = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapIntensityZero;

	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapIntensityOne = 10f;

	[Header("Threshold")]
	[Tooltip("the curve used to animate the threshold value on")]
	public AnimationCurve ShakeThreshold = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapThresholdZero;

	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapThresholdOne;
}
