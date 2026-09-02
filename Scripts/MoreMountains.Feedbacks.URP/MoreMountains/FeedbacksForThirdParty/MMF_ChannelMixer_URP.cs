using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackHelp("This feedback allows you to control bloom intensity and threshold over time. It requires you have in your scene an object with a Volume with Bloom active, and a MMBloomShaker_URP component.")]
[FeedbackPath("PostProcess/Channel Mixer URP")]
public class MMF_ChannelMixer_URP : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Channel Mixer", true, 41, false, false)]
	[Tooltip("the duration of the shake, in seconds")]
	public float ShakeDuration = 1f;

	[Tooltip("whether or not to add to the initial intensity")]
	public bool RelativeIntensity = true;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[MMFInspectorGroup("Red", true, 42, false, false)]
	[Tooltip("the curve used to animate the red value on")]
	public AnimationCurve ShakeRed = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-200f, 200f)]
	public float RemapRedZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-200f, 200f)]
	public float RemapRedOne = -200f;

	[MMFInspectorGroup("Green", true, 43, false, false)]
	[Tooltip("the curve used to animate the green value on")]
	public AnimationCurve ShakeGreen = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-200f, 200f)]
	public float RemapGreenZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-200f, 200f)]
	public float RemapGreenOne = 200f;

	[MMFInspectorGroup("Blue", true, 44, false, false)]
	[Tooltip("the curve used to animate the blue value on")]
	public AnimationCurve ShakeBlue = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-200f, 200f)]
	public float RemapBlueZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-200f, 200f)]
	public float RemapBlueOne = 200f;

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
			float attenuation = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
			MMChannelMixerShakeEvent_URP.Trigger(ShakeRed, RemapRedZero, RemapRedOne, ShakeGreen, RemapGreenZero, RemapGreenOne, ShakeBlue, RemapBlueZero, RemapBlueOne, FeedbackDuration, RelativeIntensity, attenuation, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMChannelMixerShakeEvent_URP.Trigger(ShakeRed, RemapRedZero, RemapRedOne, ShakeGreen, RemapGreenZero, RemapGreenOne, ShakeBlue, RemapBlueZero, RemapBlueOne, FeedbackDuration, RelativeIntensity, 1f, Channel, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
