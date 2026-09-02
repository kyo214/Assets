using Toked.Skill;
using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "SkillLearn RequirementChecker Action", menuName = "WMO/ScriptableObjects/Crafting/Crafting Checker Action/SkillLearn RequirementChecker Action", order = 0)]
public class SkillLearnRequirementCheckerAction : CraftingRequirementCheckerAction
{
	[SerializeField]
	private Vector2 _minMaxSkillLearn;

	public override bool CheckRequirement(CraftRecipeScriptableObject craftRecipeScriptableObject, CraftingManager craftingManager)
	{
		if (craftRecipeScriptableObject is CraftSkillRecipeScriptableObject craftSkillRecipeScriptableObject)
		{
			int num = 0;
			PlayerData playerData = craftingManager.PlayerData;
			foreach (SkillScriptableObject skillLearnSO in craftSkillRecipeScriptableObject.SkillLearnSOList)
			{
				if (playerData.CheckSkillLearn(skillLearnSO.ID))
				{
					num++;
					if ((float)num >= _minMaxSkillLearn.y)
					{
						return false;
					}
				}
			}
			return (float)num >= _minMaxSkillLearn.x;
		}
		return true;
	}
}
