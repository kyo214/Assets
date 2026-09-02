using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackPath("PostProcess/Exposure HDRP")]
[FeedbackHelp("This feedback allows you to control Exposure intensity over time. It requires you have in your scene an object with a Volume with Exposure active, and a MMExposureShaker_HDRP component.")]
public class MMF_Exposure_HDRP : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Exposure", true, 17, false, false)]
	[Tooltip("the duration of the shake, in seconds")]
	public float Duration = 0.2f;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[MMFInspectorGroup("Intensity", true, 18, false, false)]
	[Tooltip("the curve to animate the intensity on")]
	public AnimationCurve FixedExposure = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapFixedExposureZero = 8.5f;

	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapFixedExposureOne = 6f;

	[Tooltip("whether or not to add to the initial intensity")]
	public bool RelativeFixedExposure;

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
			MMExposureShakeEvent_HDRP.Trigger(FixedExposure, FeedbackDuration, RemapFixedExposureZero, RemapFixedExposureOne, RelativeFixedExposure, attenuation, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMExposureShakeEvent_HDRP.Trigger(FixedExposure, FeedbackDuration, RemapFixedExposureZero, RemapFixedExposureOne, RelativeFixedExposure, 1f, Channel, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
