using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackPath("Audio/Audio Filter Echo")]
[FeedbackHelp("This feedback lets you control an echo audio filter's wet mix value over time. You'll need a MMAudioFilterEchoShaker on your filter.")]
public class MMF_AudioFilterEcho : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Echo Filter", true, 28, false, false)]
	[Tooltip("the duration of the shake, in seconds")]
	public float Duration = 2f;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeEcho;

	[Tooltip("the curve used to animate the intensity value on")]
	public AnimationCurve ShakeEcho = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Range(0f, 1f)]
	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapEchoZero;

	[Range(0f, 1f)]
	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapEchoOne = 1f;

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
			MMAudioFilterEchoShakeEvent.Trigger(ShakeEcho, FeedbackDuration, RemapEchoZero, RemapEchoOne, RelativeEcho, feedbacksIntensity2, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMAudioFilterEchoShakeEvent.Trigger(ShakeEcho, FeedbackDuration, RemapEchoZero, RemapEchoOne, RelativeEcho, 1f, 0, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
