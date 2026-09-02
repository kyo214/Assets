namespace MoreMountains.Tools;

public struct MMAchievementChangedEvent(MMAchievement newAchievement)
{
	public MMAchievement Achievement = newAchievement;

	private static MMAchievementChangedEvent e;

	public static void Trigger(MMAchievement newAchievement)
	{
		e.Achievement = newAchievement;
		MMEventManager.TriggerEvent(e);
	}
}
