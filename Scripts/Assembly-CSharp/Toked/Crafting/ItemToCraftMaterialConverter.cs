using System;
using System.Collections.Generic;
using Toked.Inventory;
using UnityEngine;

namespace Toked.Crafting;

public class ItemToCraftMaterialConverter
{
	[Serializable]
	public class ConvertMaterialItemData
	{
		[SerializeField]
		private Dictionary<string, MaterialInventoryData> _material = new Dictionary<string, MaterialInventoryData>();

		[SerializeField]
		private InventoryObject _inventoryObject;

		public Dictionary<string, MaterialInventoryData> Material => _material;

		public InventoryObject InventoryObject => _inventoryObject;

		public ConvertMaterialItemData(int id, int amount)
		{
			_inventoryObject = new InventoryObject();
			_inventoryObject.IdxInventory = -1;
			_inventoryObject.ItemType = GetItemType(id);
			_inventoryObject.ID = id;
			_inventoryObject.Amount = amount;
		}

		public ConvertMaterialItemData(InventoryObject inventoryObject)
		{
			SetInventory(inventoryObject);
		}

		public void SetInventory(InventoryObject inventoryObject)
		{
			_inventoryObject = new InventoryObject();
			_inventoryObject.IdxInventory = inventoryObject.IdxInventory;
			_inventoryObject.ItemType = inventoryObject.ItemType;
			_inventoryObject.ID = inventoryObject.ID;
			_inventoryObject.Amount = inventoryObject.Amount;
		}

		public void AddMaterial(CraftMaterialScriptableObject so, int amount)
		{
			if (_material.ContainsKey(so.ID))
			{
				_material[so.ID].Amount += amount;
			}
			else
			{
				_material.Add(so.ID, new MaterialInventoryData(so, amount));
			}
		}

		private string GetItemType(int id)
		{
			if (id >= 200)
			{
				if (id < 400)
				{
					if (id >= 300)
					{
						return "Item";
					}
					return "HealingItem";
				}
				return "Material";
			}
			if (id >= 100)
			{
				return "Ammunition";
			}
			return "Weapon";
		}
	}

	private const float DISMANTLE_MODIFIER = 0.5f;

	private static readonly List<string> ITEMCONVERT_DEFAULTVALUE = new List<string> { "Scraps=3" };

	public static List<ConvertMaterialItemData> ConvertItemToCraftMaterial(PlayerController playerController)
	{
		List<ConvertMaterialItemData> list = new List<ConvertMaterialItemData>();
		for (int i = 0; i < playerController.data.arrInventory.Count; i++)
		{
			ConvertMaterialItemData convertMaterialItemData = ConvertItemToCraftMaterial(playerController.data.arrInventory[i]);
			if (convertMaterialItemData != null)
			{
				list.Add(convertMaterialItemData);
				playerController.data.RemoveInventoryData(i, syncNetwork: false);
			}
		}
		return list;
	}

	public static ConvertMaterialItemData ConvertItemToCraftMaterial(InventoryObject inventory)
	{
		ConvertMaterialItemData convertMaterialItemData = null;
		if (inventory.ID == -1 || inventory.Amount == 0)
		{
			return convertMaterialItemData;
		}
		BGDatabase_Item entityByKeyid = BGDatabase_Item.GetEntityByKeyid(inventory.ID);
		if (entityByKeyid != null && entityByKeyid.CanConvertToMaterial)
		{
			convertMaterialItemData = new ConvertMaterialItemData(inventory);
			foreach (CraftingIngredient item in CraftingRecipesLibraryScriptableObject.CraftingIngredientsParser(entityByKeyid.ConvertMaterial ?? ITEMCONVERT_DEFAULTVALUE))
			{
				CraftMaterialScriptableObject craftMaterialScriptableObject = item.CraftMaterialScriptableObject;
				if (craftMaterialScriptableObject != null)
				{
					int amount = item.Amount * inventory.Amount;
					convertMaterialItemData.AddMaterial(craftMaterialScriptableObject, amount);
				}
			}
		}
		return convertMaterialItemData;
	}

