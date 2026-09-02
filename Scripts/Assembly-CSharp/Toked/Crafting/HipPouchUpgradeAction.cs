using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "Hip Pouch Upgrade Action", menuName = "WMO/ScriptableObjects/Crafting/Crafting Action/Hip Pouch Upgrade Action", order = 0)]
public class HipPouchUpgradeAction : CraftingBaseAction
{
	[SerializeField]
	private int _addSlot = 2;

	public override void Craft(CraftingManager craftingManager, CraftRecipeScriptableObject craftRecipeSo)
	{
		Action(craftingManager, craftRecipeSo);
	}

	protected override void Action(CraftingManager craftingManager, CraftRecipeScriptableObject craftRecipeSo)
	{
		if (!craftingManager.PlayerData.IsMaxSlotInventory())
		{
			RemoveMaterial(craftingManager, craftRecipeSo.CraftingIngredientsList);
			craftingManager.PlayerData.AddSlotInventory(_addSlot);
			AudioManager.PlaySFX("herb_pickup");
			if (UIGameManager.Instance != null)
			{
				craftingManager.PlayerData.InitImageInventoryLocal();
			}
		}
	}
}
