namespace MoreMountains.Tools;

public struct MMAchievementUnlockedEvent(MMAchievement newAchievement)
{
	public MMAchievement Achievement = newAchievement;

	private static MMAchievementUnlockedEvent e;

	public static void Trigger(MMAchievement newAchievement)
	{
		e.Achievement = newAchievement;
		MMEventManager.TriggerEvent(e);
	}
}
