namespace MoreMountains.Tools;

public struct MMSoundManagerAllSoundsControlEvent(MMSoundManagerAllSoundsControlEventTypes eventType)
{
	public MMSoundManagerAllSoundsControlEventTypes EventType = eventType;

	private static MMSoundManagerAllSoundsControlEvent e;

	public static void Trigger(MMSoundManagerAllSoundsControlEventTypes eventType)
	{
		e.EventType = eventType;
		MMEventManager.TriggerEvent(e);
	}
}
