using System.Collections;
using UnityEngine;

namespace Toked.StatusEffect;

[CreateAssetMenu(fileName = "KevlarUtilityItemStatusEffectScriptableObject", menuName = "WMO/ScriptableObjects/StatusEffect/KevlarUtilityItemStatusEffectScriptableObject", order = 0)]
public class KevlarUtilityItemStatusEffectScriptableObject : UtilityItemStatusEffectScriptableObjectBase
{
	[SerializeField]
	private float _speedMultiplier = -0.2f;

	private PlayerController pc;

	public override void ApplyEffect(PlayerController playerController, StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
	}

	public override IEnumerator OnApplyEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		yield return base.OnApplyEffect(statusEffectController, statusEffect);
		if (statusEffectController.gameObject.CompareTag(PlayerController.PLAYER_TAG))
		{
			pc = statusEffectController.PlayerController;
			pc.timeline.clock.localTimeScale += _speedMultiplier;
			pc.ArmorManager.UpdateCurrentArmor();
		}
	}

	public override void RemoveEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		base.RemoveEffect(statusEffectController, statusEffect);
		if (statusEffectController.gameObject.CompareTag(PlayerController.PLAYER_TAG))
		{
			pc = statusEffectController.PlayerController;
			pc.timeline.clock.localTimeScale -= _speedMultiplier;
			pc.ArmorManager.UpdateCurrentArmor(base.UniqueItemId);
		}
	}
}
