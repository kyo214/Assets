using UnityEngine;
using _Modules.Localization.Scripts;

namespace Toked.Skill;

[CreateAssetMenu(fileName = "UpgradeBonusLootAction", menuName = "WMO/ScriptableObjects/Skill/Skill Action/Upgrade Bonus Loot Action", order = 0)]
public class UpgradeBonusLootAction : SkillEffectBaseAction, IStatsValueLocalization
{
	[SerializeField]
	private float _bonusLootMaterial = 0.1f;

	public override void Apply(PlayerController playerController, SkillScriptableObject skillScriptableObject)
	{
		playerController.data.MaterialInventoryManager.SetBonusLootMaterial(_bonusLootMaterial);
	}

	public string GetStatsValueLocalization()
	{
		return $"{_bonusLootMaterial * 100f}%";
	}
}
