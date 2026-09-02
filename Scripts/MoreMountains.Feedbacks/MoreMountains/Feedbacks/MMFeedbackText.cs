using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback lets you control the contents of a target Text over time.")]
[FeedbackPath("UI/Text")]
public class MMFeedbackText : MMFeedback
{
	public enum ColorModes
	{
		Instant = 0,
		Gradient = 1,
		Interpolate = 2
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("Target")]
	[Tooltip(" Text component to control")]
	public Text TargetText;

	[Tooltip("the new text to replace the old one with")]
	[TextArea]
	public string NewText = "Hello World";

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && !(TargetText == null))
		{
			TargetText.text = NewText;
		}
	}
}
