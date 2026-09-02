using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackPath("Camera/Orthographic Size")]
[FeedbackHelp("This feedback lets you control a camera's orthographic size over time. You'll need a MMCameraOrthographicSizeShaker on your camera.")]
public class MMF_CameraOrthographicSize : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Orthographic Size", true, 41, false, false)]
	[Tooltip("the duration of the shake, in seconds")]
	public float Duration = 2f;

	[Tooltip("whether or not to reset shaker values after shake")]
	public bool ResetShakerValuesAfterShake = true;

	[Tooltip("whether or not to reset the target's values after shake")]
	public bool ResetTargetValuesAfterShake = true;

	[Tooltip("whether or not to add to the initial value")]
	public bool RelativeOrthographicSize;

	[Tooltip("the curve used to animate the intensity value on")]
	public AnimationCurve ShakeOrthographicSize = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));

	[Tooltip("the value to remap the curve's 0 to")]
	public float RemapOrthographicSizeZero = 5f;

	[Tooltip("the value to remap the curve's 1 to")]
	public float RemapOrthographicSizeOne = 10f;

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
			MMCameraOrthographicSizeShakeEvent.Trigger(ShakeOrthographicSize, FeedbackDuration, RemapOrthographicSizeZero, RemapOrthographicSizeOne, RelativeOrthographicSize, feedbacksIntensity, Channel, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMCameraOrthographicSizeShakeEvent.Trigger(ShakeOrthographicSize, FeedbackDuration, RemapOrthographicSizeZero, RemapOrthographicSizeOne, relativeValue: false, 1f, 0, resetShakerValuesAfterShake: true, resetTargetValuesAfterShake: true, forwardDirection: true, stop: true);
		}
	}
}
