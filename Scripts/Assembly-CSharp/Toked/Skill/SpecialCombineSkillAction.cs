using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Toked.Skill;

[CreateAssetMenu(fileName = "UnlockSkillAction", menuName = "WMO/ScriptableObjects/Skill/Skill Action/Special Combine Action", order = 0)]
public class SpecialCombineSkillAction : SkillEffectBaseAction
{
	[SerializeField]
	private int _itemId1 = -1;

	[SerializeField]
	private int _itemId2 = -1;

	[SerializeField]
	private int _ResultItemID = -1;

	public override void Apply(PlayerController playerController, SkillScriptableObject skillScriptableObject)
	{
		playerController.data.ListSpecialCombine.Add(_itemId1 + "|" + _itemId2 + "|" + _ResultItemID);
	}

	private static IEnumerable GetItemId()
	{
		ValueDropdownList<int> result = new ValueDropdownList<int>();
		result.Add("None", -1);
		BGDatabase_HealingItem.ForEachEntity((BGDatabase_HealingItem data) =>
		{
			AddToList(data.Name ?? "", data.Keys);
		});
		return result;
		void AddToList(string inspectorName, int value)
		{
			result.Add(inspectorName, value);
		}
	}
}
