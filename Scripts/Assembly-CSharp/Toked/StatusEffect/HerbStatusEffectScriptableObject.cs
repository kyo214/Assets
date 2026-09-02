using System.Collections;
using UnityEngine;

namespace Toked.StatusEffect;

[CreateAssetMenu(fileName = "HerbStatusEffectScriptableObject", menuName = "WMO/ScriptableObjects/StatusEffect/HerbStatusEffectScriptableObject", order = 0)]
public class HerbStatusEffectScriptableObject : StatusEffectScriptableObject
{
	[SerializeField]
	private StatusEffectScriptableObject _statusEffectScriptableObject;

	[SerializeField]
	private int _stackCounterActivation;

	public override void ApplyEffect(PlayerController playerController, StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		if (statusEffectController.PlayerController.network.isLocalPlayer && statusEffect.StackCounter >= _stackCounterActivation)
		{
			statusEffectController.ApplyStatus(playerController, _statusEffectScriptableObject);
			statusEffectController.ClearStatus(_statusEffectData.Name);
		}
	}

	public override IEnumerator OnApplyEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		yield return null;
		GameObject effectParticlePrefab = _statusEffectData.EffectParticlePrefab;
		if ((bool)effectParticlePrefab)
		{
			statusEffect.statusEffectGameObject = Object.Instantiate(effectParticlePrefab, statusEffectController.transform);
		}
	}

	public override void RemoveEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		if ((bool)statusEffect.statusEffectGameObject)
		{
			Object.Destroy(statusEffect.statusEffectGameObject);
		}
	}
}
