using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Toked;
using UnityEngine;

public class EventAnimationEnemy : MonoBehaviour
{
	[SerializeField]
	private EnemyController enemyController;

	[SerializeField]
	private GameObject objectActivate;

	[SerializeField]
	private Collider _localCollider;

	[SerializeField]
	private GameObject _effect;

	[SerializeField]
	private List<SpriteRenderer> _spriteEffect = new List<SpriteRenderer>();

	public void OnPlaySFX(string filename)
	{
		AudioManager.PlaySFXTransform(filename, base.transform, isLocalPlayerTrigger: false);
	}

	public void ShowCollider()
	{
		enemyController.attack.ShowMeleeCollider();
	}

	public void ShowColliderWithoutDisable()
	{
		enemyController.attack.ShowMeleeCollider(isNextFrameDisable: false);
	}

	public void DisableCollider()
	{
		enemyController.attack.meleeCollider.gameObject.SetActive(value: false);
	}

	public void Attack2()
	{
		enemyController.attack.EventBasicAttack2.Invoke();
	}

	public void SpecialEffect()
	{
		enemyController.attack.EventSpecialAttack1Effect.Invoke();
	}

	public void Stop()
	{
		if (!enemyController.isJumping)
		{
			enemyController.SetAISpeed(0f);
			enemyController.movement.isIdle = true;
		}
	}

	public void Move()
	{
		if (!enemyController.isJumping)
		{
			enemyController.SetAISpeed(enemyController.data.aggroSpeed);
			enemyController.movement.isIdle = false;
		}
	}

	public void Explode(string audioname)
	{
		Vector3 position = enemyController.object2D.position;
		PlayerController ownPlayer = NetworkGameManager.Instance.ownPlayer;
		Vector3 position2 = ownPlayer.transform.position;
		Vector3 normalized = (position2 - new Vector3(position.x, position2.y, position.z)).normalized;
		if (Vector3.Distance(new Vector3(position.x, position2.y, position.z), position2) < enemyController.attack.DistanceExplosion && !Physics.Raycast(new Vector3(position.x, position2.y, position.z), normalized, 3f, ownPlayer.weaponController.obstacleMask) && ownPlayer.network.GetHealth() > 0f)
		{
			ownPlayer.network.AddSubHealth((float)(-enemyController.attack.DamageExplosion) * ownPlayer.PlayerMultiplyStatsData.GetMultiplyDamageExplosion());
			ownPlayer.network.ExecHurtEffect(ownPlayer.network.GetIDX());
		}
		foreach (EnemyController item in GameManager.Instance.arrEnemyController)
		{
			if (!item.animatorState.HasParam("IsEscapeDanger") && item != enemyController && MathFunc.Distance(item.transform.position, position) < 3f && item.network.GetHealth() > 0f && !item.isDead && item.GetCurrentStateHash() != AnimatorHashManager.HoveringHash && item.enemyCollider.transform.localScale != Vector3.zero)
			{
				Vector3 vector = new Vector3(position.x, item.middlePos.position.y, position.z);
				Vector3 normalized2 = (item.middlePos.position - vector).normalized;
				float maxDistance = Vector3.Distance(vector, item.middlePos.position);
				if (!Physics.Raycast(vector, normalized2, maxDistance, ownPlayer.weaponController.obstacleMask))
				{
					item.Hurt((int)enemyController.attack.DamageExplosion, 0.2f, ownPlayer.network.isLocalPlayer, ownPlayer.network.GetIDX(), 2, isGrenade: true);
				}
			}
		}
		AudioManager.PlaySFXTransform(audioname, base.transform, isLocalPlayerTrigger: false);
		CameraGame.Instance.CameraShake(0.7f, 0.7f);
		objectActivate.transform.position = enemyController.object2D.position;
		objectActivate.SetActive(value: true);
	}

	public void ExplodeWithoutDeadAnimation(string audioname)
	{
		Explode(audioname);
		enemyController.Hurt(999f, 0.1f, execShakingCam: true, NetworkGameManager.Instance.ownPlayer.network.GetIDX(), 1, isGrenade: false, isHeadOff: false, isWithDeadAnimation: false, isActivateSpecialDead: false, isdamagingEnemy: false);
		enemyController.isDead = true;
		enemyController.Hide2DSprite();
		if (NetworkGameManager.Instance.isServer)
		{
			UniTaskUtil.DelayedCall(this, 2f, () =>
			{
				enemyController.Hide2DSprite();
				enemyController.network.AddSubHealth(-999f);
			}).Forget();
		}
	}

	public void BackToStateChasing()
	{
		if (enemyController.network.GetHealth() > 0f)
		{
			enemyController.SetState(EnemyState.Chasing);
			enemyController.isDown = false;
		}
	}

	public void StompEffect()
	{
		enemyController.SetAISpeed(0f);
		if (MathFunc.Distance(base.transform.position, NetworkGameManager.Instance.ownPlayer.transform.position) < 12f)
		{
			CameraGame.Instance.CameraShake(0.7f, 0.7f);
		}
		StartCoroutine(ShowLocalCollider());
		_effect.transform.parent = enemyController.transform.parent;
		_effect.transform.position = enemyController.transform.position;
		foreach (SpriteRenderer item in _spriteEffect)
		{
			Color color = item.color;
			color.a = 1f;
			item.color = color;
		}
		_spriteEffect[0]?.DOFade(0f, 2f).SetDelay(4f);
		_spriteEffect[1]?.DOFade(0f, 2f).SetDelay(2f);
		_effect.gameObject.SetActive(value: true);
		UniTaskUtil.DelayedCall(this, 6f, () =>
		{
			_effect.gameObject.SetActive(value: false);
		}).Forget();
	}

	public void DisableLocalCollider()
	{
		if (_localCollider != null)
		{
			_localCollider.gameObject.SetActive(value: false);
		}
	}

	private IEnumerator ShowLocalCollider()
	{
		if (_localCollider != null)
		{
			_localCollider.gameObject.SetActive(value: true);
			yield return new WaitForSeconds(0.1f);
			_localCollider.gameObject.SetActive(value: false);
		}
	}

	public void Jump()
	{
		if (NetworkGameManager.Instance.isServer && enemyController.network.GetHealth() > 0f)
		{
			if (enemyController.aiPath.reachedDestination || ((bool)enemyController.attack.targetChasing && MathFunc.Distance(enemyController.middlePos.position, enemyController.attack.targetChasing.position) < 2f))
			{
				enemyController.SetAISpeed(0f);
				return;
			}
			enemyController.attack.SetAttackTarget();
			enemyController.SetAISpeed(enemyController.data.aggroSpeed2 * 3f);
		}
	}

	public void SpawnEnemy(int type)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			EnemySpawner.Instance.SpawnEnemy(null, base.transform, type, isHorde: false, enemyController.middlePos.position);
		}
	}
}
