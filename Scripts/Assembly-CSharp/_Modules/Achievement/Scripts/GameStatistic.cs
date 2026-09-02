namespace _Modules.Achievement.Scripts;

public static class GameStatistic
{
	public static void AddKillEnemy(EnemyController enemyController, byte weaponType)
	{
		GlobalSaveData instance = GlobalSaveData.instance;
		if (!(instance == null))
		{
			instance.AddGameStatisticProgress(GameStatisticType.KILL_ENEMY, 1);
			instance.AddGameStatisticProgress(GameStatisticType.KILL_ENEMY, 1, enemyController.data.type.ToString());
			if (enemyController.isElite)
			{
				instance.AddGameStatisticProgress(GameStatisticType.KILL_ENEMY, 1, "ELITE");
			}
		}
	}

	public static void AddGameOver()
	{
		GlobalSaveData.instance?.AddGameStatisticProgress(GameStatisticType.GAME_OVER, 1);
	}

	public static void AddCompletedGame()
	{
		GlobalSaveData.instance?.AddGameStatisticProgress(GameStatisticType.COMPLETE_GAME, 1);
	}

	public static void AddDeath()
	{
		GlobalSaveData.instance?.AddGameStatisticProgress(GameStatisticType.DEATH, 1);
	}
}
