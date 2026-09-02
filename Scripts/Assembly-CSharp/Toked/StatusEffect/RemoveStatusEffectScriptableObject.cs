using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toked.StatusEffect;

[CreateAssetMenu(fileName = "RemoveStatusEffectScriptableObject", menuName = "WMO/ScriptableObjects/StatusEffect/RemoveStatusEffectScriptableObject", order = 0)]
public class RemoveStatusEffectScriptableObject : StatusEffectScriptableObject
{
	[SerializeField]
	private List<StatusEffectScriptableObject> _removeStatusEffectScriptableObject;

	public override void ApplyEffect(PlayerController playerController, StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
	}

	public override IEnumerator OnApplyEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		yield return null;
		if (!statusEffectController.PlayerController.network.isLocalPlayer)
		{
			yield break;
		}
		foreach (StatusEffectScriptableObject item in _removeStatusEffectScriptableObject)
		{
			statusEffectController.ClearStatus(item.StatusEffectData.Name);
		}
	}

	public override void RemoveEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
	}
}
