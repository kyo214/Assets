namespace MoreMountains.Tools;

public struct MMSoundManagerTrackFadeEvent(MMSoundManager.MMSoundManagerTracks track, float fadeDuration, float finalVolume, MMTweenType fadeTween)
{
	public MMSoundManager.MMSoundManagerTracks Track = track;

	public float FadeDuration = fadeDuration;

	public float FinalVolume = finalVolume;

	public MMTweenType FadeTween = fadeTween;

	private static MMSoundManagerTrackFadeEvent e;

	public static void Trigger(MMSoundManager.MMSoundManagerTracks track, float fadeDuration, float finalVolume, MMTweenType fadeTween)
	{
		e.Track = track;
		e.FadeDuration = fadeDuration;
		e.FinalVolume = finalVolume;
		e.FadeTween = fadeTween;
		MMEventManager.TriggerEvent(e);
	}
}