	public static ConvertMaterialItemData ConvertItemToCraftMaterial(int id, int amount = 1)
	{
		ConvertMaterialItemData convertMaterialItemData = null;
		if (id == -1 || amount == 0)
		{
			return convertMaterialItemData;
		}
		BGDatabase_Item entityByKeyid = BGDatabase_Item.GetEntityByKeyid(id);
		if (entityByKeyid != null && entityByKeyid.CanConvertToMaterial)
		{
			convertMaterialItemData = new ConvertMaterialItemData(id, amount);
			foreach (CraftingIngredient item in CraftingRecipesLibraryScriptableObject.CraftingIngredientsParser(entityByKeyid.ConvertMaterial ?? ITEMCONVERT_DEFAULTVALUE))
			{
				CraftMaterialScriptableObject craftMaterialScriptableObject = item.CraftMaterialScriptableObject;
				if (craftMaterialScriptableObject != null)
				{
					int amount2 = item.Amount * amount;
					convertMaterialItemData.AddMaterial(craftMaterialScriptableObject, amount2);
				}
			}
		}
		return convertMaterialItemData;
	}

	public static ConvertMaterialItemData DismantleItemToCraftMaterial(InventoryObject inventory)
	{
		ConvertMaterialItemData convertItemData = null;
		if (inventory.ID == -1 || inventory.Amount == 0)
		{
			return convertItemData;
		}
		BGDatabase_ItemDismantle convertData = BGDatabase_ItemDismantle.GetEntityByKeyItem(inventory.ID);
		if (convertData != null)
		{
			convertItemData = new ConvertMaterialItemData(inventory);
			if (convertData.UseCustomMaterialValue)
			{
				CalculateMaterial(CraftingRecipesLibraryScriptableObject.CraftingIngredientsParser(convertData.Material));
			}
			else
			{
				foreach (BGDatabase_CraftRecipe item in convertData.MaterialValue)
				{
					CalculateMaterial(CraftingRecipesLibraryScriptableObject.CraftingIngredientsParser(item.CraftingIngredientsList), 0.5f);
				}
				if (convertData.AdditionalMaterialValue)
				{
					CalculateMaterial(CraftingRecipesLibraryScriptableObject.CraftingIngredientsParser(convertData.Material));
				}
			}
		}
		return convertItemData;
		void CalculateMaterial(List<CraftingIngredient> materialList, float modifier = 1f)
		{
			modifier *= (convertData.UseDurability ? inventory.GetDurabilityPercentage() : 1f);
			foreach (CraftingIngredient material in materialList)
			{
				CraftMaterialScriptableObject craftMaterialScriptableObject = material.CraftMaterialScriptableObject;
				if (craftMaterialScriptableObject != null)
				{
					int amount = (int)((float)material.Amount * modifier) * inventory.Amount;
					convertItemData.AddMaterial(craftMaterialScriptableObject, amount);
				}
			}
		}
	}

	public static ConvertMaterialItemData DismantleItemToCraftMaterial(int id, int amount = 1)
	{
		ConvertMaterialItemData convertItemData = null;
		if (id == -1 || amount == 0)
		{
			return convertItemData;
		}
		BGDatabase_ItemDismantle entityByKeyItem = BGDatabase_ItemDismantle.GetEntityByKeyItem(id);
		if (entityByKeyItem != null)
		{
			convertItemData = new ConvertMaterialItemData(id, amount);
			if (entityByKeyItem.UseCustomMaterialValue)
			{
				CalculateMaterial(CraftingRecipesLibraryScriptableObject.CraftingIngredientsParser(entityByKeyItem.Material));
			}
			else
			{
				foreach (BGDatabase_CraftRecipe item in entityByKeyItem.MaterialValue)
				{
					CalculateMaterial(CraftingRecipesLibraryScriptableObject.CraftingIngredientsParser(item.CraftingIngredientsList), 0.5f);
				}
				if (entityByKeyItem.AdditionalMaterialValue)
				{
					CalculateMaterial(CraftingRecipesLibraryScriptableObject.CraftingIngredientsParser(entityByKeyItem.Material));
				}
			}
		}
		return convertItemData;
		void CalculateMaterial(List<CraftingIngredient> materialList, float modifier = 1f)
		{
			foreach (CraftingIngredient material in materialList)
			{
				CraftMaterialScriptableObject craftMaterialScriptableObject = material.CraftMaterialScriptableObject;
				if (craftMaterialScriptableObject != null)
				{
					int amount2 = (int)((float)material.Amount * modifier) * amount;
					convertItemData.AddMaterial(craftMaterialScriptableObject, amount2);
				}
			}
		}
	}
}
