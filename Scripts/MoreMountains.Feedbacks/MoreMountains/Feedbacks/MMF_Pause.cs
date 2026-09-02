using System.Collections;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackHelp("This feedback will cause a pause when met, preventing any other feedback lower in the sequence to run until it's complete.")]
[FeedbackPath("Pause/Pause")]
public class MMF_Pause : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("Pause", true, 32, false, false)]
	[Tooltip("the duration of the pause, in seconds")]
	public float PauseDuration = 1f;

	public bool RandomizePauseDuration;

	[MMFCondition("RandomizePauseDuration", true)]
	public float MinPauseDuration = 1f;

	[MMFCondition("RandomizePauseDuration", true)]
	public float MaxPauseDuration = 3f;

	[MMFCondition("RandomizePauseDuration", true)]
	public bool RandomizeOnEachPlay = true;

	[Tooltip("if this is true, you'll need to call the Resume() method on the host MMFeedbacks for this pause to stop, and the rest of the sequence to play")]
	public bool ScriptDriven;

	[Tooltip("if this is true, a script driven pause will resume after its AutoResumeAfter delay, whether it has been manually resumed or not")]
	[MMFCondition("ScriptDriven", true)]
	public bool AutoResume;

	[Tooltip("the duration after which to auto resume, regardless of manual resume calls beforehand")]
	[MMFCondition("AutoResume", true)]
	public float AutoResumeAfter = 0.25f;

	public override IEnumerator Pause => PauseWait();

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

	protected virtual IEnumerator PauseWait()
	{
		if (Timing.TimescaleMode == TimescaleModes.Scaled)
		{
			return MMFeedbacksCoroutine.WaitFor(PauseDuration);
		}
		return MMFeedbacksCoroutine.WaitForUnscaled(PauseDuration);
	}

	protected override void CustomInitialization(MMF_Player owner)
	{
		base.CustomInitialization(owner);
		ScriptDrivenPause = ScriptDriven;
		ScriptDrivenPauseAutoResume = (AutoResume ? AutoResumeAfter : (-1f));
		if (RandomizePauseDuration)
		{
			PauseDuration = Random.Range(MinPauseDuration, MaxPauseDuration);
		}
	}

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			if (RandomizePauseDuration && RandomizeOnEachPlay)
			{
				PauseDuration = Random.Range(MinPauseDuration, MaxPauseDuration);
			}
			Owner.StartCoroutine(PlayPause());
		}
	}

	protected virtual IEnumerator PlayPause()
	{
		yield return Pause;
	}
}
