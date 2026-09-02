using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "Hip Pouch Requirement Checker Action", menuName = "WMO/ScriptableObjects/Crafting/Crafting Checker Action/Hip Pouch Upgrade Checker Action", order = 0)]
public class HipPouchRequirementCheckerAction : CraftingRequirementCheckerAction
{
	[SerializeField]
	private Vector2 _minMaxRequirementInventorySlot;

	public override bool CheckRequirement(CraftRecipeScriptableObject craftRecipeScriptableObject, CraftingManager craftingManager)
	{
		int maxInventory = craftingManager.PlayerData.GetMaxInventory();
		if (maxInventory >= (int)_minMaxRequirementInventorySlot.x)
		{
			return maxInventory <= (int)_minMaxRequirementInventorySlot.y;
		}
		return false;
	}
}
