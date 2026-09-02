using System;
using System.Collections.Generic;
using Toked.Crafting;
using Toked.Crafting.CraftingUI;
using Toked.Inventory;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
	[SerializeField]
	private CraftingRecipesLibraryScriptableObject _craftingRecipesLibrarySO;

	[SerializeField]
	private SkillLearnPopupUI _skillLearnPopupUI;

	[SerializeField]
	private bool _noIngredientsRequire;

	public Action<CraftRecipeScriptableObject, bool> OnCraftItemEvent;

	private PlayerData _playerData;

	private PlayerController _playerController;

	public SkillLearnPopupUI SkillLearnPopupUI => _skillLearnPopupUI;

	public bool NoIngredientsRequire => _noIngredientsRequire;

	public CraftingRecipesLibraryScriptableObject CraftingRecipesLibrarySo
	{
		get
		{
			return _craftingRecipesLibrarySO;
		}
		set
		{
			_craftingRecipesLibrarySO = value;
		}
	}

	public PlayerController PlayerController
	{
		get
		{
			if (_playerController == null)
			{
				_playerController = NetworkGameManager.Instance?.ownPlayer;
			}
			return _playerController;
		}
	}

	public PlayerData PlayerData
	{
		get
		{
			if (_playerData == null)
			{
				_playerData = NetworkGameManager.Instance?.ownPlayer?.data;
			}
			return _playerData;
		}
	}

	public void CraftRecipe(CraftRecipeScriptableObject craftRecipeSO)
	{
		bool flag = CheckIngredientsRequirement(craftRecipeSO.CraftingIngredientsList);
		if (flag)
		{
			if (craftRecipeSO.UseCustomCraftingAction)
			{
				craftRecipeSO.Crafting(this);
			}
			else
			{
				CraftItem(craftRecipeSO);
			}
		}
		OnCraftItemEvent?.Invoke(craftRecipeSO, flag);
	}

	private void CraftItem(CraftRecipeScriptableObject craftRecipeSO)
	{
		RemoveMaterial(craftRecipeSO.CraftingIngredientsList);
		int num = PlayerData.AddInventory(craftRecipeSO.ItemCraftId, isOnPick: true, craftRecipeSO.CraftAmount);
		PlayerController playerController = PlayerData.playerController;
		switch (num)
		{
		case 0:
			playerController.weaponController.EquipWeaponInventory(0);
			break;
		case -1:
			playerController.network.SetSpawnItemAmount(craftRecipeSO.ItemCraftId, playerController.weaponPos.position, craftRecipeSO.CraftAmount, isSpread: true);
			break;
		}
	}

	public void RemoveMaterial(List<CraftingIngredient> craftingIngredients)
	{
		if (_noIngredientsRequire)
		{
			return;
		}
		for (int i = 0; i < craftingIngredients.Count; i++)
		{
			CraftingIngredient craftingIngredient = craftingIngredients[i];
			CraftMaterialScriptableObject craftMaterialScriptableObject = craftingIngredient.CraftMaterialScriptableObject;
			if (craftMaterialScriptableObject.Type == CraftMaterialScriptableObject.MaterialType.Other)
			{
				int amount = (int)((float)craftingIngredient.Amount * PlayerData.MaterialInventoryManager.DiscountCraftMultiply);
				craftMaterialScriptableObject.RemoveMaterial(PlayerData, amount);
			}
			else
			{
				craftMaterialScriptableObject.RemoveMaterial(PlayerData, craftingIngredient.Amount);
			}
		}
	}

	public bool CheckIngredientsRequirement(List<CraftingIngredient> craftingIngredients)
	{
		for (int i = 0; i < craftingIngredients.Count; i++)
		{
			if (!CheckIngredient(craftingIngredients[i]))
			{
				return false;
			}
		}
		return true;
	}

	public bool CheckIngredient(CraftingIngredient craftingIngredients)
	{
		if (_noIngredientsRequire)
		{
			return true;
		}
		CraftMaterialScriptableObject craftMaterialScriptableObject = craftingIngredients.CraftMaterialScriptableObject;
		if (craftMaterialScriptableObject.Type == CraftMaterialScriptableObject.MaterialType.Other)
		{
			int amount = (int)((float)craftingIngredients.Amount * PlayerData.MaterialInventoryManager.DiscountCraftMultiply);
			return craftMaterialScriptableObject.CheckIngredient(PlayerData, amount);
		}
		return craftMaterialScriptableObject.CheckIngredient(PlayerData, craftingIngredients.Amount);
	}

	public void SetNoIngredientsRequire(bool value)
	{
		_noIngredientsRequire = value;
	}

	private MaterialInventory GetMaterialInventory()
	{
		return PlayerData.MainMaterialInventory;
	}
}
