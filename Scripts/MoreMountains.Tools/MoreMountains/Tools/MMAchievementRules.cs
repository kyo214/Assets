using UnityEngine;

namespace MoreMountains.Tools;

public abstract class MMAchievementRules : MonoBehaviour, MMEventListener<MMGameEvent>, MMEventListenerBase
{
	public MMAchievementList AchievementList;

	[MMInspectorButton("PrintCurrentStatus")]
	public bool PrintCurrentStatusBtn;

	public virtual void PrintCurrentStatus()
	{
		foreach (MMAchievement achievements in MMAchievementManager.AchievementsList)
		{
			string text = (achievements.UnlockedStatus ? "unlocked" : "locked");
			Debug.Log("[" + achievements.AchievementID + "] " + achievements.Title + ", status : " + text + ", progress : " + achievements.ProgressCurrent + "/" + achievements.ProgressTarget);
		}
	}

	protected virtual void Awake()
	{
		MMAchievementManager.LoadAchievementList(AchievementList);
		MMAchievementManager.LoadSavedAchievements();
	}

	protected virtual void OnEnable()
	{
		this.MMEventStartListening();
	}

	protected virtual void OnDisable()
	{
		this.MMEventStopListening();
	}

	public virtual void OnMMEvent(MMGameEvent gameEvent)
	{
		if (gameEvent.EventName == "Save")
		{
			MMAchievementManager.SaveAchievements();
		}
	}
}
