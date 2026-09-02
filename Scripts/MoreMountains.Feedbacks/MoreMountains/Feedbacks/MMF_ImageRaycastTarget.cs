using UnityEngine;
using UnityEngine.UI;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will let you control the RaycastTarget parameter of a target image, turning it on or off on play")]
[FeedbackPath("UI/Image RaycastTarget")]
public class MMF_ImageRaycastTarget : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Image", true, 12, true, false)]
	[Tooltip("the target Image we want to control the RaycastTarget parameter on")]
	public Image TargetImage;

	[Tooltip("if this is true, when played, the target image will become a raycast target")]
	public bool ShouldBeRaycastTarget = true;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized && !(TargetImage == null))
		{
			TargetImage.raycastTarget = (NormalPlayDirection ? ShouldBeRaycastTarget : (!ShouldBeRaycastTarget));
		}
	}
}
