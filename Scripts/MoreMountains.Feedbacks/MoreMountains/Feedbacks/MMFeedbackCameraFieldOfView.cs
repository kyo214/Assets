using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackPath("Camera/Field of View")]
[FeedbackHelp("This feedback lets you control a camera's field of view over time. You'll need a MMCameraFieldOfViewShaker on your camera.")]
public class MMFeedbackCameraFieldOfView : MMFeedback
{
	public static bool FeedbackTypeAuthorized = true;

	[Header("Field of View Feedback")]
	[Tooltip("the channel to emit on")]
	public int Channel;

	[Tooltip("the duration of the shake, in seconds")]
	public float Duration = 2f;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[Header("Field of View")]
	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeFieldOfView;

	[Tooltip("the curve used to animate the intensity value on")]
	public AnimationCurve ShakeFieldOfView = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	[Range(0f, 179f)]
	public float RemapFieldOfViewZero = 60f;

	[Tooltip("the value to remap the curve's 1 to")]
	[Range(0f, 179f)]
	public float RemapFieldOfViewOne = 120f;

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
			float feedbacksIntensity2 = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
			MMCameraFieldOfViewShakeEvent.Trigger(ShakeFieldOfView, FeedbackDuration, RemapFieldOfViewZero, RemapFieldOfViewOne, RelativeFieldOfView, feedbacksIntensity2, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMCameraFieldOfViewShakeEvent.Trigger(ShakeFieldOfView, FeedbackDuration, RemapFieldOfViewZero, RemapFieldOfViewOne, relativeValue: false, 1f, 0, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
