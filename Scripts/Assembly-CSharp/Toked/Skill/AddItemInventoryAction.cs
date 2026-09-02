using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Toked.Crafting;
using UnityEngine;

namespace Toked.Skill;

[CreateAssetMenu(fileName = "AddItemInventoryAction", menuName = "WMO/ScriptableObjects/Skill/Skill Action/Add Item Inventory Action", order = 0)]
public class AddItemInventoryAction : SkillEffectBaseAction, ISkillEffectValues<ItemInventoryEffectValue>
{
	[Serializable]
	private class ItemInventoryId
	{
		[SerializeField]
		public int itemInventoryId = -1;

		[SerializeField]
		public int amount = 1;

		[SerializeField]
		public bool useSkillIdChecker;

		[SerializeField]
		public string skillId;

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
			BGDatabase_HealingItem.ForEachEntity((BGDatabase_HealingItem data) =>
			{
				AddToList("HealingItem/" + data.Name, data.Keys);
			});
			return result;
			void AddToList(string inspectorName, int value)
			{
				result.Add(inspectorName, value);
			}
		}
	}

	[SerializeField]
	private List<ItemInventoryId> _itemInventoryId;

	public override void Apply(PlayerController playerController, SkillScriptableObject skillScriptableObject)
	{
		foreach (ItemInventoryId item in _itemInventoryId)
		{
			if (!item.useSkillIdChecker || !playerController.data.SkillData.CheckAdditionalPerkSkillLearn(item.skillId))
			{
				int idxInventory = playerController.data.AddInventory(item.itemInventoryId, isOnPick: false, item.amount);
				playerController.network.ExecAddInventory(item.itemInventoryId, idxInventory, item.amount);
				if (item.useSkillIdChecker && !string.IsNullOrEmpty(item.skillId))
				{
					playerController.data.SkillData.AddAdditionalPerkSkill(item.skillId);
				}
			}
		}
	}

	public List<ItemInventoryEffectValue> GetValues()
	{
		List<ItemInventoryEffectValue> list = new List<ItemInventoryEffectValue>();
		foreach (ItemInventoryId item in _itemInventoryId)
		{
			ItemScriptableObject itemData = DataManager.Instance.GetItemData(item.itemInventoryId.ToString());
			if ((bool)itemData)
			{
				list.Add(new ItemInventoryEffectValue
				{
					Value = itemData
				});
			}
		}
		return list;
	}
}
