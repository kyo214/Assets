using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "Life Stimulant UpgradeAction Action", menuName = "WMO/ScriptableObjects/Crafting/Crafting Action/LifeStimulantUpgradeAction", order = 0)]
public class LifeStimulantUpgradeAction : CraftingBaseAction
{
	[SerializeField]
	private int _addHealth = 20;

	public override void Craft(CraftingManager craftingManager, CraftRecipeScriptableObject craftRecipeSo)
	{
		Action(craftingManager, craftRecipeSo);
	}

	protected override void Action(CraftingManager craftingManager, CraftRecipeScriptableObject craftRecipeSo)
	{
		RemoveMaterial(craftingManager, craftRecipeSo.CraftingIngredientsList);
		craftingManager.PlayerData.AddMaxHealth(_addHealth, isIncreassedByPerks: false);
	}
}
