using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Toked.Skill;

[CreateAssetMenu(fileName = "RemoveItemAction", menuName = "WMO/ScriptableObjects/Skill/Skill Action/RemoveItem", order = 0)]
public class RemoveItemAction : SkillEffectBaseAction
{
	[SerializeField]
	private int _itemId = -1;

	[SerializeField]
	private bool _useSkillIdChecker;

	[SerializeField]
	private string _skillId;

	public override void Apply(PlayerController playerController, SkillScriptableObject skillScriptableObject)
	{
		if (_useSkillIdChecker)
		{
			if (!playerController.data.SkillData.CheckAdditionalPerkSkillLearn(_skillId))
			{
				RemoveItem();
			}
		}
		else
		{
			RemoveItem();
		}
		void RemoveItem()
		{
			InventoryObject inventoryObject = playerController.data.FindInventory(_itemId);
			if (inventoryObject != null)
			{
				playerController.data.RemoveInventoryData(inventoryObject.IdxInventory);
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
}
