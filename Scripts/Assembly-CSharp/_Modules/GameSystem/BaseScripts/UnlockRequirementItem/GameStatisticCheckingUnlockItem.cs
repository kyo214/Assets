using System.Collections.Generic;
using UnityEngine;
using _Modules.Achievement.Scripts;

namespace _Modules.GameSystem.BaseScripts.UnlockRequirementItem;

[CreateAssetMenu(fileName = "GameStatisticCheckingUnlockItem", menuName = "WMO/ScriptableObjects/Unlock Item Requirement/GameStatisticCheckingUnlockItem", order = 0)]
public class GameStatisticCheckingUnlockItem : UnlockItemRequirementBaseSO
{
	[SerializeField]
	private List<AchievementConditionUnlockData> _unlockCondition;

	public override bool CheckRequirement()
	{
		foreach (AchievementConditionUnlockData item in _unlockCondition)
		{
			if (!item.CheckActivation((GlobalSaveData.instance.UserSaveData?.GetGameStatisticProgress(item.gameStatisticData.GetGameStatisticKey()) ?? 0f).ToString()))
			{
				return false;
			}
		}
		return true;
	}
}
