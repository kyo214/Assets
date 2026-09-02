using System;
using Toked.Crafting;
using UnityEngine;

namespace Toked.Inventory;

[Serializable]
public class MaterialInventoryData
{
	[SerializeField]
	private string _materialKey;

	[SerializeField]
	private int _amount;

	[NonSerialized]
	private CraftMaterialScriptableObject _craftMaterialScriptableObject;

	public string MaterialKey
	{
		get
		{
			return _materialKey;
		}
		set
		{
			_materialKey = value;
		}
	}

	public int Amount
	{
		get
		{
			return _amount;
		}
		set
		{
			_amount = value;
		}
	}

	public CraftMaterialScriptableObject CraftMaterialScriptableObject
	{
		get
		{
			if (_craftMaterialScriptableObject == null)
			{
				_craftMaterialScriptableObject = DataManager.Instance.Get<CraftMaterialLibraryScriptableObject>().GetData(_materialKey);
			}
			else if (_craftMaterialScriptableObject.ID != _materialKey)
			{
				_craftMaterialScriptableObject = DataManager.Instance.Get<CraftMaterialLibraryScriptableObject>().GetData(_materialKey);
			}
			return _craftMaterialScriptableObject;
		}
		set
		{
			_craftMaterialScriptableObject = value;
		}
	}

	public MaterialInventoryData Clone()
	{
		return new MaterialInventoryData
		{
			MaterialKey = MaterialKey,
			Amount = Amount,
			CraftMaterialScriptableObject = CraftMaterialScriptableObject
		};
	}

	public void Add(int amount = 1)
	{
		_amount += amount;
	}

	public void Remove(int amount = 1)
	{
		int num = _amount - amount;
		_amount = Math.Clamp(num, 0, num);
	}

	public MaterialInventoryData()
	{
	}

	public MaterialInventoryData(CraftMaterialScriptableObject materialScriptableObject, int amount = 0)
	{
		CraftMaterialScriptableObject = materialScriptableObject;
		MaterialKey = materialScriptableObject.ID;
		Amount = amount;
	}

	public MaterialInventoryData(MaterialInventoryData materialInventoryData)
	{
		CraftMaterialScriptableObject = materialInventoryData.CraftMaterialScriptableObject;
		MaterialKey = materialInventoryData.MaterialKey;
		Amount = materialInventoryData.Amount;
	}
}
