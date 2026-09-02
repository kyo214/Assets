using UnityEngine;
using _Modules.Localization.Scripts;

namespace Toked.Skill;

[CreateAssetMenu(fileName = "UpgradeDiscountCraftAction", menuName = "WMO/ScriptableObjects/Skill/Skill Action/Upgrade Discount Craft Action", order = 0)]
public class UpgradeDiscountCraftAction : SkillEffectBaseAction, IStatsValueLocalization
{
	[SerializeField]
	private float _discountCraft = 0.2f;

	public override void Apply(PlayerController playerController, SkillScriptableObject skillScriptableObject)
	{
		playerController.data.MaterialInventoryManager.SetDiscountCraft(_discountCraft);
	}

	public string GetStatsValueLocalization()
	{
		return $"{_discountCraft * 100f}%";
	}
}
