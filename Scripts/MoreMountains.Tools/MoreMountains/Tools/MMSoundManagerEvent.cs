namespace MoreMountains.Tools;

public struct MMSoundManagerEvent(MMSoundManagerEventTypes eventType)
{
	public MMSoundManagerEventTypes EventType = eventType;

	private static MMSoundManagerEvent e;

	public static void Trigger(MMSoundManagerEventTypes eventType)
	{
		e.EventType = eventType;
		MMEventManager.TriggerEvent(e);
	}
}
