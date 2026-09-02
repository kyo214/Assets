using Steamworks;

namespace _Modules.Achievement.Scripts;

public class SteamAchievementPlatform : AchievementPlatformBase
{
	public override void AddStatsProgress(string statName, int statValue, bool keepHighestValue = false)
	{
		if (keepHighestValue)
		{
			int statInt = SteamUserStats.GetStatInt(statName);
			if (statValue > statInt)
			{
				AddStatsProgress(statName, statValue);
			}
		}
		else
		{
			SteamUserStats.SetStat(statName, statValue);
		}
	}

	public override bool CheckIfAchievementComplete(string achievementID)
	{
		return SteamServerStats.GetAchievement(SteamClient.SteamId, achievementID);
	}

	public override void ResetAllStatusAndAchievement()
	{
		SteamUserStats.ResetAll(includeAchievements: true);
	}

	public override void UnlockAchievement(string achievementID)
	{
		if (!CheckIfAchievementComplete(achievementID))
		{
			SteamServerStats.SetAchievement(SteamClient.SteamId, achievementID);
			SteamServerStats.StoreUserStats(SteamClient.SteamId);
		}
	}
}
