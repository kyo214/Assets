using System.Collections.Generic;
using UnityEngine;

namespace MoreMountains.Tools;

[CreateAssetMenu(fileName = "AchievementList", menuName = "MoreMountains/Achievement List")]
public class MMAchievementList : ScriptableObject
{
	public string AchievementsListID = "AchievementsList";

	public List<MMAchievement> Achievements;

	public virtual void ResetAchievements()
	{
		Debug.LogFormat("Reset Achievements");
		MMAchievementManager.ResetAchievements(AchievementsListID);
	}
}
