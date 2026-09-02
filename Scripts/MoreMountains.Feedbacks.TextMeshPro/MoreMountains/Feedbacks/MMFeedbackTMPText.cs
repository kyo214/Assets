using TMPro;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you change the text of a target TMP text component")]
[FeedbackPath("TextMesh Pro/TMP Text")]
public class MMFeedbackTMPText : MMFeedback
{
	public static bool FeedbackTypeAuthorized = true;

	[Header("TextMesh Pro")]
	[Tooltip("the target TMP_Text component we want to change the text on")]
	public TMP_Text TargetTMPText;

	[Tooltip("the new text to replace the old one with")]
	[TextArea]
	public string NewText = "Hello World";

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && !(TargetTMPText == null))
		{
			TargetTMPText.text = NewText;
		}
	}
}
