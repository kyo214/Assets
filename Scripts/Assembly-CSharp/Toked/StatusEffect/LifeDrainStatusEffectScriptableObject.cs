using UnityEngine;

namespace Toked.StatusEffect;

[CreateAssetMenu(fileName = "LifeDrainStatusEffectScriptableObject", menuName = "WMO/ScriptableObjects/StatusEffect/LifeDrainStatusEffectScriptableObject", order = 0)]
public class LifeDrainStatusEffectScriptableObject : CurseItemStatusEffectScriptableObjectBase
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
				network.AddSubHealth(0f - _damage, trueDamage: true, cantDead: true);
			}
		}
		else if (statusEffectController.gameObject.CompareTag(EnemyController.EMEMY_TAG))
		{
			statusEffectController.EnemyController.Hurt(_damage, _stunTime, playerController.network.isLocalPlayer, playerController.network.GetIDX(), 3);
		}
	}
}
