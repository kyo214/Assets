using System.Collections.Generic;
using Toked.Skill;
using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "Recipes Data Checker Action", menuName = "WMO/ScriptableObjects/Crafting/Crafting Checker Action/Recipes Data Checker Action", order = 0)]
public class RecipesDataCheckerAction : CraftingRequirementCheckerAction
{
	[SerializeField]
	private List<SkillScriptableObject> _skillDataList = new List<SkillScriptableObject>();

	public override bool CheckRequirement(CraftRecipeScriptableObject craftRecipeScriptableObject, CraftingManager craftingManager)
	{
		for (int i = 0; i < _skillDataList.Count; i++)
		{
			if (!craftingManager.PlayerData.CheckSkillLearn(_skillDataList[i].ID))
			{
				return false;
			}
		}
		return true;
	}
}
