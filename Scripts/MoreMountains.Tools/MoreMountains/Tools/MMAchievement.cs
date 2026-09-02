using System;
using UnityEngine;

namespace MoreMountains.Tools;

[Serializable]
public class MMAchievement
{
	[Header("Identification")]
	public string AchievementID;

	public AchievementTypes AchievementType;

	public bool HiddenAchievement;

	public bool UnlockedStatus;

	[Header("Description")]
	public string Title;

	public string Description;

	public int Points;

	[Header("Image and Sounds")]
	public Sprite LockedImage;

	public Sprite UnlockedImage;

	public AudioClip UnlockedSound;

	[Header("Progress")]
	public int ProgressTarget;

	public int ProgressCurrent;

	protected MMAchievementDisplayItem _achievementDisplayItem;

	public virtual void UnlockAchievement()
	{
		if (!UnlockedStatus)
		{
			UnlockedStatus = true;
			MMGameEvent.Trigger("Save");
			MMAchievementUnlockedEvent.Trigger(this);
		}
	}

	public virtual void LockAchievement()
	{
		UnlockedStatus = false;
	}

	public virtual void AddProgress(int newProgress)
	{
		ProgressCurrent += newProgress;
		EvaluateProgress();
	}

	public virtual void SetProgress(int newProgress)
	{
		ProgressCurrent = newProgress;
		EvaluateProgress();
	}

	protected virtual void EvaluateProgress()
	{
		MMAchievementChangedEvent.Trigger(this);
		if (ProgressCurrent >= ProgressTarget)
		{
			ProgressCurrent = ProgressTarget;
			UnlockAchievement();
		}
	}

	public virtual MMAchievement Copy()
	{
		new MMAchievement();
		return JsonUtility.FromJson<MMAchievement>(JsonUtility.ToJson(this));
	}
}
