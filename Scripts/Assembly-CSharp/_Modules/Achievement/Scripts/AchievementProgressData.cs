namespace _Modules.Achievement.Scripts;

public class AchievementProgressData
{
	public AchievementDataSO achievementData;

	public AchievementSaveData achievementSaveData;

	public AchievementProgressData(AchievementDataSO achievementData, AchievementSaveData achievementSaveData)
	{
		this.achievementData = achievementData;
		this.achievementSaveData = achievementSaveData;
	}
}
