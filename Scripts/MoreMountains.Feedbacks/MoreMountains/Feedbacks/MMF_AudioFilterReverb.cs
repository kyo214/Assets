using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackPath("Audio/Audio Filter Reverb")]
[FeedbackHelp("This feedback lets you control a low pass audio filter over time. You'll need a MMAudioFilterReverbShaker on your filter.")]
public class MMF_AudioFilterReverb : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Reverb Filter", true, 28, false, false)]
	[Tooltip("the duration of the shake, in seconds")]
	public float Duration = 2f;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeReverb;

	[Tooltip("the curve used to animate the intensity value on")]
	public AnimationCurve ShakeReverb = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Range(-10000f, 2000f)]
	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapReverbZero = -10000f;

	[Range(-10000f, 2000f)]
	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapReverbOne = 2000f;

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

	public override bool HasChannel => true;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			float feedbacksIntensity2 = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
			MMAudioFilterReverbShakeEvent.Trigger(ShakeReverb, FeedbackDuration, RemapReverbZero, RemapReverbOne, RelativeReverb, feedbacksIntensity2, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMAudioFilterReverbShakeEvent.Trigger(ShakeReverb, FeedbackDuration, RemapReverbZero, RemapReverbOne, relativeReverb: false, 1f, 0, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
