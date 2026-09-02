using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("More Mountains/Feedbacks/Shakers/PostProcessing/MMChannelMixerShaker_HDRP")]
public class MMChannelMixerShaker_HDRP : MMShaker
{
	public bool RelativeValues = true;

	[Header("Red")]
	[Tooltip("the curve used to animate the red value on")]
	public AnimationCurve ShakeRed = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-200f, 200f)]
	public float RemapRedZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-200f, 200f)]
	public float RemapRedOne = 200f;

	[Header("Green")]
	[Tooltip("the curve used to animate the green value on")]
	public AnimationCurve ShakeGreen = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-200f, 200f)]
	public float RemapGreenZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-200f, 200f)]
	public float RemapGreenOne = 200f;

	[Header("Blue")]
	[Tooltip("the curve used to animate the blue value on")]
	public AnimationCurve ShakeBlue = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-200f, 200f)]
	public float RemapBlueZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-200f, 200f)]
	public float RemapBlueOne = 200f;
}
