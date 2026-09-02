namespace _Modules.Achievement.Scripts;

public abstract class AchievementPlatformBase
{
	public abstract void AddStatsProgress(string statName, int statValue, bool keepHighestValue = false);

	public abstract bool CheckIfAchievementComplete(string achievementID);

	public abstract void UnlockAchievement(string achievementID);

	public abstract void ResetAllStatusAndAchievement();
}
