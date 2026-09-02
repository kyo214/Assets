using Toked.Item;
using Toked.StatusEffect;
using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "WMO/ScriptableObjects/Item/ItemScriptableObject", order = 0)]
public class ItemScriptableObject : ScriptableObject
{
	[SerializeField]
	private string _id;

	[SerializeField]
	private Sprite _itemSprite;

	[SerializeField]
	private GameObject _itemPrefab;

	[SerializeField]
	private ItemPickable _itemPickable;

	[SerializeField]
	private StatusEffectScriptableObject _additionalStatusEffectSO;

	[SerializeField]
	private bool _cantStackingStatusEffect;

	[SerializeField]
	private bool _useCustomItemEffect;

	[SerializeField]
	private StatusEffectScriptableObject _customItemEffectSO;

	[SerializeField]
	private bool _useCustomEquipInventoryEffect;

	[SerializeField]
	private CustomItemActionBase _customEquipInventoryEffectSO;

	[SerializeField]
	private bool _manualApplyEffectItemInventory;

	[SerializeField]
	private bool _defaultEquipValue = true;

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

	public Sprite ItemSprite
	{
		get
		{
			return _itemSprite;
		}
		set
		{
			_itemSprite = value;
		}
	}

	public GameObject ItemPrefab
	{
		get
		{
			return _itemPrefab;
		}
		set
		{
			_itemPrefab = value;
		}
	}

	public bool CantStackingStatusEffect => _cantStackingStatusEffect;

	public bool UseCustomItemEffect => _useCustomItemEffect;

	public bool UseCustomEquipInventoryEffect => _useCustomEquipInventoryEffect;

	public StatusEffectScriptableObject CustomItemEffectSO
	{
		get
		{
			return _customItemEffectSO;
		}
		set
		{
			_customItemEffectSO = value;
		}
	}

	public CustomItemActionBase CustomEquipInventoryEffectSO
	{
		get
		{
			return _customEquipInventoryEffectSO;
		}
		set
		{
			_customEquipInventoryEffectSO = value;
		}
	}

	public StatusEffectScriptableObject AdditionalStatusEffectSO
	{
		get
		{
			return _additionalStatusEffectSO;
		}
		set
		{
			_additionalStatusEffectSO = value;
		}
	}

	public bool ManualApplyEffectItemInventory
	{
		get
		{
			return _manualApplyEffectItemInventory;
		}
		set
		{
			_manualApplyEffectItemInventory = value;
		}
	}

	public bool DefaultEquipValue
	{
		get
		{
			return _defaultEquipValue;
		}
		set
		{
			_defaultEquipValue = value;
		}
	}

	public ItemPickable ItemPickable => _itemPickable;

	public void SetItemPickable(GameObject itemPrefab)
	{
		if (!(itemPrefab == null))
		{
			_itemPrefab = itemPrefab;
			_itemPickable = _itemPrefab.GetComponentInChildren<ItemPickable>();
		}
	}

	public bool CheckInventoryItemEquip()
	{
		if (_useCustomEquipInventoryEffect)
		{
			return _manualApplyEffectItemInventory;
		}
		return false;
	}

	private void SetItemPickable()
	{
		if ((bool)_itemPrefab)
		{
			_itemPickable = _itemPrefab.GetComponentInChildren<ItemPickable>();
		}
	}
}
