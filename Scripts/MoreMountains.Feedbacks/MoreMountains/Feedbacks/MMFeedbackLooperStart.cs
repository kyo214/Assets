using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback can act as a pause but also as a start point for your loops. Add a FeedbackLooper below this (and after a few feedbacks) and your MMFeedbacks will loop between both.")]
[FeedbackPath("Loop/Looper Start")]
public class MMFeedbackLooperStart : MMFeedbackPause
{
	public override bool LooperStart => true;

	public override float FeedbackDuration
	{
		get
		{
			return ApplyTimeMultiplier(PauseDuration);
		}
		set
		{
			PauseDuration = value;
		}
	}

	protected virtual void Reset()
	{
		PauseDuration = 0f;
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && MMFeedbackPause.FeedbackTypeAuthorized)
		{
			StartCoroutine(PlayPause());
		}
	}
}
