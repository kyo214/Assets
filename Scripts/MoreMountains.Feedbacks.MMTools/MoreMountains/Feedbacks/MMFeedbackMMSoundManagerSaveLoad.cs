using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackPath("Audio/MMSoundManager Save and Load")]
[FeedbackHelp("This feedback will let you trigger save, load, and reset on MMSoundManager settings. You will need a MMSoundManager in your scene for this to work.")]
public class MMFeedbackMMSoundManagerSaveLoad : MMFeedback
{
	public enum Modes
	{
		Save = 0,
		Load = 1,
		Reset = 2
	}

	public static bool FeedbackTypeAuthorized = true;

	[Header("MMSoundManager Save and Load")]
	[Tooltip("the selected mode to interact with save settings on the MMSoundManager")]
	public Modes Mode;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			switch (Mode)
			{
			case Modes.Save:
				MMSoundManagerEvent.Trigger(MMSoundManagerEventTypes.SaveSettings);
				break;
			case Modes.Load:
				MMSoundManagerEvent.Trigger(MMSoundManagerEventTypes.LoadSettings);
				break;
			case Modes.Reset:
				MMSoundManagerEvent.Trigger(MMSoundManagerEventTypes.ResetSettings);
				break;
			}
		}
	}
}
