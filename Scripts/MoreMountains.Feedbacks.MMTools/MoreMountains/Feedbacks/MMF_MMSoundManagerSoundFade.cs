using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackPath("Audio/MMSoundManager Sound Fade")]
[FeedbackHelp("This feedback lets you trigger fades on a specific sound via the MMSoundManager. You will need a MMSoundManager in your scene for this to work.")]
public class MMF_MMSoundManagerSoundFade : MMF_Feedback
{
	public static bool FeedbackTypeAuthorized = true;

	[MMFInspectorGroup("MMSoundManager Sound Fade", true, 30, false, false)]
	[Tooltip("the ID of the sound you want to fade. Has to match the ID you specified when playing the sound initially")]
	public int SoundID;

	[Tooltip("the duration of the fade, in seconds")]
	public float FadeDuration = 1f;

	[Tooltip("the volume towards which to fade")]
	[Range(0.0001f, 10f)]
	public float FinalVolume = 0.0001f;

	[Tooltip("the tween to apply over the fade")]
	public MMTweenType FadeTween = new MMTweenType(MMTween.MMTweenCurve.EaseInOutQuartic);

	protected AudioSource _targetAudioSource;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			MMSoundManagerSoundFadeEvent.Trigger(SoundID, FadeDuration, FinalVolume, FadeTween);
		}
	}
}
