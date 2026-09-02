using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("Define camera shake properties (duration in seconds, amplitude and frequency), and this will broadcast a MMCameraShakeEvent with these same settings. You'll need to add a MMCameraShaker on your camera for this to work (or a MMCinemachineCameraShaker component on your virtual camera if you're using Cinemachine). Note that although this event and system was built for cameras in mind, you could technically use it to shake other objects as well.")]
[FeedbackPath("Camera/Camera Shake")]
public class MMFeedbackCameraShake : MMFeedback
{
	public static bool FeedbackTypeAuthorized = true;

	[Header("Camera Shake")]
	[Tooltip("whether or not this shake should repeat forever, until stopped")]
	public bool RepeatUntilStopped;

	[Tooltip("the channel to broadcast this shake on")]
	public int Channel;

	[Tooltip("the properties of the shake (duration, intensity, frequenc)")]
	public MMCameraShakeProperties CameraShakeProperties = new MMCameraShakeProperties(0.1f, 0.2f, 40f);

	public override float FeedbackDuration
	{
		get
		{
			return ApplyTimeMultiplier(CameraShakeProperties.Duration);
		}
		set
		{
			CameraShakeProperties.Duration = value;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			float num = (Timing.ConstantIntensity ? 1f : feedbacksIntensity);
			MMCameraShakeEvent.Trigger(FeedbackDuration, CameraShakeProperties.Amplitude * num, CameraShakeProperties.Frequency, CameraShakeProperties.AmplitudeX * num, CameraShakeProperties.AmplitudeY * num, CameraShakeProperties.AmplitudeZ * num, RepeatUntilStopped, Channel, Timing.TimescaleMode == TimescaleModes.Unscaled);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			MMCameraShakeStopEvent.Trigger(Channel);
		}
	}
}
