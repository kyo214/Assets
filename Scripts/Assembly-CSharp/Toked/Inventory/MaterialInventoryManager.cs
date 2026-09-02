using System;
using System.Collections.Generic;
using Toked.Crafting;
using UnityEngine;

namespace Toked.Inventory;

public class MaterialInventoryManager : MonoBehaviour
{
	public enum InventoryType
	{
		Auto = 0,
		Main = 1,
		InGame = 2
	}

	[SerializeField]
	private float _discountCraft;

	[SerializeField]
	private float _bonusLootMaterial;

	[SerializeField]
	private MaterialInventoryNetwork _mainMaterialInventoryNetwork;

	[SerializeField]
	private MaterialInventoryNetwork _inGameMaterialInventoryNetwork;

	public Action<float> OnDiscountCraftChangedEvent;

	public Action<float> OnBonusLootMaterialChangedEvent;

	public float DiscountCraft => _discountCraft;

	public float DiscountCraftMultiply => 1f - _discountCraft;

	public float BonusLootMaterialMultiply => 1f + _bonusLootMaterial;

	public MaterialInventory MainMaterialInventory => _mainMaterialInventoryNetwork.MaterialInventory;

	public MaterialInventory InGameMaterialInventory => _inGameMaterialInventoryNetwork.MaterialInventory;

	public MaterialInventoryNetwork InGameMaterialNetwork => _inGameMaterialInventoryNetwork;

	public void SetDiscountCraft(float discountCraft, bool executeEvent = true)
	{
		_discountCraft = discountCraft;
		if (executeEvent)
		{
			OnDiscountCraftChangedEvent?.Invoke(discountCraft);
		}
	}

	public void SetBonusLootMaterial(float bonusLootMaterial, bool executeEvent = true)
	{
		_bonusLootMaterial = bonusLootMaterial;
		if (executeEvent)
		{
			OnBonusLootMaterialChangedEvent?.Invoke(bonusLootMaterial);
		}
	}

	public void ResetMainMaterialInventory()
	{
		MainMaterialInventory.ResetMaterial();
	}

	public void ResetInGameMaterialInventory()
	{
		InGameMaterialInventory.ResetMaterial();
	}

	public void TransferMaterialToMainInventory()
	{
		foreach (KeyValuePair<string, MaterialInventoryData> item in InGameMaterialInventory.MaterialInventoryDic)
		{
			CraftMaterialScriptableObject craftMaterialScriptableObject = item.Value.CraftMaterialScriptableObject;
			if (craftMaterialScriptableObject.Type == CraftMaterialScriptableObject.MaterialType.Other)
			{
				MainMaterialInventory.AddMaterial(craftMaterialScriptableObject, item.Value.Amount);
			}
		}
		SyncMainMaterialInventory();
	}

	public void TransferMaterialToInGameInventory()
	{
		InGameMaterialInventory.ResetMaterial();
		foreach (KeyValuePair<string, MaterialInventoryData> item in MainMaterialInventory.MaterialInventoryDic)
		{
			CraftMaterialScriptableObject craftMaterialScriptableObject = item.Value.CraftMaterialScriptableObject;
			if (craftMaterialScriptableObject.Type == CraftMaterialScriptableObject.MaterialType.Other)
			{
				InGameMaterialInventory.AddMaterial(craftMaterialScriptableObject, item.Value.Amount);
			}
		}
		SyncInGameMaterialInventory();
	}

	public void TransferMaterialToMainInventory(Dictionary<string, MaterialInventoryData> totalMaterialInventoryDic)
	{
		foreach (KeyValuePair<string, MaterialInventoryData> item in totalMaterialInventoryDic)
		{
			CraftMaterialScriptableObject craftMaterialScriptableObject = item.Value.CraftMaterialScriptableObject;
			if (craftMaterialScriptableObject.Type == CraftMaterialScriptableObject.MaterialType.Other)
			{
				MainMaterialInventory.AddMaterial(craftMaterialScriptableObject, item.Value.Amount);
			}
		}
		SyncMainMaterialInventory();
	}

	public void SyncInGameMaterialInventory()
	{
		_inGameMaterialInventoryNetwork.SyncToLocalData();
	}

	public void SyncMainMaterialInventory()
	{
		_mainMaterialInventoryNetwork.SyncToLocalData();
	}

	public Dictionary<string, MaterialInventoryData> GetInGameMaterialData(bool fromLocal = true)
	{
		if (!fromLocal)
		{
			return _inGameMaterialInventoryNetwork.GetMaterialData();
		}
		return InGameMaterialInventory.MaterialInventoryDic;
	}

	public Dictionary<string, MaterialInventoryData> GetMainMaterialData(bool fromLocal = true)
	{
		if (!fromLocal)
		{
			return _mainMaterialInventoryNetwork.GetMaterialData();
		}
		return MainMaterialInventory.MaterialInventoryDic;
	}

	public void AddMaterial(InventoryType inventoryType, CraftMaterialScriptableObject so, int amount)
	{
		int amount2 = (int)((float)amount * BonusLootMaterialMultiply);
		GetMaterialInventory(inventoryType).AddMaterial(so, amount2);
	}

	public void AddMaterial(InventoryType inventoryType, int id, int amount)
	{
		int amount2 = (int)((float)amount * BonusLootMaterialMultiply);
		GetMaterialInventory(inventoryType).AddMaterial(id, amount2);
	}

	public MaterialInventory GetMaterialInventory(InventoryType inventoryType = InventoryType.Auto)
	{
		return inventoryType switch
		{
			InventoryType.Auto => GetCurrentMaterialInventory(), 
			InventoryType.Main => MainMaterialInventory, 
			InventoryType.InGame => InGameMaterialInventory, 
			_ => GetCurrentMaterialInventory(), 
		};
	}

	private MaterialInventory GetCurrentMaterialInventory()
	{
		if ((bool)LobbyManager.Instance)
		{
			return MainMaterialInventory;
		}
		return InGameMaterialInventory;
	}
}
