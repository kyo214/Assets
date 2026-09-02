using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackPath("PostProcess/Lens Distortion")]
[FeedbackHelp("This feedback allows you to control lens distortion intensity over time. It requires you have in your scene an object with a PostProcessVolume with Lens Distortion active, and a MMLensDistortionShaker component.")]
public class MMF_LensDistortion : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Lens Distortion", true, 56, false, false)]
	[Tooltip("the duration of the shake in seconds")]
	public float Duration = 0.5f;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[MMFInspectorGroup("Intensity", true, 57, false, false)]
	[Tooltip("whether or not to add to the initial intensity value")]
	public bool RelativeIntensity;

	[Tooltip("the curve to animate the intensity on")]
	public AnimationCurve Intensity = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.2f, 1f), new Keyframe(0.25f, -1f), new Keyframe(0.35f, 0.7f), new Keyframe(0.4f, -0.7f), new Keyframe(0.6f, 0.3f), new Keyframe(0.65f, -0.3f), new Keyframe(0.8f, 0.1f), new Keyframe(0.85f, -0.1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-100f, 100f)]
	public float RemapIntensityZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-100f, 100f)]
	public float RemapIntensityOne = 40f;

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
			MMLensDistortionShakeEvent.Trigger(Intensity, FeedbackDuration, RemapIntensityZero, RemapIntensityOne, RelativeIntensity, feedbacksIntensity2, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMLensDistortionShakeEvent.Trigger(Intensity, FeedbackDuration, RemapIntensityZero, RemapIntensityOne, RelativeIntensity, 1f, 0, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
