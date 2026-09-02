using System.Collections.Generic;
using Toked.StatusEffect;
using UnityEngine;

namespace _Modules.Item.Scripts;

[DisallowMultipleComponent]
[RequireComponent(typeof(ItemPickable))]
public class ItemIntractableStatusEffect : MonoBehaviour
{
	[SerializeField]
	private ItemPickable _itemPickable;

	[SerializeField]
	private List<StatusEffectScriptableObject> _statusEffectScriptableObjectList = new List<StatusEffectScriptableObject>();

	private List<GameObject> _particleEffectGameObjectList = new List<GameObject>();

	private bool _init;

	public ItemPickable ItemPickable
	{
		get
		{
			if (_itemPickable == null)
			{
				_itemPickable = GetComponent<ItemPickable>();
			}
			return _itemPickable;
		}
		set
		{
			_itemPickable = value;
		}
	}

	public List<StatusEffectScriptableObject> StatusEffectScriptableObjectList => _statusEffectScriptableObjectList;

	private void Start()
	{
		if (!_init)
		{
			ApplyItemEffect();
			ItemPickable.OnSetItemVisibility += OnSetItemVisibility;
			_init = true;
		}
	}

	private void OnDestroy()
	{
		if (_init)
		{
			ItemPickable.OnSetItemVisibility -= OnSetItemVisibility;
		}
	}

	public void Execute(PlayerController playerController, InventoryObject inventoryObject)
	{
		if (playerController == null || ItemPickable == null)
		{
			return;
		}
		foreach (StatusEffectScriptableObject statusEffectScriptableObject2 in _statusEffectScriptableObjectList)
		{
			if (!statusEffectScriptableObject2)
			{
				continue;
			}
			StatusEffectScriptableObject statusEffectScriptableObject = statusEffectScriptableObject2.CloneStatusEffectSO(destroyOnRemove: true);
			if (inventoryObject != null)
			{
				statusEffectScriptableObject.StatusEffectData.SetAdditionalName(ItemPickable.itemID.ToString(), inventoryObject.IdxInventory.ToString());
			}
			else
			{
				statusEffectScriptableObject.StatusEffectData.SetAdditionalName(ItemPickable.uniqueID.ToString(), ItemPickable.itemID.ToString());
			}
			if (statusEffectScriptableObject is IItemEffect itemEffect)
			{
				if (inventoryObject != null)
				{
					itemEffect.Init(ItemPickable.itemID, inventoryObject.IdxInventory);
				}
				else
				{
					itemEffect.Init(ItemPickable.itemID, ItemPickable.uniqueID);
				}
			}
			playerController.StatusEffectController.ApplyStatus(playerController, statusEffectScriptableObject);
			inventoryObject?.AddSetStatusEffect(statusEffectScriptableObject);
		}
	}

	public string GetEffectLocalization(string separator = " ", bool isNewLine = true, bool IsusingBrackets = false)
	{
		List<string> list = new List<string>();
		foreach (StatusEffectScriptableObject statusEffectScriptableObject in _statusEffectScriptableObjectList)
		{
			string statusEffectLocalizationName = statusEffectScriptableObject.GetStatusEffectLocalizationName(IsusingBrackets);
			list.Add(statusEffectLocalizationName);
		}
		string text = string.Join(separator, list);
		if (isNewLine)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				return "\n" + text;
			}
			return string.Empty;
		}
		string text2;
		if (!string.IsNullOrWhiteSpace(text))
		{
			text2 = text;
			if (text2 == null)
			{
				return "";
			}
		}
		else
		{
			text2 = string.Empty;
		}
		return text2;
	}

	public string GetGeneralEffectLocalization()
	{
		foreach (StatusEffectScriptableObject statusEffectScriptableObject in _statusEffectScriptableObjectList)
		{
			if (statusEffectScriptableObject is IItemEffect)
			{
				return IItemEffect.GetItemEffectLocalize();
			}
		}
		return string.Empty;
	}

	public void ApplyItemEffect()
	{
		if (_particleEffectGameObjectList.Count > 0)
		{
			return;
		}
		foreach (StatusEffectScriptableObject statusEffectScriptableObject in _statusEffectScriptableObjectList)
		{
			if (statusEffectScriptableObject is IItemEffect itemEffect)
			{
				GameObject itemEffectParticle = itemEffect.GetItemEffectParticle();
				if (itemEffectParticle != null)
				{
					GameObject item = Object.Instantiate(itemEffectParticle, ItemPickable.itemSprite.transform);
					_particleEffectGameObjectList.Add(item);
				}
			}
		}
	}

	private void OnSetItemVisibility(bool visible)
	{
		SetItemEffect(visible);
	}

	private void SetItemEffect(bool setActive)
	{
		foreach (GameObject particleEffectGameObject in _particleEffectGameObjectList)
		{
			particleEffectGameObject.SetActive(setActive);
		}
	}
}
