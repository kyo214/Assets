using System.Collections;
using UnityEngine;

namespace Toked.StatusEffect;

[CreateAssetMenu(fileName = "SanityStatusEffectScriptableObject", menuName = "WMO/ScriptableObjects/StatusEffect/SanityStatusEffect", order = 0)]
public class SanityStatusEffectScriptableObject : StatusEffectScriptableObject
{
	[SerializeField]
	private float _damage = 1f;

	[SerializeField]
	private float _stunTime = 0.3f;

	private int sanityLevel = 1;

	public override void ApplyEffect(PlayerController playerController, StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		if (statusEffectController.gameObject.CompareTag("Player"))
		{
			statusEffectController.PlayerController.data.DecreaseSanity(_damage);
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

	public override void AdditionalUpdateFunction(float elapsedTime)
	{
	}
}
