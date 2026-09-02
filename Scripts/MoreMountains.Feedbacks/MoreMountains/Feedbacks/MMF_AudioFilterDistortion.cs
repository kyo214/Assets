using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackPath("Audio/Audio Filter Distortion")]
[FeedbackHelp("This feedback lets you control a distortion audio filter over time. You'll need a MMAudioFilterDistortionShaker on the filter.")]
public class MMF_AudioFilterDistortion : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Distortion Filter", true, 28, false, false)]
	[Tooltip("the duration of the shake, in seconds")]
	public float Duration = 2f;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeDistortion;

	[Tooltip("the curve used to animate the intensity value on")]
	public AnimationCurve ShakeDistortion = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(0f, 1f)]
	public float RemapDistortionZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(0f, 1f)]
	public float RemapDistortionOne = 1f;

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
			float num = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
			float remapMin = 0f;
			float remapMax = 0f;
			if (!Timing.ConstantIntensity)
			{
				remapMin = RemapDistortionZero * num;
				remapMax = RemapDistortionOne * num;
			}
			MMAudioFilterDistortionShakeEvent.Trigger(ShakeDistortion, FeedbackDuration, remapMin, remapMax, RelativeDistortion, num, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		base.CustomStopFeedback(position, feedbacksIntensity);
		if (Active && FeedbackTypeAuthorized)
		{
			MMAudioFilterDistortionShakeEvent.Trigger(ShakeDistortion, FeedbackDuration, 0f, 0f, relativeDistortion: false, 1f, 0, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
