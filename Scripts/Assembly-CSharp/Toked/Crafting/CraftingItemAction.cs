using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "Crafting Action", menuName = "WMO/ScriptableObjects/Crafting/Crafting Action/Default Crafting Action", order = 0)]
public abstract class CraftingItemAction : CraftingBaseAction
{
	protected override void Action(CraftingManager craftingManager, CraftRecipeScriptableObject craftRecipeSo)
	{
		if (craftingManager.PlayerData.AddInventory(craftRecipeSo.ItemCraftId, isOnPick: true, craftRecipeSo.CraftAmount) == -1)
		{
			PlayerController playerController = craftingManager.PlayerData.playerController;
			playerController.network.SetSpawnItemAmount(craftRecipeSo.ItemCraftId, playerController.weaponPos.position, craftRecipeSo.CraftAmount, isSpread: true);
		}
	}
}
