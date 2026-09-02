using System.Collections;
using UnityEngine;

namespace Toked.StatusEffect;

[CreateAssetMenu(fileName = "BurnStatusEffectScriptableObject", menuName = "WMO/ScriptableObjects/StatusEffect/BurnStatusEffect", order = 0)]
public class BurnStatusEffectScriptableObject : StatusEffectScriptableObject
{
	[SerializeField]
	private float _damage = 6f;

	[SerializeField]
	private float _stunTime = 0.3f;

	public override void ApplyEffect(PlayerController playerController, StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		if (statusEffectController.gameObject.CompareTag(PlayerController.PLAYER_TAG))
		{
			PlayerNetwork network = statusEffectController.PlayerController.network;
			if (network.isLocalPlayer)
			{
				network.ExecHurtEffect(network.GetIDX(), isCloseInventory: false);
				if (GameModes.Instance.isGrenadeFriendlyFire)
				{
					network.AddSubHealth(0f - _damage / 2f, trueDamage: true);
				}
				else
				{
					network.AddSubHealth(0f - _damage / 3f, trueDamage: true);
				}
			}
		}
		else if (statusEffectController.gameObject.CompareTag(EnemyController.EMEMY_TAG) && statusEffectController.EnemyController.enemyCollider.transform.localScale != Vector3.zero)
		{
			statusEffectController.EnemyController.Hurt(_damage, _stunTime, playerController.network.isLocalPlayer, playerController.network.GetIDX(), 3);
		}
		else if (statusEffectController.gameObject.CompareTag(ObjectCollisionBullet.DESTRUCTABLE_OBJECT_TAG))
		{
			if (statusEffectController.ObjectCollisionController.destructObject != null)
			{
				statusEffectController.ObjectCollisionController.destructObject.RPCHitObject(playerController.network.GetIDX(), (int)_damage / 2);
			}
			else
			{
				statusEffectController.ObjectCollisionController.HitDestructibleObject(_damage / 2f);
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
	}

	public override void RemoveEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		if ((bool)statusEffect.statusEffectGameObject)
		{
			Object.Destroy(statusEffect.statusEffectGameObject);
		}
	}

	public override float GetTotalEffectDuration(StatusEffectController statusEffectController)
	{
		if ((bool)statusEffectController && statusEffectController.gameObject.CompareTag(PlayerController.PLAYER_TAG))
		{
			return base.Duration * statusEffectController.PlayerController.PlayerMultiplyStatsData.GetBurnDuration();
		}
		return base.Duration;
	}
}
