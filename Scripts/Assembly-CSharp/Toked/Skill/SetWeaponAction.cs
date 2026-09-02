using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Toked.Crafting;
using UnityEngine;

namespace Toked.Skill;

[CreateAssetMenu(fileName = "SetWeaponAction", menuName = "WMO/ScriptableObjects/Skill/Skill Action/Set Weapon Action", order = 0)]
public class SetWeaponAction : SkillEffectBaseAction, ISkillEffectValues<WeaponInventoryEffectValue>
{
	[SerializeField]
	private int _weaponId = -1;

	[SerializeField]
	private bool _useSkillIdChecker;

	[SerializeField]
	private string _skillId;

	public override void Apply(PlayerController playerController, SkillScriptableObject skillScriptableObject)
	{
		if (_useSkillIdChecker && playerController.data.SkillData.CheckAdditionalPerkSkillLearn(_skillId))
		{
			return;
		}
		InventoryObject inventoryObject = playerController.data.FindInventory(_weaponId);
		if (inventoryObject != null)
		{
			if (BGDatabase_Weapon.GetEntityByKeyid(_weaponId) != null && playerController.data.arrInventory[0].ID == -1 && BGDatabase_Weapon.GetEntityByKeyid(_weaponId).WeaponType == "Melee")
			{
				playerController.weaponController.EquipWeaponInventory(inventoryObject.IdxInventory);
			}
		}
		else
		{
			AddItemInventory(playerController, skillScriptableObject);
		}
		if (_useSkillIdChecker && !string.IsNullOrEmpty(_skillId))
		{
			playerController.data.SkillData.AddAdditionalPerkSkill(_skillId);
		}
	}

	public void AddItemInventory(PlayerController playerController, SkillScriptableObject skillScriptableObject)
	{
		playerController.weaponController.EquipWeaponInventory(playerController.data.AddInventory(_weaponId));
		if (BGDatabase_Weapon.GetEntityByKeyid(_weaponId) != null)
		{
			if (BGDatabase_Weapon.GetEntityByKeyid(_weaponId).WeaponType == "Melee")
			{
				playerController.network.ExecAddInventory(_weaponId, 0, 1);
			}
			else
			{
				playerController.network.ExecAddInventory(_weaponId, 1, BGDatabase_Weapon.GetEntityByKeyid(_weaponId).MagazineSize);
			}
		}
	}

	private static IEnumerable GetItemId()
	{
		ValueDropdownList<int> result = new ValueDropdownList<int>();
		result.Add("None", -1);
		BGDatabase_Weapon.ForEachEntity((BGDatabase_Weapon data) =>
		{
			if (data.WeaponType == "Melee" || data.WeaponType == "Range")
			{
				AddToList("Weapon" + data.WeaponType + "/" + data.Name, data.Keys);
			}
		});
		return result;
		void AddToList(string inspectorName, int value)
		{
			result.Add(inspectorName, value);
		}
	}

	public List<WeaponInventoryEffectValue> GetValues()
	{
		List<WeaponInventoryEffectValue> list = new List<WeaponInventoryEffectValue>();
		ItemScriptableObject itemData = DataManager.Instance.GetItemData(_weaponId.ToString());
		if ((bool)itemData && Enum.TryParse<WeaponInventoryEffectValue.WeaponType>(BGDatabase_Weapon.GetEntityByKeyid(_weaponId)?.WeaponType, out var result))
		{
			list.Add(new WeaponInventoryEffectValue
			{
				Value = itemData,
				Type = result
			});
		}
		return list;
	}
}
