using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will 'hold', or wait, until all previous feedbacks have been executed, and will then pause the execution of your MMFeedbacks sequence, for the specified duration.")]
[FeedbackPath("Pause/Holding Pause")]
public class MMF_HoldingPause : MMF_Pause
{
	public override bool HoldingPause => true;

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

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active)
		{
			Owner.StartCoroutine(PlayPause());
		}
	}
}
