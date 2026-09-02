using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackPath("Audio/MMSoundManager Sound Control")]
[FeedbackHelp("This feedback will let you control a specific sound (or sounds), targeted by SoundID, which has to match the SoundID of the sound you intially played. You will need a MMSoundManager in your scene for this to work.")]
public class MMF_MMSoundManagerSoundControl : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("MMSoundManager Sound Control", true, 30, false, false)]
	[Tooltip("the action to trigger on the specified sound")]
	public MMSoundManagerSoundControlEventTypes ControlMode;

	[Tooltip("the ID of the sound, has to match the one you specified when playing it")]
	public int SoundID;

	protected AudioSource _targetAudioSource;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			MMSoundManagerSoundControlEvent.Trigger(ControlMode, SoundID);
		}
	}
}
