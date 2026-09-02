using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackPath("Camera/Clipping Planes")]
[FeedbackHelp("This feedback lets you control a camera's clipping planes over time. You'll need a MMCameraClippingPlanesShaker on your camera.")]
public class MMFeedbackCameraClippingPlanes : MMFeedback
{
	public static bool FeedbackTypeAuthorized = true;

	[Header("Clipping Planes Feedback")]
	[Tooltip("the channel to emit on")]
	public int Channel;

	[Tooltip("the duration of the shake, in seconds")]
	public float Duration = 2f;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeClippingPlanes;

	[Header("Near Plane")]
	[Tooltip("the curve used to animate the intensity value on")]
	public AnimationCurve ShakeNear = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapNearZero = 0.3f;

	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapNearOne = 100f;

	[Header("Far Plane")]
	[Tooltip("the curve used to animate the intensity value on")]
	public AnimationCurve ShakeFar = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapFarZero = 0.3f;

	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapFarOne = 100f;

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
			MMCameraClippingPlanesShakeEvent.Trigger(ShakeNear, FeedbackDuration, RemapNearZero, RemapNearOne, ShakeFar, RemapFarZero, RemapFarOne, RelativeClippingPlanes, feedbacksIntensity, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMCameraClippingPlanesShakeEvent.Trigger(ShakeNear, FeedbackDuration, RemapNearZero, RemapNearOne, ShakeFar, RemapFarZero, RemapFarOne, relativeValue: false, 1f, 0, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, TimescaleModes.Scaled, stop: true);
		}
	}
}
