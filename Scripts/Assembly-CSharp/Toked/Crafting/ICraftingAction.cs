namespace Toked.Crafting;

public interface ICraftingAction
{
	void Craft(CraftingManager craftingManager, CraftRecipeScriptableObject craftRecipeSo);
}
