using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoreMountains.Tools;

[ExecuteAlways]
public static class MMAchievementManager
{
	private static List<MMAchievement> _achievements;

	private static MMAchievement _achievement;

	private const string _defaultFileName = "Achievements";

	private const string _saveFolderName = "MMAchievements/";

	private const string _saveFileExtension = ".achievements";

	private static string _saveFileName;

	private static string _listID;

	public static List<MMAchievement> AchievementsList => _achievements;

	public static void LoadAchievementList(MMAchievementList achievementList)
	{
		_achievements = new List<MMAchievement>();
		if (achievementList == null)
		{
			return;
		}
		_listID = achievementList.AchievementsListID;
		foreach (MMAchievement achievement in achievementList.Achievements)
		{
			_achievements.Add(achievement.Copy());
		}
	}

	public static void UnlockAchievement(string achievementID)
	{
		_achievement = AchievementManagerContains(achievementID);
		if (_achievement != null)
		{
			_achievement.UnlockAchievement();
		}
	}

	public static void LockAchievement(string achievementID)
	{
		_achievement = AchievementManagerContains(achievementID);
		if (_achievement != null)
		{
			_achievement.LockAchievement();
		}
	}

	public static void AddProgress(string achievementID, int newProgress)
	{
		_achievement = AchievementManagerContains(achievementID);
		if (_achievement != null)
		{
			_achievement.AddProgress(newProgress);
		}
	}

	public static void SetProgress(string achievementID, int newProgress)
	{
		_achievement = AchievementManagerContains(achievementID);
		if (_achievement != null)
		{
			_achievement.SetProgress(newProgress);
		}
	}

	private static MMAchievement AchievementManagerContains(string searchedID)
	{
		if (_achievements.Count == 0)
		{
			return null;
		}
		foreach (MMAchievement achievement in _achievements)
		{
			if (achievement.AchievementID == searchedID)
			{
				return achievement;
			}
		}
		return null;
	}

	public static void ResetAchievements(string listID)
	{
		if (_achievements != null)
		{
			foreach (MMAchievement achievement in _achievements)
			{
				achievement.ProgressCurrent = 0;
				achievement.UnlockedStatus = false;
			}
		}
		DeterminePath(listID);
		MMSaveLoadManager.DeleteSave(_saveFileName + ".achievements", "MMAchievements/");
		Debug.LogFormat("Achievements Reset");
	}

	public static void ResetAllAchievements()
	{
		ResetAchievements(_listID);
	}

	public static void LoadSavedAchievements()
	{
		DeterminePath();
		ExtractSerializedMMAchievementManager((SerializedMMAchievementManager)MMSaveLoadManager.Load(typeof(SerializedMMAchievementManager), _saveFileName + ".achievements", "MMAchievements/"));
	}

	public static void SaveAchievements()
	{
		DeterminePath();
		SerializedMMAchievementManager serializedMMAchievementManager = new SerializedMMAchievementManager();
		FillSerializedMMAchievementManager(serializedMMAchievementManager);
		MMSaveLoadManager.Save(serializedMMAchievementManager, _saveFileName + ".achievements", "MMAchievements/");
	}

	private static void DeterminePath(string specifiedFileName = "")
	{
		string saveFileName = ((!string.IsNullOrEmpty(_listID)) ? _listID : "Achievements");
		if (!string.IsNullOrEmpty(specifiedFileName))
		{
			saveFileName = specifiedFileName;
		}
		_saveFileName = saveFileName;
	}

	public static void FillSerializedMMAchievementManager(SerializedMMAchievementManager serializedAchievements)
	{
		serializedAchievements.Achievements = new SerializedMMAchievement[_achievements.Count];
		for (int i = 0; i < _achievements.Count(); i++)
		{
			SerializedMMAchievement serializedMMAchievement = new SerializedMMAchievement(_achievements[i].AchievementID, _achievements[i].UnlockedStatus, _achievements[i].ProgressCurrent);
			serializedAchievements.Achievements[i] = serializedMMAchievement;
		}
	}

	public static void ExtractSerializedMMAchievementManager(SerializedMMAchievementManager serializedAchievements)
	{
		if (serializedAchievements == null)
		{
			return;
		}
		for (int i = 0; i < _achievements.Count(); i++)
		{
			for (int j = 0; j < serializedAchievements.Achievements.Length; j++)
			{
				if (_achievements[i].AchievementID == serializedAchievements.Achievements[j].AchievementID)
				{
					_achievements[i].UnlockedStatus = serializedAchievements.Achievements[j].UnlockedStatus;
					_achievements[i].ProgressCurrent = serializedAchievements.Achievements[j].ProgressCurrent;
				}
			}
		}
	}
}
