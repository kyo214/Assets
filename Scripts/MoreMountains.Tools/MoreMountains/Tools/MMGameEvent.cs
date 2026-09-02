namespace MoreMountains.Tools;

public struct MMGameEvent(string newName)
{
	public string EventName = newName;

	private static MMGameEvent e;

	public static void Trigger(string newName)
	{
		e.EventName = newName;
		MMEventManager.TriggerEvent(e);
	}
}
