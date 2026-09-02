using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty;

[AddComponentMenu("")]
[FeedbackPath("Haptics/Haptics DEPRECATED!")]
[FeedbackHelp("This feedback has been deprecated, and is just here to avoid errors in case you were to update from an old version. Use any of the new haptic feedbacks instead.")]
public class MMF_Haptics : MMF_Feedback
{
	[Header("Deprecated Feedback")]
	public bool OutputDeprecationWarning = true;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && OutputDeprecationWarning)
		{
			Debug.LogWarning(Owner.name + " : the haptic feedback on this object is using the old version of Nice Vibrations, and won't work anymore. Replace it with any of the new haptic feedbacks.");
		}
	}
}
