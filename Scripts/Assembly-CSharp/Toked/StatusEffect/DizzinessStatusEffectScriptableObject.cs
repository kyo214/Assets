using System.Collections;
using UnityEngine;

namespace Toked.StatusEffect;

[CreateAssetMenu(fileName = "DizzinessStatusEffectScriptableObject", menuName = "WMO/ScriptableObjects/StatusEffect/DizzinessStatusEffectScriptableObject", order = 0)]
public class DizzinessStatusEffectScriptableObject : StatusEffectScriptableObject
{
	[SerializeField]
	private int _dizzinessPoints = 25;

	public override void ApplyEffect(PlayerController playerController, StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
	}

	public override IEnumerator OnApplyEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		yield return null;
		GameObject effectParticlePrefab = _statusEffectData.EffectParticlePrefab;
		if ((bool)effectParticlePrefab)
		{
			statusEffect.statusEffectGameObject = Object.Instantiate(effectParticlePrefab, statusEffectController.transform);
		}
		AddDizzinessPoint(statusEffectController, statusEffect);
	}

	public override void RemoveEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		if ((bool)statusEffect.statusEffectGameObject)
		{
			Object.Destroy(statusEffect.statusEffectGameObject);
		}
	}

	private void AddDizzinessPoint(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		if (!statusEffect.HasAntiStatusEffect && statusEffectController.gameObject.CompareTag(PlayerController.PLAYER_TAG) && statusEffect.playerController.network.isLocalPlayer)
		{
			statusEffectController.PlayerController?.DizzinessManager?.AddPoints(_dizzinessPoints);
		}
	}
}
