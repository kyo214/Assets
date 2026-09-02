using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackHelp("This feedback allows you to control white balance temperature and tint over time. It requires you have in your scene an object with a Volume with WhiteBalance active, and a MMWhiteBalanceShaker_URP component.")]
[FeedbackPath("PostProcess/White Balance URP")]
public class MMF_WhiteBalance_URP : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("White Balance", true, 29, false, false)]
	[Tooltip("the duration of the shake, in seconds")]
	public float ShakeDuration = 1f;

	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeValues = true;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[MMFInspectorGroup("Temperature", true, 29, false, false)]
	[Tooltip("the curve used to animate the temperature value on")]
	public AnimationCurve ShakeTemperature = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-100f, 100f)]
	public float RemapTemperatureZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-100f, 100f)]
	public float RemapTemperatureOne = 100f;

	[MMFInspectorGroup("Tint", true, 29, false, false)]
	[Tooltip("the curve used to animate the tint value on")]
	public AnimationCurve ShakeTint = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(-100f, 100f)]
	public float RemapTintZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(-100f, 100f)]
	public float RemapTintOne = 100f;

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

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			float attenuation = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
			MMWhiteBalanceShakeEvent_URP.Trigger(ShakeTemperature, FeedbackDuration, RemapTemperatureZero, RemapTemperatureOne, ShakeTint, RemapTintZero, RemapTintOne, RelativeValues, attenuation, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMWhiteBalanceShakeEvent_URP.Trigger(ShakeTemperature, FeedbackDuration, RemapTemperatureZero, RemapTemperatureOne, ShakeTint, RemapTintZero, RemapTintOne, RelativeValues, 1f, Channel, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
