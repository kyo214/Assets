using UnityEngine;

namespace _Modules.GameSystem.BaseScripts;

public abstract class UnlockItemRequirementBaseSO : ScriptableObject
{
	public abstract bool CheckRequirement();
}
