using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback allows you to trigger a target MMFeedbacks, or any MMFeedbacks on the specified Channel within a certain range. You'll need an MMFeedbacksShaker on them.")]
[FeedbackPath("GameObject/Feedbacks Player")]
public class MMF_Feedbacks : MMF_Feedback
{
	public enum Modes
	{
		PlayFeedbacksInArea = 0,
		PlayTargetFeedbacks = 1
	}

	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Feedbacks", true, 79, false, false)]
	[Tooltip("the selected mode for this feedback")]
	public Modes Mode;

	[MMFEnumCondition("Mode", new int[] { 1 })]
	[Tooltip("a specific MMFeedbacks / MMF_Player to play")]
	public MMFeedbacks TargetFeedbacks;

	[MMFEnumCondition("Mode", new int[] { 0 })]
	[Tooltip("whether or not to use a range")]
	public bool UseRange;

	[MMFEnumCondition("Mode", new int[] { 0 })]
	[Tooltip("the range of the event, in units")]
	public float EventRange = 100f;

	[MMFEnumCondition("Mode", new int[] { 0 })]
	[Tooltip("the transform to use to broadcast the event as origin point")]
	public Transform EventOriginTransform;

	public override float FeedbackDuration
	{
		get
		{
			if (Mode == Modes.PlayTargetFeedbacks && TargetFeedbacks != null)
			{
				return TargetFeedbacks.TotalDuration;
			}
			return 0f;
		}
	}

	public override bool HasChannel => true;

	protected override void CustomInitialization(MMF_Player owner)
	{
		base.CustomInitialization(owner);
		if (EventOriginTransform == null)
		{
			EventOriginTransform = owner.transform;
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			if (Mode == Modes.PlayFeedbacksInArea)
			{
				MMFeedbacksShakeEvent.Trigger(Channel, UseRange, EventRange, EventOriginTransform.position);
			}
			else if (Mode == Modes.PlayTargetFeedbacks)
			{
				TargetFeedbacks?.PlayFeedbacks(position, feedbacksIntensity);
			}
		}
	}
}
