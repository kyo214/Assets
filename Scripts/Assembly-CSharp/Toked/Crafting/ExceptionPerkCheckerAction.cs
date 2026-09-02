using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "ExceptionPerkCheckerAction", menuName = "WMO/ScriptableObjects/Crafting/Crafting Checker Action/Exception Perk Checker Action", order = 0)]
public class ExceptionPerkCheckerAction : CraftingRequirementCheckerAction
{
	[SerializeField]
	private List<string> _exceptionPerksList = new List<string>();

	public override bool CheckRequirement(CraftRecipeScriptableObject craftRecipeScriptableObject, CraftingManager craftingManager)
	{
		return !_exceptionPerksList.Contains(craftingManager.PlayerData.SkillData.PerkId);
	}

	private static IEnumerable GetPerkId()
	{
		ValueDropdownList<string> result = new ValueDropdownList<string>();
		result.Add("None", "");
		BGDatabase_Perks.ForEachEntity((BGDatabase_Perks data) =>
		{
			AddToList(data.Name, data.Id);
		});
		return result;
		void AddToList(string inspectorName, string value)
		{
			result.Add(inspectorName, value);
		}
	}
}
