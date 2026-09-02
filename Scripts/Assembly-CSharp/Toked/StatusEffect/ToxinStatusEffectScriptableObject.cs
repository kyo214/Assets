using System.Collections;
using UnityEngine;

namespace Toked.StatusEffect;

[CreateAssetMenu(fileName = "ToxinStatusEffectScriptableObject", menuName = "WMO/ScriptableObjects/StatusEffect/ToxinStatusEffect", order = 0)]
public class ToxinStatusEffectScriptableObject : StatusEffectScriptableObject
{
	[SerializeField]
	private int _dizzinessPoint = 3;

	public float _damage = 3f;

	public float _stunTime = 0.3f;

	public override void ApplyEffect(PlayerController playerController, StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		if (statusEffectController.gameObject.CompareTag(PlayerController.PLAYER_TAG) && !statusEffect.HasAntiStatusEffect)
		{
			PlayerNetwork network = statusEffectController.PlayerController.network;
			if (network.isLocalPlayer)
			{
				network.playerController.DizzinessManager.AddPoints(_dizzinessPoint);
				network.ExecHurtEffect(network.GetIDX(), isCloseInventory: false);
				network.AddSubHealth(0f - _damage, trueDamage: true);
			}
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
		CheckHaveAntiStatusEffect(statusEffect);
	}

	public override void RemoveEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		if ((bool)statusEffect.statusEffectGameObject)
		{
			Object.Destroy(statusEffect.statusEffectGameObject);
		}
	}
}
