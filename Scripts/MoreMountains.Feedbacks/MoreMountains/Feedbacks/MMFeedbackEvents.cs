using UnityEngine;
using UnityEngine.Events;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback allows you to bind any type of Unity events to this feebdack's Play, Stop, Initialization and Reset methods.")]
[FeedbackPath("Events/Events")]
public class MMFeedbackEvents : MMFeedback
{
	public static bool FeedbackTypeAuthorized = true;

	[Header("Events")]
	[Tooltip("the events to trigger when the feedback is played")]
	public UnityEvent PlayEvents;

	[Tooltip("the events to trigger when the feedback is stopped")]
	public UnityEvent StopEvents;

	[Tooltip("the events to trigger when the feedback is initialized")]
	public UnityEvent InitializationEvents;

	[Tooltip("the events to trigger when the feedback is reset")]
	public UnityEvent ResetEvents;

	protected override void CustomInitialization(GameObject owner)
	{
		base.CustomInitialization(owner);
		if (Active && InitializationEvents != null)
		{
			InitializationEvents.Invoke();
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && PlayEvents != null)
		{
			PlayEvents.Invoke();
		}
	}

	protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && StopEvents != null)
		{
			StopEvents.Invoke();
		}
	}

	protected override void CustomReset()
	{
		if (Active && FeedbackTypeAuthorized && ResetEvents != null)
		{
			base.CustomReset();
			ResetEvents.Invoke();
		}
	}
}
