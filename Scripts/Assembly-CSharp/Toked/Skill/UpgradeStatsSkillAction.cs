using UnityEngine;
using _Modules.Localization.Scripts;

namespace Toked.Skill;

[CreateAssetMenu(fileName = "UpgradeStatsSkillAction", menuName = "WMO/ScriptableObjects/Skill/Skill Action/Upgrade Stats Skill Action", order = 0)]
public class UpgradeStatsSkillAction : SkillEffectBaseAction, IStatsValueLocalization
{
	[SerializeField]
	private PlayerStatsSO _stats;

	[SerializeField]
	private float _addValue;

	public override void Apply(PlayerController playerController, SkillScriptableObject skillScriptableObject)
	{
		playerController.PlayerMultiplyStatsData.AddValue(_stats.name, _addValue);
	}

	public string GetStatsValueLocalization()
	{
		return $"{_addValue * 100f}%";
	}
}
