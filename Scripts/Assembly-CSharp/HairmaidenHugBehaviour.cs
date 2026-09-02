using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class HairmaidenHugBehaviour : MonoBehaviour
{
	private EnemyController enemyController;

	private EnemyAttack enemyAttack;

	private float bodyColliderRadius;

	private void Start()
	{
		enemyController = base.transform.parent.GetComponent<EnemyController>();
		enemyAttack = base.transform.parent.GetComponent<EnemyAttack>();
		enemyController.attack.targetPlayer = null;
		enemyAttack.special1Type = "Entangle";
		enemyController.myrigidbody.isKinematic = false;
		if (!NetworkGameManager.Instance.isServer)
		{
			enemyController.stoperCollider.gameObject.SetActive(value: false);
		}
		bodyColliderRadius = enemyController.bodyCollider.radius;
	}

	public void Attack()
	{
		if (enemyController.network.GetHealth() > 0f)
		{
			enemyController.network.SetDoSpesialAttack(value: true);
			enemyController.isAttacking = true;
			if (enemyAttack.targetChasing != null)
			{
				enemyController.movement.angleAnim = enemyAttack.AngleEnemy((enemyAttack.targetChasing.transform.position - base.transform.position).normalized, enemyController.movement.angleAnim);
				enemyController.network.SetAngleDirection(enemyAttack.AngleEnemy((enemyAttack.targetChasing.transform.position - base.transform.position).normalized, enemyController.movement.angleAnim));
			}
			enemyController.network.SetAnimation("StartAggro" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			enemyController.SetAISpeed(0f);
			UniTaskUtil.DelayedCall(this, 0.9f, () =>
			{
				Attacking();
			}).Forget();
		}
	}

	private void Attacking()
	{
		if (!(enemyController.network.GetHealth() > 0f))
		{
			return;
		}
		enemyController.SetAISpeed(8f);
		enemyAttack.SetAttackTarget();
		if ((bool)enemyAttack.targetChasing)
		{
			enemyController.movement.angleAnim = enemyAttack.AngleEnemy((enemyAttack.targetChasing.transform.position - base.transform.position).normalized, enemyController.movement.angleAnim);
		}
		enemyController.network.SetAnimation("MoveAggro" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
		UniTaskUtil.DelayedCall(this, 0.3f, () =>
		{
			if (NetworkGameManager.Instance.isServer && enemyController.network.GetHealth() > 0f)
			{
				enemyAttack.SetAttackTarget(isSetPrevChasing: false);
				if (enemyAttack.targetChasing != null)
				{
					enemyController.movement.angleAnim = enemyAttack.AngleEnemy((enemyAttack.targetChasing.transform.position - base.transform.position).normalized, enemyController.movement.angleAnim);
				}
				enemyController.network.SetAnimation("Special1-" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
				enemyController.SetAISpeed(12f);
			}
		}).Forget();
	}

	public void Entangle()
	{
		PlayerController targetPlayer = enemyAttack.targetPlayer;
		if (!NetworkGameManager.Instance.isServer || !(enemyController.network.GetHealth() > 0f))
		{
			return;
		}
		DOTween.To(() => enemyController.bodyCollider.radius, (float x) =>
		{
			enemyController.bodyCollider.radius = x;
		}, 1f, 1f).OnComplete(() =>
		{
			enemyController.bodyCollider.radius = bodyColliderRadius;
			enemyController.myrigidbody.isKinematic = true;
		});
		targetPlayer.network.ExecEnTangled(enemyController.network.GetIDX(), enemyController.movement.angleAnim);
		enemyController.SetAISpeed(0f);
		enemyController.network.SetAnimation("Special2-" + enemyController.movement.angleAnim);
		UniTaskUtil.DelayedCall(this, 2f, () =>
		{
			Bite();
		}).Forget();
		UniTaskUtil.DelayedCall(this, 1f, () =>
		{
			if (NetworkGameManager.Instance.isServer && enemyController.network.GetHealth() > 0f)
			{
				enemyController.network.SetAnimation("StartAggro" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
		}).Forget();
		enemyController.attack.meleeCollider.gameObject.SetActive(value: false);
		enemyAttack.timerDelayAggro2.StartDuration(Random.Range(5f, 7f));
	}

	private void Bite()
	{
		if (enemyController.network.GetHealth() > 0f)
		{
			Invoke("BiteDamageToPlayer", 0.5f);
			enemyController.network.SetAnimation("bite" + enemyController.movement.angleAnim);
		}
	}

	private void BiteDamageToPlayer()
	{
		if (enemyController.network.GetHealth() > 0f && enemyController.attack.targetPlayer != null && enemyController.attack.targetPlayer.isEntangled && enemyController.attack.targetPlayer.network.GetHealth() > 0f)
		{
			Invoke("BiteDamageToPlayer", 0.5f);
			enemyController.attack.targetPlayer.network.ExecHurtEffect(enemyController.attack.targetPlayer.network.GetIDX());
			enemyController.attack.targetPlayer.network.AddSubHealth((0f - enemyController.data.damage) / 3f);
			CameraGame.Instance.CameraShake();
		}
	}

	private void FixedUpdate()
	{
		if (!(enemyController.network.GetHealth() > 0f) || !(enemyController.attack.targetPlayer != null))
		{
			return;
		}
		if (!enemyController.attack.targetPlayer.isEntangled)
		{
			enemyController.network.SetAnimation("Idle" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			UniTaskUtil.DelayedCall(this, 0.7f, () =>
			{
				enemyController.myrigidbody.isKinematic = false;
				enemyAttack.ChangeStateToIdle();
			}).Forget();
		}
		else if (enemyController.attack.targetPlayer.network.GetHealth() <= 0f)
		{
			enemyController.network.SetAnimation("Idle" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			UniTaskUtil.DelayedCall(this, 0.7f, () =>
			{
				enemyController.myrigidbody.isKinematic = false;
				enemyAttack.ChangeStateToIdle();
			}).Forget();
		}
	}
}
