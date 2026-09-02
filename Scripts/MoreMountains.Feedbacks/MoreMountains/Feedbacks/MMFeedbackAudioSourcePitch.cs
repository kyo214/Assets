using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackPath("Audio/AudioSource Pitch")]
[FeedbackHelp("This feedback lets you control the pitch of a target AudioSource over time.")]
public class MMFeedbackAudioSourcePitch : MMFeedback
{
	public static bool FeedbackTypeAuthorized = true;

	[Header("Pitch Feedback")]
	[Tooltip("the channel to emit on")]
	public int Channel;

	[Tooltip("the duration of the shake, in seconds")]
	public float Duration = 2f;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[Header("Pitch")]
	[Tooltip("whether or not to add to the initial value")]
	public bool RelativePitch;

	[Tooltip("the curve used to animate the intensity value on")]
	public AnimationCurve PitchTween = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.5f, 0f), new Keyframe(1f, 1f));

	[Range(-3f, 3f)]
	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapPitchZero;

	[Range(-3f, 3f)]
	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapPitchOne = 1f;

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
			MMAudioSourcePitchShakeEvent.Trigger(PitchTween, FeedbackDuration, RemapPitchZero, RemapPitchOne, RelativePitch, feedbacksIntensity2, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMAudioSourcePitchShakeEvent.Trigger(PitchTween, FeedbackDuration, RemapPitchZero, RemapPitchOne, RelativePitch, 1f, 0, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
