using System.Collections;
using UnityEngine;

namespace Toked.StatusEffect;

[CreateAssetMenu(fileName = "EnergyDrainItemStatusEffectScriptableObject", menuName = "WMO/ScriptableObjects/StatusEffect/EnergyDrainItemStatusEffectScriptableObject", order = 0)]
public class EnergyDrainItemStatusEffectScriptableObject : CurseItemStatusEffectScriptableObjectBase
{
	[SerializeField]
	private int _drainStamina = 1;

	[SerializeField]
	private PlayerStatsSO _staminaPlayerStatsSo;

	[SerializeField]
	private float _maxStaminaMultiplier = 0.5f;

	private PlayerController pc;

	public override void ApplyEffect(PlayerController playerController, StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		if (statusEffectController.gameObject.CompareTag(PlayerController.PLAYER_TAG))
		{
			pc = statusEffectController.PlayerController;
			if (pc.network.isLocalPlayer)
			{
				pc.AddStamina(-_drainStamina, recoveryStamina: false);
			}
			if (!pc.sweatVFX.enabled)
			{
				pc.sweatVFX.enabled = true;
			}
		}
	}

	public override IEnumerator OnApplyEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		yield return base.OnApplyEffect(statusEffectController, statusEffect);
		if (statusEffectController.gameObject.CompareTag(PlayerController.PLAYER_TAG))
		{
			pc = statusEffectController.PlayerController;
			pc.PlayerMultiplyStatsData.AddValue(_staminaPlayerStatsSo.name, 0f - _maxStaminaMultiplier);
			if (pc.network.isLocalPlayer)
			{
				pc.AddStamina(0, recoveryStamina: false);
			}
		}
	}

	public override void RemoveEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		base.RemoveEffect(statusEffectController, statusEffect);
		if (statusEffectController.gameObject.CompareTag(PlayerController.PLAYER_TAG))
		{
			pc = statusEffectController.PlayerController;
			pc.PlayerMultiplyStatsData.AddValue(_staminaPlayerStatsSo.name, _maxStaminaMultiplier);
			if (pc.network.isLocalPlayer)
			{
				pc.data.RecoverCurrentStamina();
			}
			else if (pc.sweatVFX.enabled)
			{
				pc.sweatVFX.enabled = false;
			}
		}
	}
}
