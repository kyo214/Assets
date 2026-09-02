using System.Collections;
using I2.Loc;
using Sirenix.OdinInspector;
using Toked.Inventory;
using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "CraftMaterialScriptableObject", menuName = "WMO/ScriptableObjects/Crafting/CraftMaterialScriptableObject", order = 0)]
public class CraftMaterialScriptableObject : ScriptableObject
{
	public enum MaterialType
	{
		Other = 0,
		Weapon = 1,
		Heal = 2,
		Item = 3,
		SkillPoint = 4
	}

	[SerializeField]
	private int _sortOrderId;

	[SerializeField]
	private string _id;

	[SerializeField]
	private int _itemInventoryId;

	[SerializeField]
	private string _name;

	[SerializeField]
	private MaterialType _materialType;

	[SerializeField]
	[TermsPopup("")]
	private string _materialNameLocalizeId;

	[SerializeField]
	[TermsPopup("")]
	private string _materialDescriptionLocalizeId;

	[SerializeField]
	private Sprite _materialSprite;

	[SerializeField]
	private Vector2 _minMaxDropAmount = new Vector2(1f, 1f);

	public int SortOrderId
	{
		get
		{
			return _sortOrderId;
		}
		set
		{
			_sortOrderId = value;
		}
	}

	public string ID
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
		}
	}

	public int ItemInventoryId
	{
		get
		{
			return _itemInventoryId;
		}
		set
		{
			_itemInventoryId = value;
		}
	}

	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}

	public MaterialType Type
	{
		get
		{
			return _materialType;
		}
		set
		{
			_materialType = value;
		}
	}

	public string MaterialNameLocalizeId
	{
		get
		{
			return _materialNameLocalizeId;
		}
		set
		{
			_materialNameLocalizeId = value;
		}
	}

	public string MaterialDescriptionLocalizeId
	{
		get
		{
			return _materialDescriptionLocalizeId;
		}
		set
		{
			_materialDescriptionLocalizeId = value;
		}
	}

	public Sprite MaterialSprite
	{
		get
		{
			return _materialSprite;
		}
		set
		{
			_materialSprite = value;
		}
	}

	public Vector2 MinMaxDropAmount
	{
		get
		{
			return _minMaxDropAmount;
		}
		set
		{
			_minMaxDropAmount = value;
		}
	}

	private static IEnumerable GetItemId()
	{
		ValueDropdownList<int> result = new ValueDropdownList<int>();
		result.Add("None", -1);
		BGDatabase_Ammunition.ForEachEntity((BGDatabase_Ammunition data) =>
		{
			AddToList("Ammunition/" + data.Name, data.Keys);
		});
		BGDatabase_Weapon.ForEachEntity((BGDatabase_Weapon data) =>
		{
			AddToList("Weapon/" + data.Name, data.Keys);
		});
		BGDatabase_Item.ForEachEntity((BGDatabase_Item data) =>
		{
			AddToList("Item/" + data.Name, data.Keys);
		});
		return result;
		void AddToList(string inspectorName, int value)
		{
			result.Add(inspectorName, value);
		}
	}

	public void RemoveMaterial(PlayerData playerData, int amount)
	{
		switch (Type)
		{
		case MaterialType.Other:
			playerData.GetCurrentMaterialInventory().RemoveMaterial(this, amount);
			return;
		case MaterialType.SkillPoint:
			playerData.RemoveSkillPoint(amount);
			return;
		}
		InventoryObject inventoryObject = playerData.FindInventory(_itemInventoryId);
		if (inventoryObject != null)
		{
			playerData.RemoveInventory(inventoryObject.IdxInventory, syncNetwork: true, duplicateItem: false, amount);
		}
	}

	public void AddMaterial(PlayerData playerData, int amount, MaterialInventoryManager.InventoryType inventoryType = MaterialInventoryManager.InventoryType.Auto)
	{
		switch (Type)
		{
		case MaterialType.Other:
			playerData.MaterialInventoryManager.AddMaterial(inventoryType, this, amount);
			break;
		case MaterialType.SkillPoint:
			playerData.AddSkillPoint(amount);
			break;
		default:
			playerData.AddInventory(_itemInventoryId, isOnPick: false, amount);
			break;
		}
	}

	public int GetMaterialAmount(PlayerData playerData, MaterialInventoryManager.InventoryType inventoryType = MaterialInventoryManager.InventoryType.Auto)
	{
		return Type switch
		{
			MaterialType.Other => playerData.MaterialInventoryManager.GetMaterialInventory(inventoryType).GetMaterialAmount(_id), 
			MaterialType.SkillPoint => playerData.GetSkillPoint(), 
			_ => playerData.FindTotalInventory(_itemInventoryId), 
		};
	}

	public bool CheckIngredient(PlayerData playerData, int amount)
	{
		return Type switch
		{
			MaterialType.Other => playerData.GetCurrentMaterialInventory().CheckRequirementMaterial(ID, amount), 
			MaterialType.SkillPoint => playerData.CheckSkillPoint(amount), 
			_ => playerData.CheckInventory(ItemInventoryId, amount), 
		};
	}
}
