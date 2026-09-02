namespace MoreMountains.Tools;

public struct MMSoundManagerSoundFadeEvent(int soundID, float fadeDuration, float finalVolume, MMTweenType fadeTween)
{
	public int SoundID = soundID;

	public float FadeDuration = fadeDuration;

	public float FinalVolume = finalVolume;

	public MMTweenType FadeTween = fadeTween;

	private static MMSoundManagerSoundFadeEvent e;

	public static void Trigger(int soundID, float fadeDuration, float finalVolume, MMTweenType fadeTween)
	{
		e.SoundID = soundID;
		e.FadeDuration = fadeDuration;
		e.FinalVolume = finalVolume;
		e.FadeTween = fadeTween;
		MMEventManager.TriggerEvent(e);
	}
}
