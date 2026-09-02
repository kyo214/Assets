using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "Check Unlock Crafting Item Requirement Checker Action", menuName = "WMO/ScriptableObjects/Crafting/Crafting Checker Action/Check Unlock Crafting Item Requirement Checker Action", order = 0)]
public class CheckUnlockCraftingItemRequirementCheckerAction : CraftingRequirementCheckerAction
{
	[SerializeField]
	private CraftRecipeScriptableObject _craftingSo;

	public override bool CheckRequirement(CraftRecipeScriptableObject craftRecipeScriptableObject, CraftingManager craftingManager)
	{
		return !_craftingSo.CheckRequirement(craftingManager);
	}
}
