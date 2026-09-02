namespace MoreMountains.Tools;

public struct MMSoundManagerTrackEvent(MMSoundManagerTrackEventTypes trackEventType, MMSoundManager.MMSoundManagerTracks track = MMSoundManager.MMSoundManagerTracks.Master, float volume = 1f)
{
	public MMSoundManagerTrackEventTypes TrackEventType = trackEventType;

	public MMSoundManager.MMSoundManagerTracks Track = track;

	public float Volume = volume;

	private static MMSoundManagerTrackEvent e;

	public static void Trigger(MMSoundManagerTrackEventTypes trackEventType, MMSoundManager.MMSoundManagerTracks track = MMSoundManager.MMSoundManagerTracks.Master, float volume = 1f)
	{
		e.TrackEventType = trackEventType;
		e.Track = track;
		e.Volume = volume;
		MMEventManager.TriggerEvent(e);
	}
}
