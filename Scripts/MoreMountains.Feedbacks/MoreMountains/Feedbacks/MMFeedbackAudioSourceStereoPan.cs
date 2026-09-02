using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackPath("Audio/AudioSource Stereo Pan")]
[FeedbackHelp("This feedback lets you control the stereo pan of a target AudioSource over time.")]
public class MMFeedbackAudioSourceStereoPan : MMFeedback
{
	public static bool FeedbackTypeAuthorized = true;

	[Header("StereoPan Feedback")]
	[Tooltip("the channel to emit on")]
	public int Channel;

	[Tooltip("the duration of the shake, in seconds")]
	public float Duration = 2f;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[Header("StereoPan")]
	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeStereoPan;

	[Tooltip("the curve used to animate the intensity value on")]
	public AnimationCurve ShakeStereoPan = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.3f, 1f), new Keyframe(0.6f, -1f), new Keyframe(1f, 0f));

	[Range(-1f, 1f)]
	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapStereoPanZero;

	[Range(-1f, 1f)]
	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapStereoPanOne = 1f;

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
			MMAudioSourceStereoPanShakeEvent.Trigger(ShakeStereoPan, FeedbackDuration, RemapStereoPanZero, RemapStereoPanOne, RelativeStereoPan, feedbacksIntensity2, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMAudioSourceStereoPanShakeEvent.Trigger(ShakeStereoPan, FeedbackDuration, RemapStereoPanZero, RemapStereoPanOne, relativeStereoPan: false, 1f, 0, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
