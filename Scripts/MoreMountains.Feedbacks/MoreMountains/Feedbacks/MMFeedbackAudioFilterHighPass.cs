using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackPath("Audio/Audio Filter High Pass")]
[FeedbackHelp("This feedback lets you control a high pass audio filter over time. You'll need a MMAudioFilterHighPassShaker on your filter.")]
public class MMFeedbackAudioFilterHighPass : MMFeedback
{
	public static bool FeedbackTypeAuthorized = true;

	[Header("High Pass Feedback")]
	[Tooltip("the channel to emit on")]
	public int Channel;

	[Tooltip("the duration of the shake, in seconds")]
	public float Duration = 2f;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[Header("High Pass")]
	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeHighPass;

	[Tooltip("the curve used to animate the intensity value on")]
	public AnimationCurve ShakeHighPass = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Range(10f, 22000f)]
	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapHighPassZero;

	[Range(10f, 22000f)]
	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapHighPassOne = 10000f;

	public override float FeedbackDuration
	{
		get
		{
			return ApplyTimeMultiplier(Duration);
		}
		set
		{
			Duration = value;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			float feedbacksIntensity2 = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
			MMAudioFilterHighPassShakeEvent.Trigger(ShakeHighPass, FeedbackDuration, RemapHighPassZero, RemapHighPassOne, RelativeHighPass, feedbacksIntensity2, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMAudioFilterHighPassShakeEvent.Trigger(ShakeHighPass, FeedbackDuration, RemapHighPassZero, RemapHighPassOne, relativeHighPass: false, 1f, 0, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
