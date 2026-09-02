using Cinemachine;
using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackPath("Camera/Cinemachine Impulse Clear")]
[FeedbackHelp("This feedback lets you trigger a Cinemachine Impulse clear, stopping instantly any impulse that may be playing.")]
public class MMFeedbackCinemachineImpulseClear : MMFeedback
{
	public static bool FeedbackTypeAuthorized = true;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			CinemachineImpulseManager.Instance.Clear();
		}
	}
}
