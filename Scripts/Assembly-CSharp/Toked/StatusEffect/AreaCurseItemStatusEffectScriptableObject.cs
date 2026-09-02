using System.Collections;
using DG.Tweening;
using Toked.Weapon.Throwable;
using UnityEngine;

namespace Toked.StatusEffect;

[CreateAssetMenu(fileName = "AreaCurseItemStatusEffectScriptableObject", menuName = "WMO/ScriptableObjects/StatusEffect/AreaCurseItemStatusEffectScriptableObject", order = 0)]
public class AreaCurseItemStatusEffectScriptableObject : CurseItemStatusEffectScriptableObjectBase
{
	[SerializeField]
	private float _damage = 6f;

	[SerializeField]
	private float _stunTime = 0.3f;

	[SerializeField]
	private bool _isUsingGreenScreenEffect;

	public override void ApplyEffect(PlayerController playerController, StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		if (statusEffect.HasAntiStatusEffect)
		{
			return;
		}
		if (statusEffectController.gameObject.CompareTag(PlayerController.PLAYER_TAG))
		{
			PlayerNetwork network = statusEffectController.PlayerController.network;
			if (network.isLocalPlayer)
			{
				network.ExecHurtEffect(network.GetIDX(), isCloseInventory: false, _isUsingGreenScreenEffect);
				network.AddSubHealth(0f - _damage, trueDamage: true, cantDead: true);
			}
		}
		else if (statusEffectController.gameObject.CompareTag(EnemyController.EMEMY_TAG))
		{
			statusEffectController.EnemyController.Hurt(_damage, _stunTime, playerController.network.isLocalPlayer, playerController.network.GetIDX(), 3);
		}
	}

	public override IEnumerator OnApplyEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		yield return null;
		CheckHaveAntiStatusEffect(statusEffect);
		GameObject effectParticlePrefab = _statusEffectData.EffectParticlePrefab;
		if (!effectParticlePrefab)
		{
			yield break;
		}
		statusEffect.statusEffectGameObject = Object.Instantiate(effectParticlePrefab, statusEffectController.transform);
		if (statusEffect.statusEffectGameObject.TryGetComponent<AreaImpactItemBase>(out var component))
		{
			component.Init(statusEffect.playerController, -1f, _damage);
			if (statusEffectController.gameObject.CompareTag(PlayerController.PLAYER_TAG) && statusEffectController.PlayerController.network.isLocalPlayer && _isUsingGreenScreenEffect)
			{
				UIGameManager.Instance.flashGreen2.enabled = true;
				UIGameManager.Instance.flashGreen2.color = new Color(1f, 1f, 1f, 0.65f);
				UIGameManager.Instance.flashGreen2.DOFade(0.1f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetDelay(0.2f);
			}
		}
	}

	public override void RemoveEffect(StatusEffectController statusEffectController, StatusEffectController.StatusEffect statusEffect)
	{
		if (statusEffectController.gameObject.CompareTag(PlayerController.PLAYER_TAG) && statusEffectController.PlayerController.network.isLocalPlayer && _isUsingGreenScreenEffect)
		{
			UIGameManager.Instance.flashGreen2.enabled = false;
			UIGameManager.Instance.flashGreen2.DOKill();
		}
		base.RemoveEffect(statusEffectController, statusEffect);
	}
}
