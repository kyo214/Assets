using System.Collections.Generic;
using Toked.Skill;
using UnityEngine;
using _Modules.Localization.Scripts;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "AddHealthSkillAction", menuName = "WMO/ScriptableObjects/Skill/Skill Action/Add Health Skill Action", order = 0)]
public class AddHealthSkillAction : SkillEffectBaseAction, IStatsValueLocalization, ISkillEffectValues<HealthEffectValue>
{
	[SerializeField]
	private int _addHealth = 20;

	[SerializeField]
	private bool _useSkillIdChecker;

	[SerializeField]
	private string _skillId;

	public override void Apply(PlayerController playerController, SkillScriptableObject skillScriptableObject)
	{
		if (!_useSkillIdChecker || !playerController.data.SkillData.CheckAdditionalPerkSkillLearn(_skillId))
		{
			playerController.data.AddMaxHealth(_addHealth, isIncreassedByPerks: true);
			if (_useSkillIdChecker && !string.IsNullOrEmpty(_skillId))
			{
				playerController.data.SkillData.AddAdditionalPerkSkill(_skillId);
			}
		}
	}

	public string GetStatsValueLocalization()
	{
		if (_addHealth <= 0)
		{
			return _addHealth.ToString();
		}
		return "+" + _addHealth;
	}

	public List<HealthEffectValue> GetValues()
	{
		return new List<HealthEffectValue>
		{
			new HealthEffectValue
			{
				Value = _addHealth
			}
		};
	}
}
