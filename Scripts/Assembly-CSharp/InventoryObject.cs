using System;
using System.Collections.Generic;
using Toked.StatusEffect;
using UnityEngine;
using _Modules.CharacterSkin.Scripts;

[Serializable]
public class InventoryObject
{
	[Serializable]
	public class StatusEffectItemObject
	{
		[SerializeField]
		private string _baseName;

		[SerializeField]
		private string _additionalName;

		private StatusEffectScriptableObject _statusEffectSo;

		public string BaseName
		{
			get
			{
				return _baseName;
			}
			set
			{
				_baseName = value;
			}
		}

		public string AdditionalName
		{
			get
			{
				return _additionalName;
			}
			set
			{
				_additionalName = value;
			}
		}

		public StatusEffectScriptableObject StatusEffectSo
		{
			get
			{
				if (_statusEffectSo == null || _statusEffectSo?.StatusEffectData.Name != Name)
				{
					_statusEffectSo = CloneSo(_baseName, _additionalName);
				}
				return _statusEffectSo;
			}
		}

		public string Name
		{
			get
			{
				if (!string.IsNullOrWhiteSpace(_additionalName))
				{
					return _baseName + "_" + _additionalName;
				}
				return _baseName;
			}
		}

		public void SetAdditionalName(params string[] additionalNames)
		{
			if (additionalNames != null && additionalNames.Length != 0)
			{
				_additionalName = string.Join("_", additionalNames);
			}
		}

		public StatusEffectItemObject()
		{
		}

		public StatusEffectItemObject(StatusEffectItemObject statusEffectItemObject)
		{
			Set(statusEffectItemObject.StatusEffectSo ?? CloneSo(statusEffectItemObject.BaseName, statusEffectItemObject.AdditionalName));
		}

		public StatusEffectItemObject(StatusEffectScriptableObject statusEffectSo)
		{
			Set(statusEffectSo);
		}

		private void Set(StatusEffectScriptableObject statusEffectSo)
		{
			_statusEffectSo = statusEffectSo;
			_baseName = statusEffectSo.StatusEffectData.BaseName;
			_additionalName = statusEffectSo.StatusEffectData.AdditionalName;
		}

		private StatusEffectScriptableObject CloneSo(string baseName, string additionalName)
		{
			StatusEffectScriptableObject data = DataManager.Instance.Get<StatusEffectLibraryScriptableObject>().GetData(baseName);
			if (data == null)
			{
				return null;
			}
			StatusEffectScriptableObject statusEffectScriptableObject = data.CloneStatusEffectSO(destroyOnRemove: true);
			statusEffectScriptableObject.StatusEffectData.SetAdditionalName(additionalName);
			return statusEffectScriptableObject;
		}

		public string GetStatusEffectLocalizationName(string separator = " ")
		{
			return _statusEffectSo?.GetStatusEffectLocalizationName() ?? _baseName;
		}
	}

	public int UniqueID = -1;

	public int ID;

	public string Name;

	public int IdxInventory;

	public string ItemType;

	public int Amount;

	public int Ammo;

	public float Durability = -1f;

	public bool IsUsable;

	public bool IsEquippable;

	public int MaxItemInSlot;

	public bool IsCombinable;

	public bool IsOpenable;

	public bool equip;

	public List<StatusEffectItemObject> statusEffects = new List<StatusEffectItemObject>();

	public InventoryObject()
	{
	}

	public InventoryObject(InventoryObject inventoryObject)
	{
		SetInventoryObject(inventoryObject);
	}

	public void SetInventoryObject(InventoryObject inventoryObject)
	{
		UniqueID = inventoryObject.UniqueID;
		ID = inventoryObject.ID;
		Name = inventoryObject.Name;
		IdxInventory = inventoryObject.IdxInventory;
		ItemType = inventoryObject.ItemType;
		Amount = inventoryObject.Amount;
		Ammo = inventoryObject.Ammo;
		Durability = inventoryObject.Durability;
		IsUsable = inventoryObject.IsUsable;
		IsEquippable = inventoryObject.IsEquippable;
		MaxItemInSlot = inventoryObject.MaxItemInSlot;
		IsCombinable = inventoryObject.IsCombinable;
		IsOpenable = inventoryObject.IsOpenable;
		equip = inventoryObject.equip;
		statusEffects = new List<StatusEffectItemObject>();
		foreach (StatusEffectItemObject statusEffect in inventoryObject.statusEffects)
		{
			if (statusEffect != null)
			{
				StatusEffectItemObject statusEffectItemObject = new StatusEffectItemObject(statusEffect);
				if (statusEffectItemObject.StatusEffectSo is IItemEffect itemEffect)
				{
					itemEffect.Init(ID, IdxInventory);
				}
				statusEffects.Add(statusEffectItemObject);
			}
		}
	}

