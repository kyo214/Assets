using System;
using UnityEngine;

namespace _Modules.Achievement.Scripts;

[Serializable]
public class AchievementSaveData
{
	[SerializeField]
	private string _id;

	[SerializeField]
	private float[] _progressList;

	[SerializeField]
	private bool _completed;

	[SerializeField]
	private bool _isClaimed;

	public string Id
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
		}
	}

	public float[] ProgressList
	{
		get
		{
			return _progressList;
		}
		set
		{
			_progressList = value;
		}
	}

	public bool Completed
	{
		get
		{
			return _completed;
		}
		set
		{
			_completed = value;
		}
	}

	public bool IsClaimed
	{
		get
		{
			return _isClaimed;
		}
		set
		{
			_isClaimed = value;
		}
	}

	public AchievementSaveData()
	{
	}

	public AchievementSaveData(AchievementDataSO achievementData)
	{
		Id = achievementData.AchievementId;
		ProgressList = new float[achievementData.UnlockCondition.Length];
		IsClaimed = false;
	}

	public float CalculateTotalProgress()
	{
		float num = 0f;
		for (int num2 = ProgressList.Length - 1; num2 >= 0; num2--)
		{
			num += ProgressList[num2];
		}
		return num;
	}
}
