using Toked;
using UnityEngine;

public class EnemyChasingBehaviour : StateMachineBehaviour
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
			if (enemyController.isAttacking && enemyController.animator.GetCurrentAnimatorStateInfo(0).IsTag("Moving"))
			{
				enemyController.isAttacking = false;
			}
			if (!enemyController.isHurt)
			{
				if (attack.fov.visibleTargets.Count > 0 && enemyController.aiPath.maxSpeed != enemyController.data.aggroSpeed2)
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
					if (enemyController.network.GetIsHorde() && !attack.isFoundPlayer)
					{
						if (GameModes.Instance.modeGame == "Defense")
						{
							enemyController.SetAISpeed(enemyController.data.aggroSpeed);
						}
						else
						{
							enemyController.SetAISpeed(enemyController.data.aggroSpeed2);
						}
						if (enemyController.aiPath.velocity.magnitude > 1f && enemyController.movement.angleAnim != 0)
						{
							enemyController.network.SetAnimation("MoveAggro" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
						}
					}
					else if (enemyController.aiPath.maxSpeed != enemyController.data.aggroSpeed2)
					{
						enemyController.SetAISpeed(enemyController.data.aggroSpeed);
						if (enemyController.aiPath.velocity.magnitude > 1f)
						{
							enemyController.network.SetAnimation("Move" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
						}
					}
				}
				if (attack.timerDurationAggro2.isCompleted() && enemyController.aiPath.maxSpeed == enemyController.data.aggroSpeed2)
				{
					enemyController.SetAISpeed(enemyController.data.aggroSpeed);
					attack.timerDelayAggro2.StartDuration(Random.Range(4f, 6f));
					enemyController.animator.speed = 1f;
				}
				if (enemyController.aiPath.maxSpeed != enemyController.data.aggroSpeed2 && !attack.timerDelayAggro2.isRunning && enemyController.aiTarget.target != null && MathFunc.Distance(enemyController.transform.position, enemyController.aiTarget.target.position) <= enemyController.data.distanceAggro2)
				{
					enemyController.animator.speed = 1.3f;
					attack.timerDurationAggro2.StartDuration(Random.Range(3.5f, 4.5f));
					enemyController.SetAISpeed(enemyController.data.aggroSpeed2);
					enemyController.network.SetAnimation("MoveAggro" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
					if (attack.prevTargetChasing != null && attack.targetChasing != null)
					{
						PlayerController playerNearest = NetworkGameManager.Instance.GetPlayerNearest(isHaveHealth: true, attack.targetChasing.position);
						if (playerNearest != null)
						{
							attack.isChasingSound = false;
							attack.targetChasing = playerNearest.targetedPoint;
						}
					}
					if (!attack.isChasingSound)
					{
						attack.prevTargetChasing = attack.targetChasing;
					}
					else
					{
						attack.isChasingSound = false;
					}
					enemyController.targetObj.position = attack.targetChasing.position;
					attack.SetAITarget(attack.targetChasing);
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
		int num = -1;
		if (enemyController.aiTarget.target != null && !enemyController.isJumping && (enemyController.aiPath.reachedDestination || MathFunc.Distance(enemyController.transform.position, enemyController.aiTarget.target.position) < enemyController.data.distAttack) && !enemyController.isHurt)
		{
			if (attack.EventBasicAttack.GetPersistentEventCount() > 0)
			{
				if (enemyController.aiTarget.target.gameObject.activeSelf)
				{
					if (MathFunc.Distance(enemyController.transform.position, enemyController.aiTarget.target.position) < enemyController.data.distAttack)
					{
						if (!attack.timerTriggerAttack.isRunning && !attack.timerTriggerAttack.isPaused && !enemyController.isAttacking)
						{
							attack.isFoundPlayer = true;
							if ((bool)GameManagerPhoton.Instance && (bool)GameManagerPhoton.Instance.CurrentMission && GameManagerPhoton.Instance.CurrentMission.MissionObjective.IsSpawnEndlessHordeFromBeginning && !GameManager.Instance.isInfiniteHordeMode)
							{
								enemyController.isAlwaysChasing = false;
							}
							enemyController.animator.speed = 1f;
							attack.timerTriggerAttack.StartDuration(0.05f);
							enemyController.SetState(EnemyState.Attacking);
							enemyController.isOnDestinationTarget = true;
							enemyController.isTargetMoveEnable = false;
							enemyController.isPlayerSighted = false;
							attack.timerDelayChasing.StopDuration();
							attack.timerRandomIdleChasing.StopDuration();
							attack.timerIdleChasing.StopDuration();
						}
					}
					else if (enemyController.isTargetMoveEnable)
					{
						if (enemyController.isRoaming)
						{
							PlayerController randomPlayer = NetworkGameManager.Instance.GetRandomPlayer(isHaveHealth: true);
							num = enemyController.roamingGroup;
							foreach (EnemyController item in GameManager.Instance.arrEnemyController)
							{
								if (item.isRoaming && item.roamingGroup == num)
								{
									item.attack.RoamingToPlayer(randomPlayer);
								}
							}
						}
						else
						{
							attack.SetStateToPatrol();
						}
					}
				}
				else
				{
					enemyController.isMoveable = true;
					enemyController.animator.speed = 1f;
					enemyController.isAttacking = false;
					enemyController.attack.DisableAllTimer();
					attack.SetAITargettoNull();
					enemyController.attack.targetChasing = null;
					enemyController.SetAISpeed(0f);
					enemyController.SetState(EnemyState.Patrol);
					enemyController.AIEnable = true;
					enemyController.SetEnableAI(value: true);
					enemyController.attack.fov.visibleTargets.Clear();
					enemyController.movement.SetCurrentMoveSpeed(enemyController.data.GetSpeed());
					enemyController.attack.timerTriggerAttack.StopDuration();
					enemyController.attack.timerDelayChasing.StopDuration();
					enemyController.attack.timerRandomIdleChasing.StopDuration();
					enemyController.attack.timerIdleChasing.StopDuration();
					GameManager.Instance.waveManager.DisableHorde(enemyController);
				}
			}
			else
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
		if (attack.timerTriggerAttack.isCompleted() && !enemyController.network.networkPhoton.isMoveToJump)
		{
			if ((bool)enemyController.aiTarget.target && MathFunc.Distance(enemyController.transform.position, enemyController.aiTarget.target.position) < enemyController.data.distAttack)
			{
				if (enemyController.GetCurrentStateHash() == AnimatorHashManager.AttackingHash)
				{
					if (attack.nextSpecialAttack1)
					{
						attack.EventSpecialAttack1.Invoke();
						attack.nextSpecialAttack1 = false;
						attack.timerSpecialAttack1.StartDuration(Random.Range(attack.minTimerSpecialAttack, attack.maxTimerSpecialAttack));
					}
					else
					{
						attack.EventBasicAttack.Invoke();
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
				else
				{
					enemyController.isMoveable = true;
					enemyController.isAttacking = false;
					enemyController.attack.DisableAllTimer();
					attack.SetAITargettoNull();
					enemyController.attack.targetChasing = null;
					enemyController.SetAISpeed(0f);
					enemyController.SetState(EnemyState.Patrol);
					enemyController.attack.fov.visibleTargets.Clear();
					enemyController.movement.SetCurrentMoveSpeed(enemyController.data.GetSpeed());
				}
			}
		}
		if (enemyController.isAttacking && enemyController.animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && enemyController.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
		{
			enemyController.isAttacking = false;
		}
	}
}
