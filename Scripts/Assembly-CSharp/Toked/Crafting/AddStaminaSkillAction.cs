using System.Collections.Generic;
using Toked.Skill;
using UnityEngine;
using _Modules.Localization.Scripts;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "AddStaminaSkillAction", menuName = "WMO/ScriptableObjects/Skill/Skill Action/Add Stamina Skill Action", order = 0)]
public class AddStaminaSkillAction : SkillEffectBaseAction, IStatsValueLocalization, ISkillEffectValues<StaminaEffectValue>
{
	[SerializeField]
	private int _addStamina = 20;

	[SerializeField]
	private bool _useSkillIdChecker;

	[SerializeField]
	private string _skillId;

	public override void Apply(PlayerController playerController, SkillScriptableObject skillScriptableObject)
	{
		if (!_useSkillIdChecker || !playerController.data.SkillData.CheckAdditionalPerkSkillLearn(_skillId))
		{
			playerController.data.AddMaxStamina(_addStamina, isIncreasedByPerks: true);
			if (_useSkillIdChecker && !string.IsNullOrEmpty(_skillId))
			{
				playerController.data.SkillData.AddAdditionalPerkSkill(_skillId);
			}
		}
	}

	public string GetStatsValueLocalization()
	{
		if (_addStamina <= 0)
		{
			return _addStamina.ToString();
		}
		return "+" + _addStamina;
	}

	public List<StaminaEffectValue> GetValues()
	{
		return new List<StaminaEffectValue>
		{
			new StaminaEffectValue
			{
				Value = _addStamina
			}
		};
	}
}
