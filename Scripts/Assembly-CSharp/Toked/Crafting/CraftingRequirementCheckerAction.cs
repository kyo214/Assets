using UnityEngine;

namespace Toked.Crafting;

public abstract class CraftingRequirementCheckerAction : ScriptableObject
{
	public abstract bool CheckRequirement(CraftRecipeScriptableObject craftRecipeScriptableObject, CraftingManager craftingManager);
}
