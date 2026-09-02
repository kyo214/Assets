using System.Collections.Generic;
using System.Linq;
using DarkTonic.MasterAudio;
using Toked;
using Toked.Crafting;
using UnityEngine;

namespace _Modules.Player.BaseScripts;

public class ArmorManager : MonoBehaviour
{
	[SerializeField]
	private PlayerController _playerController;

	[SerializeField]
	private List<ItemScriptableObject> _itemArmorList = new List<ItemScriptableObject>();

	private float _currentArmor;

	[SoundGroup]
	[SerializeField]
	private string _brokenKevlarSfx;

	public float CalculateDamage(float damage)
	{
		List<int> toRemove = new List<int>();
		float num = damage * -1f;
		foreach (InventoryObject item in _playerController.data.arrInventory)
		{
			if (item.ID != -1 && CheckIsArmor(item))
			{
				float num2 = Mathf.Clamp(num, 0f, item.Durability);
				num -= num2;
				item.Durability -= num2;
				CheckBrokenArmor(item);
				SyncDurability(item);
				UpdateUI(item);
				if (num <= 0f)
				{
					break;
				}
			}
		}
		foreach (int item2 in toRemove)
		{
			_playerController.data.RemoveInventory(item2);
		}
		UpdateCurrentArmor();
		return num * -1f;
		void CheckBrokenArmor(InventoryObject inventoryObject)
		{
			if (CheckInventoryBrokenArmor(inventoryObject))
			{
				toRemove.Add(inventoryObject.IdxInventory);
			}
		}
		void SyncDurability(InventoryObject inventoryObject)
		{
			_playerController.network.playerPhoton.RPCSyncDurability(inventoryObject.IdxInventory, inventoryObject.Durability);
		}
	}

	public bool CheckInventoryBrokenArmor(InventoryObject inventoryObject, bool instantRemove = false)
	{
		if (inventoryObject.Durability <= 0f)
		{
			AudioManager.PlaySFX(_brokenKevlarSfx);
			inventoryObject.Durability = 0f;
			if (instantRemove)
			{
				_playerController.data.RemoveInventory(inventoryObject.IdxInventory);
			}
			return true;
		}
		return false;
	}

	public void SyncArmorManager(InventoryObject inventoryObject)
	{
		CheckInventoryBrokenArmor(inventoryObject, instantRemove: true);
		UpdateCurrentArmor();
		UpdateUI(inventoryObject);
	}

	public float GetTotalArmor(params int[] exceptionUid)
	{
		float num = 0f;
		foreach (InventoryObject item in _playerController.data.arrInventory)
		{
			if (item.ID != -1 && CheckIsArmor(item, exceptionUid))
			{
				num += item.Durability;
			}
		}
		return num;
	}

	private bool CheckIsArmor(InventoryObject inventoryObject, params int[] exceptionUid)
	{
		if (exceptionUid != null && exceptionUid.Contains(inventoryObject.IdxInventory))
		{
			return false;
		}
		return _itemArmorList.Any((ItemScriptableObject x) => x.ID == inventoryObject.ID.ToString() && inventoryObject.equip);
	}

	public void UpdateCurrentArmor(params int[] exceptionUid)
	{
		_currentArmor = GetTotalArmor(exceptionUid);
		if (_playerController.network.isLocalPlayer)
		{
			UIGameManager.Instance?.UpdateArmorUI((int)_currentArmor);
		}
		_playerController.ForceUpdateStatsValueDebug();
	}

	private void UpdateUI(InventoryObject inventoryObject)
	{
		_playerController.inventoryManager.GetItemInventorySlotUI(inventoryObject.IdxInventory)?.SetActiveArmor(inventoryObject.Durability.ToString());
	}
}
