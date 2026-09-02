using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMPaniniProjectionShaker_HDRP")]
public class MMPaniniProjectionShaker_HDRP : MMShaker
{
	[Header("Distance")]
	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeDistance;

	[Tooltip("the curve used to animate the distance value on")]
	public AnimationCurve ShakeDistance = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(0f, 1f)]
	public float RemapDistanceZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(0f, 1f)]
	public float RemapDistanceOne = 1f;
}
