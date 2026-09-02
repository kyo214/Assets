using System.Collections.Generic;
using UnityEngine;

namespace Toked.Crafting;

public abstract class CraftingBaseAction : ScriptableObject, ICraftingAction
{
	public virtual void Craft(CraftingManager craftingManager, CraftRecipeScriptableObject craftRecipeSo)
	{
		RemoveMaterial(craftingManager, craftRecipeSo.CraftingIngredientsList);
		Action(craftingManager, craftRecipeSo);
	}

	protected abstract void Action(CraftingManager craftingManager, CraftRecipeScriptableObject craftRecipeSo);

	protected void RemoveMaterial(CraftingManager craftingManager, List<CraftingIngredient> craftingIngredients)
	{
		craftingManager.RemoveMaterial(craftingIngredients);
	}
}
