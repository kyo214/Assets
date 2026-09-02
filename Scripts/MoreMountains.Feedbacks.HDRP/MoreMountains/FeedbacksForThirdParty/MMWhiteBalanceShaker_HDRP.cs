using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMWhiteBalanceShaker_HDRP")]
public class MMWhiteBalanceShaker_HDRP : MMShaker
{
	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeValues = true;

	[Header("Temperature")]
	[Tooltip("the curve used to animate the temperature value on")]
	public AnimationCurve ShakeTemperature = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-100f, 100f)]
	public float RemapTemperatureZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-100f, 100f)]
	public float RemapTemperatureOne = 100f;

	[Header("Tint")]
	[Tooltip("the curve used to animate the tint value on")]
	public AnimationCurve ShakeTint = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-100f, 100f)]
	public float RemapTintZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-100f, 100f)]
	public float RemapTintOne = 100f;
}
