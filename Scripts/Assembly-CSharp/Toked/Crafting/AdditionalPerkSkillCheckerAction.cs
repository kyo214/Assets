using System.Collections.Generic;
using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "Additional Perk Skill Checker Action", menuName = "WMO/ScriptableObjects/Crafting/Crafting Checker Action/Additional Perk Skill Checker Action", order = 0)]
public class AdditionalPerkSkillCheckerAction : CraftingRequirementCheckerAction
{
	[SerializeField]
	private List<string> _additionalPerkSkill = new List<string>();

	public override bool CheckRequirement(CraftRecipeScriptableObject craftRecipeScriptableObject, CraftingManager craftingManager)
	{
		for (int i = 0; i < _additionalPerkSkill.Count; i++)
		{
			if (!craftingManager.PlayerData.SkillData.CheckAdditionalPerkSkillLearn(_additionalPerkSkill[i]))
			{
				return false;
			}
		}
		return true;
	}
}
