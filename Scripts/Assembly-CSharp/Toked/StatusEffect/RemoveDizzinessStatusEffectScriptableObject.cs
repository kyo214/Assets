using System.Collections;
using UnityEngine;

namespace Toked.StatusEffect;

[CreateAssetMenu(fileName = "RemoveDizzinessStatusEffectScriptableObject", menuName = "WMO/ScriptableObjects/StatusEffect/RemoveDizzinessStatusEffectScriptableObject", order = 0)]
public class RemoveDizzinessStatusEffectScriptableObject : StatusEffectScriptableObject
{
	public override void ApplyEffect(PlayerController playerController, StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
	}

	public override IEnumerator OnApplyEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		yield return null;
		statusEffectController.PlayerController?.DizzinessManager?.ClearPoints();
	}

	public override void RemoveEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
	}
}
