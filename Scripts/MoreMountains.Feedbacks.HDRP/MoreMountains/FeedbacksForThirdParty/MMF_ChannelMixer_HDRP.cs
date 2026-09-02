using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackPath("PostProcess/Channel Mixer HDRP")]
[FeedbackHelp("This feedback allows you to control channel mixer's red, green and blue over time.It requires you have in your scene an object with a Volumewith Channel Mixer active, and a MM Channel Mixer HDRP component.")]
public class MMF_ChannelMixer_HDRP : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Color Grading", true, 10, false, false)]
	[Tooltip("the duration of the shake, in seconds")]
	public float ShakeDuration = 1f;

	[Tooltip("whether or not to add to the initial intensity")]
	public bool RelativeIntensity = true;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[MMFInspectorGroup("Red", true, 13, false, false)]
	[Tooltip("the curve used to animate the red value on")]
	public AnimationCurve ShakeRed = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-200f, 200f)]
	public float RemapRedZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-200f, 200f)]
	public float RemapRedOne = 200f;

	[MMFInspectorGroup("Green", true, 12, false, false)]
	[Tooltip("the curve used to animate the green value on")]
	public AnimationCurve ShakeGreen = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-200f, 200f)]
	public float RemapGreenZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-200f, 200f)]
	public float RemapGreenOne = 200f;

	[MMFInspectorGroup("Blue", true, 11, false, false)]
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
			MMChannelMixerShakeEvent_HDRP.Trigger(ShakeRed, RemapRedZero, RemapRedOne, ShakeGreen, RemapGreenZero, RemapGreenOne, ShakeBlue, RemapBlueZero, RemapBlueOne, FeedbackDuration, RelativeIntensity, attenuation, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMChannelMixerShakeEvent_HDRP.Trigger(ShakeRed, RemapRedZero, RemapRedOne, ShakeGreen, RemapGreenZero, RemapGreenOne, ShakeBlue, RemapBlueZero, RemapBlueOne, FeedbackDuration, RelativeIntensity, 1f, Channel, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
