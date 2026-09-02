using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "Energy Stimulant UpgradeAction Action", menuName = "WMO/ScriptableObjects/Crafting/Crafting Action/EnergyStimulantUpgradeAction", order = 0)]
public class EnergyStimulantUpgradeAction : CraftingBaseAction
{
	[SerializeField]
	private int _addStamina = 20;

	public override void Craft(CraftingManager craftingManager, CraftRecipeScriptableObject craftRecipeSo)
	{
		Action(craftingManager, craftRecipeSo);
	}

	protected override void Action(CraftingManager craftingManager, CraftRecipeScriptableObject craftRecipeSo)
	{
		RemoveMaterial(craftingManager, craftRecipeSo.CraftingIngredientsList);
		craftingManager.PlayerData.AddMaxStamina(_addStamina, isIncreasedByPerks: false);
	}
}
