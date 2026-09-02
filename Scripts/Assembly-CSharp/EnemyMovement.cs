using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Pathfinding;
using Toked;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
	[SerializeField]
	private EnemyController enemyController;

	[SerializeField]
	private EnemyNetwork enemyNetwork;

	public XTimer timerChangeAngle;

	public XTimer timerChangeState;

	public Seeker AiSeeker;

	public int JumpTagIndex = 1;

	public int m_angleAnim;

	public bool isIdle;

	public int prevAngleAnim;

	public int angleDirection;

	public Vector3 direction;

	[SerializeField]
	private float currentMoveSpeed;

	public int angleAnim
	{
		get
		{
			return m_angleAnim;
		}
		set
		{
			m_angleAnim = value;
			if (m_angleAnim > 315)
			{
				m_angleAnim -= 360;
			}
		}
	}

	private void Start()
	{
		if (NetworkGameManager.Instance.isServer && !GameManager.Instance.enemyStartChasing && !enemyController.isAlwaysChasing)
		{
			timerChangeAngle.StartDuration(UnityEngine.Random.Range(0f, enemyController.data.minTimeRandomAngle));
			timerChangeState.StartDuration(UnityEngine.Random.Range(0f, enemyController.data.minTimeRandomState));
			if (enemyController.aiPath.enabled || angleAnim <= 0)
			{
				SetAngle();
			}
		}
		if (enemyController.isElite)
		{
			AiSeeker.tagPenalties[JumpTagIndex] = 100000;
		}
	}

	private void FixedUpdate()
	{
		if (prevAngleAnim != angleAnim && NetworkGameManager.Instance.isServer && enemyController.aiPath.enabled && !enemyController.isFakeDead)
		{
			enemyController.attack.fov.transform.localEulerAngles = new Vector3(0f, angleAnim + CameraGame.Instance.camRotate, 0f);
			prevAngleAnim = angleAnim;
		}
	}

	public void SetAngle(int angle = -1)
	{
		if (angle == -1)
		{
			enemyNetwork.SetAngleDirection((short)UnityEngine.Random.Range(0, 360));
		}
		else
		{
			timerChangeAngle.StartDuration(UnityEngine.Random.Range(enemyController.data.minTimeRandomAngle, enemyController.data.maxTimeRandomAngle));
			enemyNetwork.SetAngleDirection(angle);
		}
		angleDirection = enemyNetwork.GetAngleDirection() - (CameraGame.Instance.camRotate - 45);
		if (angleDirection < 0)
		{
			angleDirection += 360;
		}
		angleAnim = SetAngleByCam(angleDirection);
		direction = new Vector3(Mathf.Sin(MathF.PI / 180f * (float)angleDirection), 0f, Mathf.Cos(MathF.PI / 180f * (float)angleDirection)).normalized;
		direction = MathFunc.IsoDirection(direction);
		if (enemyController.isFakeDead)
		{
			enemyNetwork.SetAnimation("Dead2Melee" + angleAnim);
		}
	}

	public int SetAngleByCam(int angle360)
	{
		if (angle360 < 0)
		{
			angle360 += 360;
		}
		if (angle360 < 0)
		{
			angle360 += 360;
		}
		angle360 = Mathf.FloorToInt((float)angle360 / 90f) * 90 + 45;
		angle360 %= 360;
		switch (angle360)
		{
		case 0:
			angle360 = 45;
			break;
		case 90:
			angle360 = 135;
			break;
		case 180:
			angle360 = 135;
			break;
		case 270:
			angle360 = 225;
			break;
		}
		return angle360;
	}

	public float GetCurrentMoveSpeed()
	{
		return currentMoveSpeed;
	}

	public void SetCurrentMoveSpeed(float value)
	{
		currentMoveSpeed = value;
	}

	public async UniTask StartMove(bool ignoreLanded = false)
	{
		CancellationToken token = this.GetCancellationTokenOnDestroy();
		if (!ignoreLanded)
		{
			if (enemyController.network.GetHealth() > 0f)
			{
				AudioManager.PlaySFXTransform("enemy0-jump-land", enemyController.transform, isLocalPlayerTrigger: false);
				enemyController.bodyCollider.enabled = true;
			}
			await UniTask.Delay(TimeSpan.FromSeconds(0.15000000596046448), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			if (enemyController.network.GetHealth() > 0f)
			{
				enemyController.enemyCollider.enabled = true;
				enemyController.stoperCollider.enabled = true;
				if (enemyController.movement.angleAnim == 0)
				{
					enemyController.movement.angleAnim = 45;
				}
				if (enemyController.isElite)
				{
					enemyController.network.SetAnimation("Land" + enemyController.movement.angleAnim);
				}
				else
				{
					enemyController.network.SetAnimation("Land" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
				}
			}
			await UniTask.Delay(TimeSpan.FromSeconds(0.20000000298023224), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		}
		if (enemyController.network.GetHealth() > 0f)
		{
			enemyController.isJumping = false;
			enemyController.network.SetIsJumping(value: false);
			if (!enemyController.aiPath.enabled)
			{
				enemyController.isOnDestinationTarget = false;
				enemyController.transform.localEulerAngles = Vector3.zero;
				enemyController.bodyCollider.enabled = true;
				if (NetworkGameManager.Instance.isServer)
				{
					enemyController.myrigidbody.isKinematic = false;
				}
				enemyController.isPlayerSighted = false;
				enemyController.AIEnable = true;
				enemyController.SetEnableAI(value: true);
				if (NetworkGameManager.Instance.isServer)
				{
					enemyController.attack.fov.SetDisable(value: false);
				}
				if (enemyController.isAlwaysChasing)
				{
					enemyController.attack.timerRandomIdleChasing.StartDuration(UnityEngine.Random.Range(8, 12));
					enemyController.ctrHeadShake = 0;
					if ((bool)enemyController.headObj)
					{
						enemyController.headObj.localEulerAngles = new Vector3(0f, 0f, 0f);
					}
					enemyController.attack.StartChasing(playerSighted: false);
					enemyController.SetAISpeed(enemyController.data.aggroSpeed2);
					enemyController.network.SetAnimation("MoveAggro" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
				}
				else
				{
					currentMoveSpeed = enemyController.data.GetSpeed();
					MoveRandomPath();
					enemyController.aiPath.destination = enemyController.aiPath.transform.position;
					enemyController.SetAISpeed(enemyController.data.GetSpeed());
					enemyController.network.SetAnimation("Move" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
				}
			}
		}
		if (!NetworkGameManager.Instance.isServer)
		{
			return;
		}
		float num = 9999f;
		PlayerController playerController = null;
		foreach (PlayerController item in NetworkGameManager.Instance.arrPlayerController)
		{
			float num2 = MathFunc.Distance(enemyController.transform.position, item.transform.position);
			if (num2 < 13f && num > num2)
			{
				num = num2;
				playerController = item;
			}
		}
		if (!(enemyController.network.GetHealth() > 0f) || !(playerController != null))
		{
			return;
		}
		Vector3 normalized = (enemyController.middlePos.position - playerController.weaponPos.position).normalized;
		if (!Physics.Raycast(playerController.weaponPos.position, normalized, num, GameManager.Instance.wallFloorCollider))
		{
			enemyController.network.networkPhoton.RpcEnemyAggro();
			playerController.targetedPoint.position = new Vector3(playerController.targetedPoint.position.x, enemyController.transform.position.y, playerController.targetedPoint.position.z);
			enemyController.attack.targetChasing = playerController.targetedPoint;
			enemyController.AIEnable = true;
			enemyController.SetEnableAI(value: true);
			enemyController.SetState(EnemyState.Chasing);
			enemyController.attack.SetAITarget(enemyController.attack.targetChasing);
			enemyController.attack.timerRandomIdleChasing.StartDuration(UnityEngine.Random.Range(6, 12));
			enemyController.SetAISpeed(enemyController.data.aggroSpeed2);
			enemyController.ctrHeadShake = 0;
			if ((bool)enemyController.headObj)
			{
				enemyController.headObj.localEulerAngles = new Vector3(0f, 0f, 0f);
			}
			enemyController.network.SetAnimation("MoveAggro" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
		}
	}

	public void SetStateAfterPlayerDead(bool isNotChasingType = false)
	{
		enemyController.isAttacking = false;
		enemyController.attack.DisableAllTimer();
		enemyController.attack.targetChasing = null;
		enemyController.attack.fov.visibleTargets.Clear();
		enemyController.attack.timerTriggerAttack.StopDuration();
		enemyController.attack.timerDelayChasing.StopDuration();
		enemyController.attack.timerRandomIdleChasing.StopDuration();
		enemyController.attack.timerIdleChasing.StopDuration();
		if (enemyController.isAlwaysChasing && !NetworkGameManager.Instance.IsAllPlayerDead(isOnlyCheckPlayerDown: true))
		{
			enemyController.network.SetAnimation("Move" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			enemyController.SetAISpeed(enemyController.data.aggroSpeed);
			enemyController.attack.timerRandomIdleChasing.StartDuration(UnityEngine.Random.Range(4, 12));
			enemyController.ctrHeadShake = 0;
			if ((bool)enemyController.headObj)
			{
				enemyController.headObj.localEulerAngles = new Vector3(0f, 0f, 0f);
			}
			enemyController.attack.StartChasing(playerSighted: false, null, enemyController.aiTarget.target);
		}
		else
		{
			enemyController.attack.SetStateToPatrol();
		}
	}

	public void MoveRandomPath()
	{
		if (enemyController.aiPath.enabled)
		{
			RandomPath p = RandomPath.Construct(base.transform.position, 6);
			enemyController.aiSeeker.CancelCurrentPathRequest();
			enemyController.aiSeeker.StartPath(p);
		}
	}
}
