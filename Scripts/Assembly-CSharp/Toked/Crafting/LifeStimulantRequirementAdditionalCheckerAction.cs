using Toked.Skill;
using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "Life Stimulant Requirement Checker Action", menuName = "WMO/ScriptableObjects/Crafting/Crafting Checker Action/Life Stimulant Upgrade Additional Checker Action", order = 0)]
public class LifeStimulantRequirementAdditionalCheckerAction : CraftingRequirementCheckerAction
{
	[SerializeField]
	private SkillScriptableObject _perkSo;

	[SerializeField]
	private Vector2 _minMaxRequirementValue = new Vector2(0f, 120f);

	public override bool CheckRequirement(CraftRecipeScriptableObject craftRecipeScriptableObject, CraftingManager craftingManager)
	{
		if (_perkSo.ID == craftingManager.PlayerData.SkillData.PerkId)
		{
			float maxHealth = craftingManager.PlayerData.GetMaxHealth();
			if (maxHealth >= (float)(int)_minMaxRequirementValue.x)
			{
				return maxHealth <= (float)(int)_minMaxRequirementValue.y;
			}
			return false;
		}
		return false;
	}
}
