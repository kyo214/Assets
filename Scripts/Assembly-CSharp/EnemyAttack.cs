using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Toked;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAttack : MonoBehaviour
{
	public EnemyController enemyController;

	public FieldOfView fov;

	public Transform targetChasing;

	public Transform prevTargetChasing;

	public bool isChasingSound;

	public bool FirstTimeChasing;

	public Transform meleeCollider;

	public XTimer timerDelayChasing;

	public XTimer timerRandomIdleChasing;

	[SerializeField]
	public XTimer timerIdleChasing;

	public XTimer timerTriggerAttack;

	public XTimer timerDelayAggro1;

	public XTimer timerDelayAggro2;

	public XTimer timerDurationAggro2;

	public XTimer timerSpecialAttack1;

	public float minTimerSpecialAttack;

	public float maxTimerSpecialAttack;

	public UnityEvent EventBasicAttack;

	public UnityEvent EventBasicAttack2;

	public UnityEvent EventSpecialAttack1;

	public UnityEvent EventSpecialAttack1Effect;

	public bool isRange;

	public bool nextSpecialAttack1;

	public string special1Type;

	public PlayerController targetPlayer;

	public bool isFoundPlayer;

	public float DistanceExplosion = 3f;

	public byte DamageExplosion = 30;

	private void Start()
	{
		if (!NetworkGameManager.Instance.isServer)
		{
			fov.SetDisable(value: true);
		}
		if (timerSpecialAttack1 != null)
		{
			timerSpecialAttack1.StartDuration(UnityEngine.Random.Range(minTimerSpecialAttack, maxTimerSpecialAttack));
		}
		isFoundPlayer = false;
	}

	private void FixedUpdate()
	{
		if (!NetworkGameManager.Instance.isServer || !(enemyController.network.GetHealth() > 0f) || enemyController.isDead || enemyController.isDown)
		{
			return;
		}
		RandomIdleChasing();
		NextAttack();
		if (timerDelayAggro1.isCompleted() && !enemyController.isHurt && Mathf.Approximately(enemyController.aiPath.maxSpeed, enemyController.data.GetSpeed()))
		{
			if (!enemyController.animator.GetCurrentAnimatorStateInfo(0).IsTag("Moving"))
			{
				enemyController.network.SetAnimation("Move" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
			enemyController.SetAISpeed(enemyController.data.aggroSpeed);
		}
	}

	public void StartChasing(bool playerSighted, Transform enemyTarget = null, Transform excludeTarget = null)
	{
		bool flag = false;
		enemyController.SetState(EnemyState.AlertChasing);
		enemyController.attack.timerIdleChasing.StopDuration();
		enemyController.attack.timerRandomIdleChasing.StopDuration();
		enemyController.isPlayerSighted = playerSighted;
		bool flag2 = true;
		if (enemyTarget != null)
		{
			targetChasing = enemyTarget;
			enemyController.isTargetMoveEnable = true;
		}
		else if (playerSighted)
		{
			enemyController.network.networkPhoton.RpcEnemyAggro();
			Transform transform = fov.NearestTarget();
			if (transform != null && transform.childCount > 0)
			{
				Transform child = fov.visibleTargets[0].GetChild(0);
				child.position = new Vector3(child.position.x, enemyController.transform.position.y, child.position.z);
				targetChasing = child;
			}
			else if (fov.visibleTargets.Count > 0)
			{
				targetChasing = fov.visibleTargets[0];
			}
			if (enemyController.isElite)
			{
				if ((bool)targetChasing)
				{
					enemyController.movement.angleAnim = AngleEnemy((targetChasing.transform.position - base.transform.position).normalized, enemyController.movement.angleAnim);
				}
				if (enemyController.movement.angleAnim != 0)
				{
					enemyController.network.SetAnimation("StartAggro" + enemyController.movement.angleAnim);
				}
				flag = true;
			}
			if (enemyController.headObj != null && !enemyController.isElite)
			{
				enemyController.ctrHeadShake = 0;
				enemyController.timerHeadShake.StartDuration(0.05f);
				enemyController.headObj.localEulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(-15, 15));
			}
		}
		else if (enemyController.isAlwaysChasing)
		{
			targetChasing = null;
			PlayerController playerNearest = NetworkGameManager.Instance.GetPlayerNearest(isHaveHealth: true, enemyController.transform.position);
			if (playerNearest != null)
			{
				playerNearest.targetedPoint.position = new Vector3(playerNearest.targetedPoint.position.x, enemyController.transform.position.y, playerNearest.targetedPoint.position.z);
				targetChasing = playerNearest.targetedPoint;
			}
			if (targetChasing != null)
			{
				enemyController.isPlayerSighted = true;
				enemyController.isTargetMoveEnable = false;
				SetAITarget(targetChasing);
			}
			else
			{
				flag2 = false;
			}
		}
		else
		{
			flag2 = false;
		}
		if (flag2)
		{
			enemyController.SetAISpeed(0f);
			if (enemyController.movement.angleAnim != 0 && !flag)
			{
				enemyController.network.SetAnimation("Idle" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
			timerDelayChasing.StartDuration(enemyController.data.aggroDelay);
			enemyController.movement.SetCurrentMoveSpeed(0f);
			enemyController.isOnDestinationTarget = false;
		}
	}

	public void SpawnedChasing(bool playerSighted, Transform enemyTarget = null)
	{
		if (!(enemyTarget != null))
		{
			return;
		}
		enemyController.SetEnableAI(value: false);
		enemyController.movement.angleAnim = AngleEnemy((enemyTarget.position - base.transform.position).normalized, -1);
		enemyController.network.SetAnimation("Move" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
		enemyController.myrigidbody.DOKill();
		enemyController.myrigidbody.DOMove(enemyController.transform.position, 0f);
		enemyController.myrigidbody.DOMove(enemyTarget.position, 2f).SetDelay(0.1f).SetSpeedBased(isSpeedBased: true)
			.SetEase(Ease.Linear)
			.OnComplete(() =>
			{
				if (!enemyController.aiPath.enabled && !enemyController.isJumping)
				{
					enemyController.movement.StartMove(ignoreLanded: true).Forget();
				}
				if (enemyController.isJumping)
				{
					enemyController.isJumping = false;
					enemyController.network.SetIsJumping(value: false);
				}
			});
		enemyController.isTargetMoveEnable = true;
	}

	public void EnemyAlertAndChasing()
	{
		enemyController.AIEnable = true;
		enemyController.SetEnableAI(value: true);
		SetAITarget(targetChasing);
		enemyController.SetState(EnemyState.Chasing);
		timerRandomIdleChasing.StartDuration(UnityEngine.Random.Range(4, 12));
		if (!enemyController.isHurt)
		{
			enemyController.SetAISpeed(enemyController.data.aggroSpeed);
		}
		enemyController.ctrHeadShake = 0;
		enemyController.timerHeadShake.StartDuration(UnityEngine.Random.Range(2, 6));
		if ((bool)enemyController.headObj)
		{
			enemyController.headObj.localEulerAngles = new Vector3(0f, 0f, 0f);
		}
		timerDelayChasing.StopDuration();
	}

	public int AngleEnemy(Vector3 direction, int prevAngle)
	{
		int num = -1000;
		if (direction != Vector3.zero)
		{
			num = Mathf.FloorToInt(Quaternion.LookRotation(direction, Vector3.up).eulerAngles.y);
		}
		int num2 = num - 45;
		int num3 = 0;
		if (num != -1000)
		{
			enemyController.network.SetAngleDirection((short)num2);
			num2 -= CameraGame.Instance.camRotate - 45;
			meleeCollider.localEulerAngles = new Vector3(0f, num2 - 45, 0f);
			if (num2 < 0)
			{
				num2 += 360;
			}
			if (prevAngle == -1)
			{
				prevAngle = num2 + 180;
			}
			float num4 = Mathf.Abs(num2 - prevAngle);
			if (num4 > 180f)
			{
				num4 = 360f - num4;
			}
			num3 = Mathf.FloorToInt((float)num2 / 90f) * 90 + 45;
			num3 %= 360;
			switch (num3)
			{
			case 0:
				num3 = 45;
				break;
			case 90:
				num3 = 135;
				break;
			case 180:
				num3 = 135;
				break;
			case 270:
				num3 = 225;
				break;
			}
			if (num4 < 55f && prevAngle != 0)
			{
				num3 = prevAngle;
			}
		}
		else
		{
			num3 = prevAngle;
		}
		return num3;
	}

	private void NextAttack()
	{
		if (timerSpecialAttack1 != null && timerSpecialAttack1.isCompleted())
		{
			nextSpecialAttack1 = true;
		}
	}

	public void ShowMeleeCollider(bool isNextFrameDisable = true)
	{
		int num = enemyController.network.GetAngleDirection() - 45;
		num -= CameraGame.Instance.camRotate - 45;
		meleeCollider.localEulerAngles = new Vector3(0f, num, 0f);
		if (isNextFrameDisable)
		{
			ColliderMelee().Forget();
		}
		else
		{
			meleeCollider.gameObject.SetActive(value: true);
		}
		if (enemyController.isElite)
		{
			if (enemyController.data.type == 100)
			{
				AudioManager.PlaySFXTransform("hairmaiden-attack-claw", enemyController.transform, isLocalPlayerTrigger: false);
			}
			else if (enemyController.data.type == 102)
			{
				AudioManager.PlaySFXTransform("hairmaiden-attack-claw", enemyController.transform, isLocalPlayerTrigger: false, 0.5f);
				AudioManager.PlaySFXTransform("enemy0-attack-swing", enemyController.transform, isLocalPlayerTrigger: false, 3f);
			}
			else
			{
				AudioManager.PlaySFXTransform("enemy0-attack-swing", enemyController.transform, isLocalPlayerTrigger: false, 3f);
			}
		}
		else
		{
			AudioManager.PlaySFXTransform("enemy0-attack-swing", enemyController.transform, isLocalPlayerTrigger: false);
		}
	}

	private async UniTask ColliderMelee()
	{
		CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
		meleeCollider.gameObject.SetActive(value: true);
		await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationTokenOnDestroy);
		meleeCollider.gameObject.SetActive(value: false);
	}

	public void RoamingToPlayer(PlayerController player, bool isAlwaysChasing = false)
	{
		if (player != null)
		{
			enemyController.targetObj.position = player.transform.position;
		}
		timerRandomIdleChasing.StartDuration(UnityEngine.Random.Range(4, 12));
		enemyController.isTargetMoveEnable = true;
		enemyController.isPlayerSighted = false;
		targetChasing = enemyController.targetObj;
		SetAITarget(targetChasing);
		enemyController.SetAISpeed(enemyController.data.aggroSpeed);
		enemyController.isRoaming = true;
		enemyController.SetState(EnemyState.Chasing);
		enemyController.isAlwaysChasing = isAlwaysChasing;
	}

	public void StopChasing()
	{
		if (enemyController.isPlayerSighted)
		{
			enemyController.isMoveable = true;
			enemyController.isAttacking = false;
			enemyController.attack.DisableAllTimer();
			SetAITargettoNull();
			enemyController.attack.targetChasing = null;
			enemyController.SetAISpeed(0f);
			enemyController.SetState(EnemyState.Patrol);
			enemyController.attack.fov.visibleTargets.Clear();
			enemyController.movement.SetCurrentMoveSpeed(enemyController.data.GetSpeed());
			enemyController.isPlayerSighted = false;
			enemyController.isTargetMoveEnable = false;
			enemyController.network.SetAnimation("Move" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			GameManager.Instance.waveManager.DisableHorde(enemyController);
		}
	}

	private void RandomIdleChasing()
	{
	}

	public void DisableAllTimer()
	{
		timerDelayChasing.StopDuration();
		timerRandomIdleChasing.StopDuration();
		timerIdleChasing.StopDuration();
		timerTriggerAttack.StopDuration();
	}

	public void SetStateToPatrol(bool isMoveable = true)
	{
		enemyController.isMoveable = isMoveable;
		enemyController.animator.speed = 1f;
		enemyController.isOnDestinationTarget = true;
		enemyController.isTargetMoveEnable = false;
		targetChasing = null;
		SetAITargettoNull();
		enemyController.SetAISpeed(0f);
		enemyController.isPlayerSighted = false;
		enemyController.SetState(EnemyState.Patrol);
		enemyController.AIEnable = true;
		enemyController.SetEnableAI(value: true);
		enemyController.movement.SetCurrentMoveSpeed(enemyController.data.GetSpeed());
		timerDelayChasing.StopDuration();
		timerRandomIdleChasing.StopDuration();
		timerIdleChasing.StopDuration();
		if (isMoveable)
		{
			enemyController.movement.timerChangeState.StartDuration(0.1f);
		}
		GameManager.Instance.waveManager.DisableHorde(enemyController);
	}

	public void ChangeStateToIdle()
	{
		enemyController.network.SetDoSpesialAttack(value: false);
		enemyController.isAttacking = false;
		if (enemyController.attack.targetPlayer != null && enemyController.attack.targetPlayer.network.GetHealth() > 0f)
		{
			StartChasing(playerSighted: true, targetPlayer.transform);
		}
		else
		{
			SetStateToPatrol();
		}
		enemyController.attack.targetPlayer = null;
		if (!timerSpecialAttack1.isRunning)
		{
			nextSpecialAttack1 = false;
			timerSpecialAttack1.StartDuration(UnityEngine.Random.Range(minTimerSpecialAttack, maxTimerSpecialAttack));
		}
	}

	public void SetAITarget(Transform targetTransform)
	{
		enemyController.aiTarget.target = targetTransform;
	}

	public void SetAITargettoNull()
	{
		enemyController.aiTarget.target = null;
	}

	public void SetAttackTarget(bool isSetPrevChasing = true)
	{
		if (targetChasing != null)
		{
			enemyController.targetObj.position = base.transform.position + (targetChasing.position - base.transform.position) * 1.2f;
			enemyController.targetObj.position = new Vector3(enemyController.targetObj.position.x, targetChasing.position.y, enemyController.targetObj.position.z);
			if (isSetPrevChasing)
			{
				prevTargetChasing = targetChasing;
			}
			targetChasing = enemyController.targetObj;
			SetAITarget(enemyController.targetObj);
		}
	}
}
