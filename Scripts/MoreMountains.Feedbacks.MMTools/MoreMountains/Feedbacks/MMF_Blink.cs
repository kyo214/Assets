using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback lets you trigger a blink on an MMBlink object.")]
[FeedbackPath("Renderer/MMBlink")]
public class MMF_Blink : MMF_Feedback
{
	public enum BlinkModes
	{
		Toggle = 0,
		Start = 1,
		Stop = 2
	}

	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Blink", true, 61, true, false)]
	[Tooltip("the target object to blink")]
	public MMBlink TargetBlink;

	[Tooltip("the selected mode for this feedback")]
	public BlinkModes BlinkMode;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && !(TargetBlink == null))
		{
			TargetBlink.TimescaleMode = Timing.TimescaleMode;
			switch (BlinkMode)
			{
			case BlinkModes.Toggle:
				TargetBlink.ToggleBlinking();
				break;
			case BlinkModes.Start:
				TargetBlink.StartBlinking();
				break;
			case BlinkModes.Stop:
				TargetBlink.StopBlinking();
				break;
			}
		}
	}
}
