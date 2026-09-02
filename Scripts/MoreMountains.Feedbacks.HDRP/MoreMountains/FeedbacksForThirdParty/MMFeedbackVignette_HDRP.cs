using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackPath("PostProcess/Vignette HDRP")]
[FeedbackHelp("This feedback allows you to control vignette intensity over time. It requires you have in your scene an object with a Volume with Vignette active, and a MMVignetteShaker_HDRP component.")]
public class MMFeedbackVignette_HDRP : MMFeedback
{
	public static bool FeedbackTypeAuthorized = true;

	[Header("Vignette")]
	[Tooltip("the channel to emit on")]
	public int Channel;

	[Tooltip("the duration of the shake, in seconds")]
	public float Duration = 0.2f;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[Header("Intensity")]
	[Tooltip("the curve to animate the intensity on")]
	public AnimationCurve Intensity = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's zero to")]
	[Range(0f, 1f)]
	public float RemapIntensityZero;

	[Tooltip("the value to remap the curve's one to")]
	[Range(0f, 1f)]
	public float RemapIntensityOne = 1f;

	[Tooltip("whether or not to add to the initial intensity")]
	public bool RelativeIntensity;

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
			float attenuation = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
			MMVignetteShakeEvent_HDRP.Trigger(Intensity, FeedbackDuration, RemapIntensityZero, RemapIntensityOne, RelativeIntensity, attenuation, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMVignetteShakeEvent_HDRP.Trigger(Intensity, FeedbackDuration, RemapIntensityZero, RemapIntensityOne, RelativeIntensity, 1f, Channel, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
