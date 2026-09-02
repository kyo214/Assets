using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackHelp("This feedback allows you to control bloom intensity and threshold over time. It requires you have in your scene an object with a Volume with Bloom active, and a MMBloomShaker_URP component.")]
[FeedbackPath("PostProcess/Bloom URP")]
public class MMF_Bloom_URP : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Bloom", true, 41, false, false)]
	[Tooltip("the duration of the feedback, in seconds")]
	public float ShakeDuration = 0.2f;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[Tooltip("whether or not to add to the initial intensity")]
	public bool RelativeValues = true;

	[MMFInspectorGroup("Intensity", true, 42, false, false)]
	[Tooltip("the curve to animate the intensity on")]
	public AnimationCurve ShakeIntensity = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapIntensityZero;

	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapIntensityOne = 1f;

	[MMFInspectorGroup("Threshold", true, 43, false, false)]
	[Tooltip("the curve to animate the threshold on")]
	public AnimationCurve ShakeThreshold = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapThresholdZero;

	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapThresholdOne;

	public override float FeedbackDuration
	{
		get
		{
			return ApplyTimeMultiplier(ShakeDuration);
		}
		set
		{
			ShakeDuration = value;
		}
	}

	public override bool HasChannel => true;

	protected override void CustomPlayFeedback(Vector3 position, float attenuation = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			MMBloomShakeEvent_URP.Trigger(ShakeIntensity, FeedbackDuration, RemapIntensityZero, RemapIntensityOne, ShakeThreshold, RemapThresholdZero, RemapThresholdOne, RelativeValues, attenuation, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMBloomShakeEvent_URP.Trigger(ShakeIntensity, FeedbackDuration, RemapIntensityZero, RemapIntensityOne, ShakeThreshold, RemapThresholdZero, RemapThresholdOne, RelativeValues, 1f, Channel, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
