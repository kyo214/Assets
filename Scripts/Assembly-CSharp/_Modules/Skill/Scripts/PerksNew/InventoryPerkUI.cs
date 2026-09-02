using System.Collections.Generic;
using Toked.Crafting;
using Toked.Skill;
using UnityEngine;

namespace _Modules.Skill.Scripts.PerksNew;

public class InventoryPerkUI : MonoBehaviour
{
	[SerializeField]
	private ItemSlotInventoryPerkUI _meleeWeaponItemSlotInventoryPerkUI;

	[SerializeField]
	private ItemSlotInventoryPerkUI _rangeWeaponItemSlotInventoryPerkUI;

	[SerializeField]
	private List<ItemSlotInventoryPerkUI> _itemSlotsInventoryPerkUiList;

	[SerializeField]
	private ItemScriptableObject _defaultMeleeItem;

	[SerializeField]
	private List<ItemScriptableObject> _defaultItem;

	public void Init(SkillScriptableObject perkSo)
	{
		SetInventorySlotUI(perkSo);
		SetInventoryItemUI(perkSo);
	}

	public void InitSlot()
	{
		NetworkGameManager instance = NetworkGameManager.Instance;
		int num = (((object)instance != null && instance.mode == NetworkGameManager.MultiplayerMode.Solo) ? 6 : 4);
		for (int num2 = _itemSlotsInventoryPerkUiList.Count - 1; num2 >= 0; num2--)
		{
			_itemSlotsInventoryPerkUiList[num2].gameObject.SetActive(num2 < num);
		}
	}

	private void SetInventorySlotUI(SkillScriptableObject perkSo)
	{
		int num = ((NetworkGameManager.Instance.mode == NetworkGameManager.MultiplayerMode.Solo) ? 6 : 4);
		int num2 = InventorySlotEffectValue.CalculateTotalValue(perkSo.GetEffectValues<InventorySlotEffectValue>());
		num = Mathf.Clamp(num + num2, 0, _itemSlotsInventoryPerkUiList.Count);
		for (int i = 0; i < num; i++)
		{
			_itemSlotsInventoryPerkUiList[i].gameObject.SetActive(value: true);
		}
		for (int num3 = _itemSlotsInventoryPerkUiList.Count - 1; num3 >= num; num3--)
		{
			_itemSlotsInventoryPerkUiList[num3].gameObject.SetActive(value: false);
		}
	}

	private void SetInventoryItemUI(SkillScriptableObject perkSo)
	{
		_rangeWeaponItemSlotInventoryPerkUI.Reset();
		for (int i = 0; i < _itemSlotsInventoryPerkUiList.Count; i++)
		{
			_itemSlotsInventoryPerkUiList[i].Reset();
		}
		int num = 0;
		for (int j = 0; j < _defaultItem.Count; j++)
		{
			ItemScriptableObject itemScriptableObject = _defaultItem[j];
			_itemSlotsInventoryPerkUiList[num++].Init(itemScriptableObject);
		}
		_meleeWeaponItemSlotInventoryPerkUI.Init(_defaultMeleeItem);
		foreach (WeaponInventoryEffectValue effectValue in perkSo.GetEffectValues<WeaponInventoryEffectValue>())
		{
			switch (effectValue.Type)
			{
			case WeaponInventoryEffectValue.WeaponType.Melee:
				_meleeWeaponItemSlotInventoryPerkUI.Init(effectValue.Value);
				break;
			case WeaponInventoryEffectValue.WeaponType.Range:
				_rangeWeaponItemSlotInventoryPerkUI.Init(effectValue.Value);
				break;
			}
		}
		List<ItemInventoryEffectValue> effectValues = perkSo.GetEffectValues<ItemInventoryEffectValue>();
		for (int k = 0; k < effectValues.Count; k++)
		{
			if (num >= _itemSlotsInventoryPerkUiList.Count)
			{
				break;
			}
			_itemSlotsInventoryPerkUiList[num++].Init(effectValues[k].Value);
		}
	}
}
