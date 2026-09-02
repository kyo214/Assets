using System;
using System.Collections.Generic;
using Toked.Crafting;
using UnityEngine;

namespace Toked.Inventory;

public class MaterialInventory : MonoBehaviour
{
	[SerializeField]
	private Dictionary<string, MaterialInventoryData> _materialInventoryDic = new Dictionary<string, MaterialInventoryData>();

	public Dictionary<string, MaterialInventoryData> MaterialInventoryDic
	{
		get
		{
			return _materialInventoryDic;
		}
		set
		{
			_materialInventoryDic = value;
		}
	}

	public event Action<CraftMaterialScriptableObject, int> OnChangedMaterialEvent;

	public event Action OnResetMaterialEvent;

	public void ResetMaterial()
	{
		_materialInventoryDic.Clear();
		OnResetMaterialEvent?.Invoke();
	}

	public void SetMaterial(Dictionary<string, MaterialInventoryData> materialInventoryDic)
	{
		foreach (KeyValuePair<string, MaterialInventoryData> item in materialInventoryDic)
		{
			SetMaterial(item.Value.MaterialKey, item.Value.Amount);
		}
	}

	public void SetMaterial(string materialId, int amount)
	{
		CraftMaterialScriptableObject materialSo = GetMaterialSo(materialId);
		if (_materialInventoryDic.ContainsKey(materialId))
		{
			_materialInventoryDic[materialId].Amount = amount;
		}
		else
		{
			_materialInventoryDic.Add(materialId, CreateNewMaterialData(materialSo, amount));
		}
		OnChangedMaterialEvent?.Invoke(materialSo, amount);
	}

	public void SetMaterial(int itemInventoryId, int amount)
	{
		CraftMaterialScriptableObject materialSoByInventoryItemKey = GetMaterialSoByInventoryItemKey(itemInventoryId);
		if ((bool)materialSoByInventoryItemKey)
		{
			if (_materialInventoryDic.ContainsKey(materialSoByInventoryItemKey.ID))
			{
				_materialInventoryDic[materialSoByInventoryItemKey.ID].Amount = amount;
			}
			else
			{
				_materialInventoryDic.Add(materialSoByInventoryItemKey.ID, CreateNewMaterialData(materialSoByInventoryItemKey, amount));
			}
		}
	}

	public void AddMaterial(int itemInventoryId, int amount)
	{
		CraftMaterialScriptableObject materialSoByInventoryItemKey = GetMaterialSoByInventoryItemKey(itemInventoryId);
		AddMaterial(materialSoByInventoryItemKey, amount);
	}

	public void AddMaterial(CraftMaterialScriptableObject materialScriptableObject, int amount)
	{
		MaterialInventoryData materialInventoryData = GetMaterial(materialScriptableObject.ID);
		if (materialInventoryData == null)
		{
			materialInventoryData = CreateNewMaterialData(materialScriptableObject);
			_materialInventoryDic.Add(materialScriptableObject.ID, materialInventoryData);
		}
		materialInventoryData.Add(amount);
		OnChangedMaterialEvent?.Invoke(materialScriptableObject, materialInventoryData.Amount);
	}

	public void RemoveMaterial(int itemInventoryId, int amount)
	{
		CraftMaterialScriptableObject materialSoByInventoryItemKey = GetMaterialSoByInventoryItemKey(itemInventoryId);
		RemoveMaterial(materialSoByInventoryItemKey, amount);
	}

	public void RemoveMaterial(CraftMaterialScriptableObject materialScriptableObject, int amount)
	{
		MaterialInventoryData material = GetMaterial(materialScriptableObject.ID);
		if (material != null)
		{
			material.Remove(amount);
			OnChangedMaterialEvent?.Invoke(materialScriptableObject, material.Amount);
		}
	}

	public MaterialInventoryData GetMaterial(string key)
	{
		_materialInventoryDic.TryGetValue(key, out var value);
		return value;
	}

	public static CraftMaterialScriptableObject GetMaterialSoByInventoryItemKey(int key)
	{
		string id = BGDatabase_CraftMaterial.GetEntityByKeyItemInventoryKey(key).Id;
		return DataManager.Instance.Get<CraftMaterialLibraryScriptableObject>().GetData(id);
	}

	public CraftMaterialScriptableObject GetMaterialSo(string materialId)
	{
		return DataManager.Instance.Get<CraftMaterialLibraryScriptableObject>().GetData(materialId);
	}

	public bool CheckRequirementMaterial(string key, int amount)
	{
		return GetMaterialAmount(key) >= amount;
	}

	public int GetMaterialAmount(string key)
	{
		return GetMaterial(key)?.Amount ?? 0;
	}

	private MaterialInventoryData CreateNewMaterialData(CraftMaterialScriptableObject materialScriptableObject, int amount = 0)
	{
		return new MaterialInventoryData(materialScriptableObject, amount);
	}
}
