using System.Collections.Generic;
using UnityEngine;
using _Modules.Data.Scripts;

namespace _Modules.Achievement.Scripts;

public static class AchievementManager
{
	private static AchievementPlatformBase achievementPlatform;

	public static AchievementDataSO[] achievementDataList;

	private static List<AchievementProgressData> achievementProgressNotCompletedList = new List<AchievementProgressData>();

	private static Dictionary<string, List<AchievementDataSO>> achievementAffectorImpactedDictionary = new Dictionary<string, List<AchievementDataSO>>();

	private static UserSaveData UserData => GlobalSaveData.instance.UserSaveData;

	public static void InitAchievement()
	{
		InitAchievementData();
		InitAchievementPlatform();
	}

	private static void InitAchievementPlatform()
	{
	}

	private static void InitAchievementData()
	{
		achievementAffectorImpactedDictionary = new Dictionary<string, List<AchievementDataSO>>();
		AchievementDataSO[] array = achievementDataList;
		int num = ((array != null) ? array.Length : 0);
		for (int i = 0; i < num; i++)
		{
			AchievementDataSO achievementDataSO = achievementDataList[i];
			for (int j = 0; j < achievementDataSO.Dependencies.Length; j++)
			{
				AddAchievementAffector(achievementDataSO.Dependencies[j].AchievementId, achievementDataSO);
			}
		}
	}

	private static void AddAchievementAffector(string key, AchievementDataSO achievementData)
	{
		if (achievementAffectorImpactedDictionary.ContainsKey(key))
		{
			if (!achievementAffectorImpactedDictionary[key].Contains(achievementData))
			{
				achievementAffectorImpactedDictionary[key].Add(achievementData);
			}
		}
		else
		{
			achievementAffectorImpactedDictionary.Add(key, new List<AchievementDataSO> { achievementData });
		}
	}

	private static List<AchievementDataSO> GetAchievementAffector(string key)
	{
		if (achievementAffectorImpactedDictionary.ContainsKey(key))
		{
			return achievementAffectorImpactedDictionary[key];
		}
		return new List<AchievementDataSO>();
	}

	public static void UpdateAchievementCurrentProfileData()
	{
		achievementProgressNotCompletedList = new List<AchievementProgressData>();
		if (UserData.AchievementSaveDataDictionary == null)
		{
			UserData.AchievementSaveDataDictionary = new Dictionary<string, AchievementSaveData>();
		}
		int num = achievementDataList.Length;
		for (int i = 0; i < num; i++)
		{
			AchievementDataSO achievementDataSO = achievementDataList[i];
			for (int num2 = achievementDataSO.UnlockCondition.Length - 1; num2 >= 0; num2--)
			{
				achievementDataSO.UnlockCondition[num2].gameStatisticData.UpdateGameStatisticKey();
			}
			AchievementSaveData value;
			if (!UserData.AchievementSaveDataDictionary.ContainsKey(achievementDataSO.AchievementId))
			{
				value = new AchievementSaveData(achievementDataSO);
				UserData.AchievementSaveDataDictionary.Add(achievementDataSO.AchievementId, value);
			}
			value = UserData.AchievementSaveDataDictionary[achievementDataSO.AchievementId];
			if (!CheckIfAchievementCompleted(achievementDataSO, value))
			{
				achievementProgressNotCompletedList.Add(new AchievementProgressData(achievementDataSO, value));
			}
			else
			{
				achievementPlatform?.UnlockAchievement(value.Id);
			}
		}
	}

	private static void UnlockAchievement(AchievementProgressData achievementProgressData)
	{
		achievementProgressData.achievementSaveData.Completed = true;
		achievementPlatform?.UnlockAchievement(achievementProgressData.achievementSaveData.Id);
		List<AchievementDataSO> achievementAffector = GetAchievementAffector(achievementProgressData.achievementData.AchievementId);
		achievementProgressNotCompletedList.Remove(achievementProgressData);
		Debug.Log("Unlock Achievement : " + achievementProgressData.achievementSaveData.Id + " Remaning : " + achievementProgressNotCompletedList.Count);
		for (int i = 0; i < achievementAffector.Count; i++)
		{
			if (!UserData.GetAchievementStatusCompleted(achievementAffector[i].AchievementId))
			{
				AchievementProgressData achievementProgressNotCompletedData = GetAchievementProgressNotCompletedData(achievementAffector[i].AchievementId);
				if (achievementProgressNotCompletedData != null && CheckIfAchievementCompleted(achievementAffector[i], achievementProgressNotCompletedData.achievementSaveData))
				{
					UnlockAchievement(achievementProgressNotCompletedData);
				}
			}
		}
		GlobalSaveData.instance?.SaveUserData();
	}

