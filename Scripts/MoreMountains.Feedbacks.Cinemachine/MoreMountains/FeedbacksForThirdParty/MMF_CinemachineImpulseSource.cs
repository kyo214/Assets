using Cinemachine;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackPath("Camera/Cinemachine Impulse Source")]
[FeedbackHelp("This feedback lets you generate an impulse on a Cinemachine Impulse source. You'll need a Cinemachine Impulse Listener on your camera for this to work.")]
public class MMF_CinemachineImpulseSource : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Cinemachine Impulse Source", true, 28, false, false)]
	[Tooltip("the velocity to apply to the impulse shake")]
	public Vector3 Velocity = new Vector3(1f, 1f, 1f);

	[Tooltip("the impulse definition to broadcast")]
	public CinemachineImpulseSource ImpulseSource;

	[Tooltip("whether or not to clear impulses (stopping camera shakes) when the Stop method is called on that feedback")]
	public bool ClearImpulseOnStop;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && ImpulseSource != null)
		{
			ImpulseSource.GenerateImpulse(Velocity);
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && ClearImpulseOnStop)
		{
			base.CustomStopFeedback(position, feedbacksIntensity);
			CinemachineImpulseManager.Instance.Clear();
		}
	}
}
