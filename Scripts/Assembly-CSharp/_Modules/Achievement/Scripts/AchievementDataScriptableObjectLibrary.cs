using System;
using UnityEngine;

namespace _Modules.Achievement.Scripts;

[CreateAssetMenu(fileName = "AchievementDataScriptableObjectLibrary", menuName = "WMO/ScriptableObjects/Achievement/AchievementDataScriptableObjectLibrary", order = 0)]
public class AchievementDataScriptableObjectLibrary : ScriptableObjectLibraryBase<AchievementDataSO>
{
	protected override void UpdateData(AchievementDataSO data)
	{
		throw new NotImplementedException();
	}

	public override void SortData()
	{
		base.DataList.Sort((AchievementDataSO x, AchievementDataSO y) => x.OrderNumber.CompareTo(y.OrderNumber));
	}
}
