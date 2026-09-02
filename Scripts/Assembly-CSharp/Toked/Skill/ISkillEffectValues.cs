using System.Collections.Generic;

namespace Toked.Skill;

public interface ISkillEffectValues<T>
{
	List<T> GetValues();
}
