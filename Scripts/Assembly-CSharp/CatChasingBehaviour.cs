using Toked;
using UnityEngine;

public class CatChasingBehaviour : StateMachineBehaviour
{
	[SerializeField]
	private EnemyController enemyController;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (enemyController == null)
		{
			enemyController = animator.transform.parent.GetComponent<EnemyController>();
		}
		if (NetworkGameManager.Instance.isServer)
		{
			enemyController.network.networkPhoton.isChasing = true;
		}
		else
		{
			enemyController.network.networkPhoton.isChasing = false;
		}
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		if (!NetworkGameManager.Instance.isServer || !(enemyController.network.GetHealth() > 0f) || enemyController.isDead || enemyController.isDown)
		{
			return;
		}
		EnemyAttack attack = enemyController.attack;
		if (enemyController.GetCurrentStateHash() != AnimatorHashManager.ChasingHash && attack.targetChasing != null && attack.timerDelayChasing.isCompleted() && attack.targetChasing.gameObject.activeSelf)
		{
			attack.EnemyAlertAndChasing();
		}
		if (attack.fov.visibleTargets.Count > 0 && !enemyController.isPlayerSighted)
		{
			enemyController.isPlayerSighted = true;
			enemyController.isRoaming = false;
			enemyController.isTargetMoveEnable = false;
			Transform transform = attack.fov.NearestTarget();
			if (transform != null && transform.childCount > 0)
			{
				Transform child = attack.fov.visibleTargets[0].GetChild(0);
				child.position = new Vector3(child.position.x, enemyController.transform.position.y, child.position.z);
				attack.targetChasing = child;
			}
			else
			{
				attack.targetChasing = attack.fov.visibleTargets[0];
			}
			attack.SetAITarget(attack.targetChasing);
		}
		if (!attack.timerIdleChasing.isRunning)
		{
			if (!enemyController.isHurt)
			{
				if (attack.fov.visibleTargets.Count > 0)
				{
					Transform transform2 = attack.fov.NearestTarget();
					if (transform2 != null && transform2.childCount > 0)
					{
						Transform child2 = attack.fov.visibleTargets[0].GetChild(0);
						child2.position = new Vector3(child2.position.x, enemyController.transform.position.y, child2.position.z);
						attack.targetChasing = child2;
						attack.isChasingSound = false;
					}
					else
					{
						attack.targetChasing = attack.fov.visibleTargets[0];
						attack.isChasingSound = false;
					}
				}
				if (!enemyController.network.GetIsJumping() && !enemyController.isHurt && !enemyController.isAttacking && enemyController.barricadeCollider == null)
				{
					if (enemyController.aiPath.velocity.magnitude > 1f)
					{
						enemyController.movement.angleAnim = attack.AngleEnemy(enemyController.aiPath.desiredVelocity.normalized, enemyController.movement.angleAnim);
					}
					enemyController.AIEnable = true;
					enemyController.SetEnableAI(value: true);
					enemyController.SetAISpeed(enemyController.data.GetSpeed());
					if (enemyController.aiPath.velocity.magnitude > 1f)
					{
						enemyController.network.SetAnimation("Move" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
					}
				}
			}
			if ((((bool)enemyController.aiTarget.target && MathFunc.Distance(enemyController.transform.position, enemyController.aiTarget.target.position) > enemyController.data.distChasing) || !enemyController.aiTarget.target) && !enemyController.isAlwaysChasing)
			{
				attack.StopChasing();
			}
			if (!enemyController.aiPath.pathPending && !enemyController.aiPath.hasPath && !enemyController.isHurt && !enemyController.isDown && !enemyController.isDead && enemyController.animator.GetCurrentAnimatorStateInfo(0).IsTag("Moving"))
			{
				enemyController.movement.MoveRandomPath();
				enemyController.SetAISpeed(enemyController.data.GetSpeed());
				enemyController.movement.timerChangeState.StartDuration(Random.Range(enemyController.data.minTimeRandomState, enemyController.data.maxTimeRandomState));
				attack.timerDelayChasing.StartDuration(3f);
			}
		}
		if (!enemyController.isHurt && !enemyController.isAttacking)
		{
			if (attack.timerRandomIdleChasing.isCompleted())
			{
				attack.timerIdleChasing.StartDuration(Random.Range(0.5f, 1f));
				enemyController.SetAISpeed(0f);
				enemyController.network.SetAnimation("Idle" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
			if (attack.timerIdleChasing.isCompleted())
			{
				if (attack.targetChasing != null && attack.targetChasing.gameObject.activeSelf)
				{
					if (enemyController.isTargetMoveEnable || enemyController.isAlwaysChasing || MathFunc.Distance(attack.targetChasing.position, enemyController.middlePos.position) < enemyController.data.distChasing)
					{
						attack.timerRandomIdleChasing.StartDuration(Random.Range(4, 12));
						attack.SetAITarget(attack.targetChasing);
						if (!enemyController.barricadeCollider || enemyController.barricadeCollider.barricade.Hp <= 0)
						{
							enemyController.SetAISpeed(enemyController.data.aggroSpeed);
						}
					}
					else if (!enemyController.isAlwaysChasing)
					{
						attack.StopChasing();
					}
				}
				else if (NetworkGameManager.Instance.isServer && enemyController.attack.targetChasing == null)
				{
					enemyController.attack.fov.enabled = false;
					enemyController.AIEnable = false;
					enemyController.SetEnableAI(value: false);
					enemyController.SetAISpeed(0f);
					enemyController.SetState(EnemyState.Idle);
					enemyController.network.SetAnimation("Idle" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
					if (enemyController.attack.targetChasing != null)
					{
						enemyController.movement.angleAnim = enemyController.attack.AngleEnemy((enemyController.attack.targetChasing.transform.position - enemyController.attack.transform.position).normalized, enemyController.movement.angleAnim);
					}
				}
			}
		}
		if (enemyController.aiTarget.target != null && !enemyController.isJumping && (enemyController.aiPath.reachedDestination || MathFunc.Distance(enemyController.transform.position, enemyController.aiTarget.target.position) < 1f) && !enemyController.isHurt)
		{
			enemyController.attack.fov.enabled = false;
			enemyController.AIEnable = false;
			enemyController.SetEnableAI(value: false);
			enemyController.SetAISpeed(0f);
			enemyController.SetState(EnemyState.Patrol);
			enemyController.network.SetAnimation("Idle" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			if (attack.targetChasing != null)
			{
				enemyController.movement.angleAnim = attack.AngleEnemy((attack.targetChasing.transform.position - attack.transform.position).normalized, enemyController.movement.angleAnim);
			}
		}
	}
}
