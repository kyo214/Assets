using UnityEngine;

namespace Toked.Skill;

[CreateAssetMenu(fileName = "UnlockSkillAction", menuName = "WMO/ScriptableObjects/Skill/Skill Action/Unlock Skill Action", order = 0)]
public class UnlockSkillAction : SkillEffectBaseAction
{
	public override void Apply(PlayerController playerController, SkillScriptableObject skillScriptableObject)
	{
		playerController.data.AddSkillLearn(skillScriptableObject.ID);
	}
}
