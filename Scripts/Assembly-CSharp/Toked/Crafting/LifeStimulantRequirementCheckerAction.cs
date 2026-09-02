using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "Life Stimulant Requirement Checker Action", menuName = "WMO/ScriptableObjects/Crafting/Crafting Checker Action/Life Stimulant Upgrade Checker Action", order = 0)]
public class LifeStimulantRequirementCheckerAction : CraftingRequirementCheckerAction
{
	[SerializeField]
	private Vector2 _minMaxRequirementValue;

	public override bool CheckRequirement(CraftRecipeScriptableObject craftRecipeScriptableObject, CraftingManager craftingManager)
	{
		float maxHealth = craftingManager.PlayerData.GetMaxHealth();
		if (maxHealth >= (float)(int)_minMaxRequirementValue.x)
		{
			return maxHealth <= (float)(int)_minMaxRequirementValue.y;
		}
		return false;
	}
}
