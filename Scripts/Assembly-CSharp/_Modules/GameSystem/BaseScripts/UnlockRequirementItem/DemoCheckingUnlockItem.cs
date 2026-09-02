using UnityEngine;

namespace _Modules.GameSystem.BaseScripts.UnlockRequirementItem;

[CreateAssetMenu(fileName = "DemoCheckingUnlockItem", menuName = "WMO/ScriptableObjects/Unlock Item Requirement/DemoCheckingUnlockItem", order = 0)]
public class DemoCheckingUnlockItem : UnlockItemRequirementBaseSO
{
	public override bool CheckRequirement()
	{
		return false;
	}
}
