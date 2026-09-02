using UnityEngine;

namespace Toked.Skill;

[CreateAssetMenu(fileName = "UnlockPerkAction", menuName = "WMO/ScriptableObjects/Skill/Skill Action/Unlock Perk Action", order = 0)]
public class UnlockPerkAction : SkillEffectBaseAction
{
	public override void Apply(PlayerController playerController, SkillScriptableObject skillScriptableObject)
	{
		playerController.data.SkillData.SetPerk(skillScriptableObject.ID);
	}
}
