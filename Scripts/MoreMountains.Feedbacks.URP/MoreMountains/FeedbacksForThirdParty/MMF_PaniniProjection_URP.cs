using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackHelp("This feedback allows you to control Panini Projection distance and crop to fit over time. It requires you have in your scene an object with a Volume with PaniniProjection active, and a MMPaniniProjectionShaker_URP component.")]
[FeedbackPath("PostProcess/Panini Projection URP")]
public class MMF_PaniniProjection_URP : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Panini Projection", true, 28, false, false)]
	[Tooltip("the duration of the shake, in seconds")]
	public float Duration = 0.2f;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[MMFInspectorGroup("Distance", true, 27, false, false)]
	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeDistance;

	[Tooltip("the curve used to animate the distance value on")]
	public AnimationCurve ShakeDistance = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(0f, 1f)]
	public float RemapDistanceZero;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(0f, 1f)]
	public float RemapDistanceOne = 1f;

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
			MMPaniniProjectionShakeEvent_URP.Trigger(ShakeDistance, FeedbackDuration, RemapDistanceZero, RemapDistanceOne, RelativeDistance, attenuation, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMPaniniProjectionShakeEvent_URP.Trigger(ShakeDistance, FeedbackDuration, RemapDistanceZero, RemapDistanceOne, RelativeDistance, 1f, Channel, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