	public void SetStatusEffect(List<StatusEffectScriptableObject> statusEffectSoList)
	{
		statusEffects.Clear();
		foreach (StatusEffectScriptableObject statusEffectSo in statusEffectSoList)
		{
			if (statusEffectSo != null)
			{
				statusEffects.Add(new StatusEffectItemObject(statusEffectSo));
			}
		}
	}

	public void AddSetStatusEffect(StatusEffectScriptableObject statusEffectSo)
	{
		if (statusEffectSo != null)
		{
			statusEffects.Add(new StatusEffectItemObject(statusEffectSo));
		}
	}

	public void AddSetStatusEffectWithItemId(StatusEffectScriptableObject statusEffectSo)
	{
		if (statusEffectSo != null)
		{
			StatusEffectScriptableObject statusEffectScriptableObject = statusEffectSo.CloneStatusEffectSO(destroyOnRemove: true);
			statusEffectScriptableObject.StatusEffectData.SetAdditionalName(ID.ToString(), IdxInventory.ToString());
			if (statusEffectScriptableObject is IItemEffect itemEffect)
			{
				itemEffect.Init(ID, IdxInventory);
			}
			statusEffects.Add(new StatusEffectItemObject(statusEffectScriptableObject));
		}
	}

	public void ResetData()
	{
		Name = "Null";
		ItemType = "Null";
		UniqueID = -1;
		ID = -1;
		Ammo = 0;
		Amount = 0;
		Durability = -1f;
		equip = false;
		statusEffects.Clear();
	}

	public void ResetDataAndRemoveEffect(StatusEffectController statusEffectController)
	{
		RemoveStatusEffect(statusEffectController);
		ResetData();
	}

	public void ApplyStatusEffect(StatusEffectController statusEffectController)
	{
		foreach (StatusEffectItemObject statusEffect in statusEffects)
		{
			statusEffectController?.ApplyStatus(statusEffectController.PlayerController, statusEffect.StatusEffectSo);
		}
	}

	public void RemoveStatusEffect(StatusEffectController statusEffectController)
	{
		foreach (StatusEffectItemObject statusEffect in statusEffects)
		{
			statusEffectController?.ClearStatus(statusEffect.Name);
		}
	}

	public bool HasStatusEffect()
	{
		return statusEffects.Count > 0;
	}

	public string GetEffectLocalization(string separator = " ")
	{
		List<string> list = new List<string>();
		foreach (StatusEffectItemObject statusEffect in statusEffects)
		{
			string statusEffectLocalizationName = statusEffect.GetStatusEffectLocalizationName();
			list.Add(statusEffectLocalizationName);
		}
		string text = string.Join(separator, list);
		if (!string.IsNullOrWhiteSpace(text))
		{
			return " " + text;
		}
		return string.Empty;
	}

	public string GetCurseLocalization(string objectName)
	{
		foreach (StatusEffectItemObject statusEffect in statusEffects)
		{
			if (statusEffect.StatusEffectSo is CurseItemStatusEffectScriptableObjectBase)
			{
				return "<color=#" + IItemEffect.HexColor + ">" + objectName + " " + IItemEffect.GetItemEffectLocalize() + "</color>";
			}
		}
		return objectName;
	}

	public float GetDurabilityPercentage()
	{
		BGDatabase_Item entityByKeyid = BGDatabase_Item.GetEntityByKeyid(ID);
		if (entityByKeyid != null && entityByKeyid.Durability > 0)
		{
			return Durability / (float)entityByKeyid.Durability;
		}
		return 1f;
	}
}
