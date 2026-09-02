using MoreMountains.Tools;
using UnityEngine;

namespace MoreMountains.Feedbacks;

[AddComponentMenu("")]
[FeedbackPath("Audio/MMSoundManager Track Fade")]
[FeedbackHelp("This feedback will let you fade all the sounds on a specific track at once. You will need a MMSoundManager in your scene for this to work.")]
public class MMFeedbackMMSoundManagerTrackFade : MMFeedback
{
	public static bool FeedbackTypeAuthorized = true;

	[Header("MMSoundManager Track Fade")]
	[Tooltip("the track to fade the volume on")]
	public MMSoundManager.MMSoundManagerTracks Track;

	[Tooltip("the duration of the fade, in seconds")]
	public float FadeDuration = 1f;

	[Tooltip("the volume to reach at the end of the fade")]
	[Range(0.0001f, 10f)]
	public float FinalVolume = 0.0001f;

	[Tooltip("the tween to operate the fade on")]
	public MMTweenType FadeTween = new MMTweenType(MMTween.MMTweenCurve.EaseInOutQuartic);

	public override float FeedbackDuration => FadeDuration;

	protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1f)
	{
		if (Active && FeedbackTypeAuthorized)
		{
			MMSoundManagerTrackFadeEvent.Trigger(Track, FadeDuration, FinalVolume, FadeTween);
		}
	}
}
