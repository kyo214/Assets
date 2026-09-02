using UnityEngine;

namespace Toked.Skill;

public abstract class SkillEffectBaseAction : ScriptableObject
{
	public abstract void Apply(PlayerController playerController, SkillScriptableObject skillScriptableObject);
}
