using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackPath("PostProcess/Lens Distortion URP")]
[FeedbackHelp("This feedback allows you to control URP lens distortion intensity over time. It requires you have in your scene an object with a Volume with Lens Distortion active, and a MMLensDistortionShaker_URP component.")]
public class MMF_LensDistortion_URP : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Lens Distortion", true, 22, false, false)]
	[Tooltip("the duration of the shake in seconds")]
	public float Duration = 0.8f;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[MMFInspectorGroup("Intensity", true, 23, false, false)]
	[Tooltip("whether or not to add to the initial intensity value")]
	public bool RelativeIntensity;

	[Tooltip("the curve to animate the intensity on")]
	public AnimationCurve Intensity = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.2f, 1f), new Keyframe(0.25f, -1f), new Keyframe(0.35f, 0.7f), new Keyframe(0.4f, -0.7f), new Keyframe(0.6f, 0.3f), new Keyframe(0.65f, -0.3f), new Keyframe(0.8f, 0.1f), new Keyframe(0.85f, -0.1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-100f, 100f)]
	public float RemapIntensityZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-100f, 100f)]
	public float RemapIntensityOne = 0.5f;

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
			float attenuation = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
			MMLensDistortionShakeEvent_URP.Trigger(Intensity, FeedbackDuration, RemapIntensityZero, RemapIntensityOne, RelativeIntensity, attenuation, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMLensDistortionShakeEvent_URP.Trigger(Intensity, FeedbackDuration, RemapIntensityZero, RemapIntensityOne, RelativeIntensity, 1f, Channel, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