	public static void AddProgress(string key, float value)
	{
		List<AchievementProgressData> list = new List<AchievementProgressData>(achievementProgressNotCompletedList);
		for (int num = list.Count - 1; num >= 0; num--)
		{
			bool flag = false;
			AchievementProgressData achievementProgressData = list[num];
			AchievementSaveData achievementSaveData = achievementProgressData.achievementSaveData;
			for (int num2 = achievementProgressData.achievementData.UnlockCondition.Length - 1; num2 >= 0; num2--)
			{
				AchievementConditionUnlockData achievementConditionUnlockData = achievementProgressData.achievementData.UnlockCondition[num2];
				if (achievementConditionUnlockData.gameStatisticData.GameStatisticKey == key)
				{
					flag = true;
					achievementSaveData.ProgressList[num2] += value;
					if (achievementConditionUnlockData.CheckActivation(achievementSaveData.ProgressList[num2].ToString()) && CheckIfAchievementCompleted(achievementProgressData.achievementData, achievementSaveData))
					{
						UnlockAchievement(achievementProgressData);
						break;
					}
				}
			}
			if (flag && achievementProgressData.achievementData.StatsID != "")
			{
				achievementPlatform?.AddStatsProgress(achievementProgressData.achievementData.StatsID, (int)achievementProgressData.achievementSaveData.CalculateTotalProgress(), keepHighestValue: true);
			}
		}
	}

	private static bool CheckIfAchievementCompleted(AchievementDataSO achievementData, AchievementSaveData achievementSaveData)
	{
		if (!CheckDepedenciesAchievementCompleted(achievementData.Dependencies))
		{
			return false;
		}
		for (int num = achievementData.UnlockCondition.Length - 1; num >= 0; num--)
		{
			if (!achievementData.UnlockCondition[num].CheckActivation(achievementSaveData.ProgressList[num].ToString()))
			{
				return false;
			}
		}
		return true;
	}

	public static void ResetAchievementDataForOneLevel()
	{
		for (int num = achievementProgressNotCompletedList.Count - 1; num >= 0; num--)
		{
			AchievementProgressData achievementProgressData = achievementProgressNotCompletedList[num];
			AchievementSaveData achievementSaveData = achievementProgressData.achievementSaveData;
			for (int num2 = achievementProgressData.achievementData.UnlockCondition.Length - 1; num2 >= 0; num2--)
			{
				if (achievementProgressData.achievementData.UnlockCondition[num2].IsOnOneRun)
				{
					achievementSaveData.ProgressList[num2] = 0f;
				}
			}
		}
	}

	public static bool CheckDepedenciesAchievementCompleted(AchievementDataSO[] depedenciesAchievementDataList)
	{
		for (int i = 0; i < depedenciesAchievementDataList.Length; i++)
		{
			string achievementId = depedenciesAchievementDataList[i].AchievementId;
			if (!UserData.GetAchievementStatusCompleted(achievementId))
			{
				return false;
			}
		}
		return true;
	}

	private static AchievementProgressData GetAchievementProgressNotCompletedData(string key)
	{
		for (int i = 0; i < achievementProgressNotCompletedList.Count; i++)
		{
			if (achievementProgressNotCompletedList[i].achievementData.AchievementId == key)
			{
				return achievementProgressNotCompletedList[i];
			}
		}
		return null;
	}

	public static void UnlockAchievement(AchievementDataSO achievementData)
	{
		AchievementConditionUnlockData[] unlockCondition = achievementData.UnlockCondition;
		foreach (AchievementConditionUnlockData achievementConditionUnlockData in unlockCondition)
		{
			UserData.AddGameStatisticProgress(GameStatisticData.ConvertToKey(achievementConditionUnlockData.gameStatisticData.GameStatisticType, achievementConditionUnlockData.gameStatisticData.TargetAdditionalVarKey), float.Parse(achievementConditionUnlockData.TargetVarValue));
		}
	}

	public static void ResetAchievementFromPlatform()
	{
		achievementPlatform?.ResetAllStatusAndAchievement();
	}
}
