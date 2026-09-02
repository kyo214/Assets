using UnityEngine;

namespace Toked.Skill;

[CreateAssetMenu(fileName = "UnlockCustomIdSkillAction", menuName = "WMO/ScriptableObjects/Skill/Skill Action/Unlock Custom Id Skill Action", order = 0)]
public class UnlockCustomAdditionalSkillAction : SkillEffectBaseAction
{
	[SerializeField]
	private string _skillId;

	public override void Apply(PlayerController playerController, SkillScriptableObject skillScriptableObject)
	{
		playerController.data.SkillData.AddAdditionalPerkSkill(_skillId);
	}
}
