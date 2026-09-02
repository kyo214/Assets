using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback allows you to trigger any MMFeedbacks on the specified Channel within a certain range. You'll need an MMFeedbacksShaker on them.")]
[FeedbackPath("GameObject/MMFeedbacks")]
public class MMFeedbackFeedbacks : MMFeedback
{
	public static bool FeedbackTypeAuthorized = true;

	[Header("MMFeedbacks")]
	[Tooltip("the channel to broadcast on")]
	public int Channel;

	[Tooltip("whether or not to use a range")]
	public bool UseRange;

	[Tooltip("the range of the event, in units")]
	public float EventRange = 100f;

	[Tooltip("the transform to use to broadcast the event as origin point")]
	public Transform EventOriginTransform;

	public override float FeedbackDuration => 0f;

	protected override void CustomInitialization(GameObject owner)
	{
		base.CustomInitialization(owner);
		if (EventOriginTransform == null)
		{
			EventOriginTransform = base.transform;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			MMFeedbacksShakeEvent.Trigger(Channel, UseRange, EventRange, EventOriginTransform.position);
		}
	}
}
