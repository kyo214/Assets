namespace MoreMountains.Tools;

public struct MMFadeStopEvent(int id = 0)
{
	public int ID = id;

	private static MMFadeStopEvent e;

	public static void Trigger(int id = 0)
	{
		e.ID = id;
		MMEventManager.TriggerEvent(e);
	}
}
