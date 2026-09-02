using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will trigger a MMGameEvent of the specified name when played")]
[FeedbackPath("Events/MMGameEvent")]
public class MMFeedbackMMGameEvent : MMFeedback
{
	public static bool FeedbackTypeAuthorized = true;

	public string MMGameEventName;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			MMGameEvent.Trigger(MMGameEventName);
		}
	}
}
