using System.Collections.Generic;
using Toked.Skill;
using UnityEngine;
using _Modules.Localization.Scripts;

namespace Toked.Crafting;

[CreateAssetMenu(fileName = "HipPouchUpgradeAction", menuName = "WMO/ScriptableObjects/Skill/Skill Action/Hip Pouch Upgrade Action", order = 0)]
public class HipPouchSkillAction : SkillEffectBaseAction, IStatsValueLocalization, ISkillEffectValues<InventorySlotEffectValue>
{
	[SerializeField]
	private int _addSlot = 2;

	[SerializeField]
	private bool _useSkillIdChecker;

	[SerializeField]
	private string _skillId;

	public override void Apply(PlayerController playerController, SkillScriptableObject skillScriptableObject)
	{
		if (!_useSkillIdChecker || !playerController.data.SkillData.CheckAdditionalPerkSkillLearn(_skillId))
		{
			playerController.data.AddSlotInventory(_addSlot);
			if (UIGameManager.Instance != null)
			{
				playerController.data.InitImageInventoryLocal();
			}
			if (_useSkillIdChecker && !string.IsNullOrEmpty(_skillId))
			{
				playerController.data.SkillData.AddAdditionalPerkSkill(_skillId);
			}
		}
	}

	public string GetStatsValueLocalization()
	{
		return _addSlot.ToString();
	}

	public List<InventorySlotEffectValue> GetValues()
	{
		return new List<InventorySlotEffectValue>
		{
			new InventorySlotEffectValue
			{
				Value = _addSlot
			}
		};
	}
}
