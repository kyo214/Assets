using System.Collections;
using UnityEngine;

namespace Toked.StatusEffect;

[CreateAssetMenu(fileName = "TonicStatusEffectScriptableObject", menuName = "WMO/ScriptableObjects/StatusEffect/TonicStatusEffect", order = 0)]
public class TonicStatusEffectScriptableObject : StatusEffectScriptableObject
{
	public override void ApplyEffect(PlayerController playerController, StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
	}

	public override IEnumerator OnApplyEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		yield return null;
		PlayerNetwork network = statusEffectController.PlayerController.network;
		statusEffectController.PlayerController.DizzinessManager.ClearPoints();
		if (network.isLocalPlayer)
		{
			network.SetUnlimitedStamina(isUnlimitedStamina: true);
			UIGameManager.Instance.NoStaminaEffect.SetActive(value: true);
		}
	}

	public override void RemoveEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		PlayerNetwork network = statusEffectController.PlayerController.network;
		if (network.isLocalPlayer)
		{
			network.SetUnlimitedStamina(isUnlimitedStamina: false);
			UIGameManager.Instance.NoStaminaEffect.SetActive(value: false);
		}
	}
}
