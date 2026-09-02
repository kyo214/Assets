using UnityEngine;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "Energy Stimulant Requirement Checker Action", menuName = "WMO/ScriptableObjects/Crafting/Crafting Checker Action/Energy Stimulant Upgrade Checker Action", order = 0)]
public class EnergyStimulantRequirementCheckerAction : CraftingRequirementCheckerAction
{
	[SerializeField]
	private Vector2 _minMaxRequirementValue;

	public override bool CheckRequirement(CraftRecipeScriptableObject craftRecipeScriptableObject, CraftingManager craftingManager)
	{
		float maxStamina = craftingManager.PlayerData.GetMaxStamina();
		if (maxStamina >= (float)(int)_minMaxRequirementValue.x)
		{
			return maxStamina <= (float)(int)_minMaxRequirementValue.y;
		}
		return false;
	}
}
