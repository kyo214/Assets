using UnityEngine;

namespace MoreMountains.Tools;

public struct MMSoundManagerSoundControlEvent(MMSoundManagerSoundControlEventTypes eventType, int soundID, AudioSource source = null)
{
	public int SoundID = soundID;

	public MMSoundManagerSoundControlEventTypes MMSoundManagerSoundControlEventType = eventType;

	public AudioSource TargetSource = source;

	private static MMSoundManagerSoundControlEvent e;

	public static void Trigger(MMSoundManagerSoundControlEventTypes eventType, int soundID, AudioSource source = null)
	{
		e.SoundID = soundID;
		e.TargetSource = source;
		e.MMSoundManagerSoundControlEventType = eventType;
		MMEventManager.TriggerEvent(e);
	}
}
