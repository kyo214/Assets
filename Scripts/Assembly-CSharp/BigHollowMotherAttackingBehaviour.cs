using Cysharp.Threading.Tasks;
using Toked;
using UnityEngine;

public class BigHollowMotherAttackingBehaviour : StateMachineBehaviour
{
	[SerializeField]
	private EnemyController enemyController;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (enemyController == null)
		{
			enemyController = animator.transform.parent.GetComponent<EnemyController>();
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!NetworkGameManager.Instance.isServer || !(enemyController.network.GetHealth() > 0f) || enemyController.isDead || enemyController.isDown)
		{
			return;
		}
		EnemyAttack enemyAttack = enemyController.attack;
		if (enemyAttack.timerTriggerAttack.isCompleted() && !enemyController.network.networkPhoton.isMoveToJump)
		{
			if ((bool)enemyController.aiTarget.target && MathFunc.Distance(enemyController.transform.position, enemyController.aiTarget.target.position) < enemyController.data.distAttack)
			{
				if (enemyController.GetCurrentStateHash() == AnimatorHashManager.AttackingHash)
				{
					if (enemyAttack.nextSpecialAttack1)
					{
						enemyAttack.EventSpecialAttack1.Invoke();
						enemyAttack.nextSpecialAttack1 = false;
						enemyAttack.timerSpecialAttack1.StartDuration(Random.Range(enemyAttack.minTimerSpecialAttack, enemyAttack.maxTimerSpecialAttack));
					}
					else
					{
						enemyAttack.EventBasicAttack.Invoke();
					}
				}
			}
			else
			{
				enemyController.AIEnable = true;
				enemyController.SetEnableAI(value: true);
				enemyController.SetAISpeed(enemyController.data.aggroSpeed);
				enemyController.network.SetAnimation("Move" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
				enemyController.SetState(EnemyState.Chasing);
			}
		}
		if (!enemyController.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") || !(enemyController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f))
		{
			return;
		}
		if (enemyAttack.prevTargetChasing != null)
		{
			enemyAttack.targetChasing = enemyAttack.prevTargetChasing;
			enemyAttack.SetAITarget(enemyAttack.targetChasing);
		}
		enemyController.isOnDestinationTarget = false;
		if (enemyController.GetCurrentStateHash() == AnimatorHashManager.AttackingHash)
		{
			bool flag = false;
			bool flag2 = false;
			foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerController)
			{
				if (enemyController.aiTarget.target == item.targetedPoint)
				{
					flag2 = true;
					if (item.network.GetHealth() <= 0f || (bool)item.network.playerPhoton.disconnected)
					{
						flag = true;
					}
				}
			}
			if (flag || !flag2)
			{
				enemyController.movement.SetStateAfterPlayerDead();
			}
			else
			{
				if (((bool)enemyController.barricadeCollider && enemyController.barricadeCollider.barricade.Hp > 0) || enemyController.network.IsSpecialAttacking() || enemyAttack.isRange || enemyController.aiPath.reachedDestination || (!enemyController.isElite && MathFunc.Distance(enemyController.middlePos.position, enemyAttack.targetChasing.position) < 1.5f) || (enemyController.isElite && MathFunc.Distance(enemyController.middlePos.position, enemyAttack.targetChasing.position) < 2f))
				{
					enemyController.SetAISpeed(0f);
					if (enemyAttack.targetChasing != null)
					{
						enemyController.movement.angleAnim = enemyAttack.AngleEnemy((enemyAttack.targetChasing.transform.position - enemyAttack.transform.position).normalized, enemyController.movement.angleAnim);
					}
					enemyController.network.SetAnimation("Idle" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
					if (!enemyController.barricadeCollider || enemyController.barricadeCollider.barricade.Hp <= 0)
					{
						enemyAttack.timerTriggerAttack.StartDuration(enemyController.data.delayAttack);
					}
				}
				else
				{
					enemyController.AIEnable = false;
					enemyController.SetEnableAI(value: false);
					enemyController.SetAISpeed(0f);
					enemyController.network.SetAnimation("Idle" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
					if (enemyAttack.targetChasing != null)
					{
						enemyController.movement.angleAnim = enemyAttack.AngleEnemy((enemyAttack.targetChasing.transform.position - enemyAttack.transform.position).normalized, enemyController.movement.angleAnim);
					}
					UniTaskUtil.DelayedCall(enemyController, 1f, () =>
					{
						if (!enemyController.AIEnable && !enemyController.isDead && !enemyController.isDown)
						{
							enemyController.AIEnable = true;
							enemyController.SetEnableAI(value: true);
							enemyController.SetAISpeed(enemyController.data.aggroSpeed);
							enemyController.network.SetAnimation("Move" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
							enemyAttack.timerTriggerAttack.StartDuration(enemyController.data.delayAttack);
							enemyController.SetState(EnemyState.Chasing);
						}
					}).Forget();
					enemyAttack.timerTriggerAttack.StartDuration(enemyController.data.delayAttack + 1f);
				}
				enemyController.isAttacking = false;
			}
		}
		else if (enemyController.isMoveable || (enemyController.AIEnable && enemyController.aiPath.speed != 0f))
		{
			enemyController.AIEnable = true;
			enemyController.SetEnableAI(value: true);
			enemyController.network.SetAnimation("Move" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			enemyController.isAttacking = false;
			enemyController.SetAISpeed(enemyController.data.aggroSpeed);
		}
		if (enemyController.network.IsSpecialAttacking())
		{
			enemyController.network.SetDoSpesialAttack(value: false);
		}
		enemyAttack.timerDelayAggro2.StartDuration(Random.Range(4f, 6f));
		enemyAttack.timerDurationAggro2.StopDuration();
	}
}
