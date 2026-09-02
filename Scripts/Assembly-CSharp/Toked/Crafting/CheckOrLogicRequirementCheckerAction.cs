using System.Collections.Generic;
using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "Check Or Logic Requirement Checker Action", menuName = "WMO/ScriptableObjects/Crafting/Crafting Checker Action/Logic/Check Or Logic Requirement Checker Action", order = 0)]
public class CheckOrLogicRequirementCheckerAction : CraftingRequirementCheckerAction
{
	[SerializeField]
	private List<CraftingRequirementCheckerAction> _craftingRequirementCheckerActions;

	public override bool CheckRequirement(CraftRecipeScriptableObject craftRecipeScriptableObject, CraftingManager craftingManager)
	{
		foreach (CraftingRequirementCheckerAction craftingRequirementCheckerAction in _craftingRequirementCheckerActions)
		{
			if (!(craftingRequirementCheckerAction == null) && craftingRequirementCheckerAction.CheckRequirement(craftRecipeScriptableObject, craftingManager))
			{
				return true;
			}
		}
		return false;
	}
}
