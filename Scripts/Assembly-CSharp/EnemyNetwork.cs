using Fusion;
using Toked;
using UnityEngine;

public class EnemyNetwork : MonoBehaviour
{
	public EnemyController enemyController;

	public EnemyNetworkPhoton networkPhoton;

	public Rigidbody objectRigidbody;

	public NetworkObject networkObject;

	public NetworkPosition networkTransform;

	public SyncController syncController;

	private Vector3 nextPosition;

	private Quaternion nextRotation;

	private void Awake()
	{
	}

	public void FixedUpdate()
	{
		if (enemyController.aiPath.enabled)
		{
			enemyController.aiPath.MovementUpdate(Time.deltaTime, out nextPosition, out nextRotation);
			enemyController.aiPath.FinalizeMovement(nextPosition, nextRotation);
		}
	}

	public void SetHealth(float value)
	{
		if (value < 0f)
		{
			networkPhoton.health = 0;
		}
		else
		{
			networkPhoton.health = (short)value;
		}
	}

	public void SetIdxEnemy(int idx)
	{
		networkPhoton.idx = (byte)idx;
	}

	public void SetInactiveEnemy(bool value)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			networkPhoton.isNonActive = value;
		}
	}

	public void AddSubHealth(float value)
	{
		if ((float)networkPhoton.health + value < 0f)
		{
			networkPhoton.health = 0;
		}
		else
		{
			networkPhoton.health += (short)value;
		}
	}

	public void ExecHurt(float stuntTime, byte animationType, byte fromPlayer)
	{
		short stuntTime2 = (short)(stuntTime * 100f);
		networkPhoton.RpcExecHurt(networkPhoton.idx, stuntTime2, animationType, fromPlayer);
	}

	public void ExecInit(Vector3 pos)
	{
		GameManager.Instance.gameManagerPhoton.RpcInitEnemy(GetIDX());
	}

	public void ExecDoorBroken(byte uidInteractObj, Vector3 sourcePos, byte type)
	{
		networkPhoton.RpcExecDoorBroken(uidInteractObj, MathFunc.EncodeVector3ToULong(sourcePos), type);
	}

	public void ExecDoorAttacked(byte uidInteractObj)
	{
		networkPhoton.RpcExecDoorAttacked(uidInteractObj);
	}

	public void SetIsJumping(bool value)
	{
		networkPhoton.isJumping = value;
	}

	public void SetType(byte type)
	{
		networkPhoton.type = type;
	}

	public void SetIsHovering(bool value)
	{
		networkPhoton.isHovering = value;
	}

	public void SetIsHorde(bool value)
	{
		networkPhoton.isHorde = value;
	}

	public void SetDoSpesialAttack(bool value)
	{
		networkPhoton.doSpecialAttack1 = value;
	}

	public bool GetIsJumping()
	{
		return networkPhoton.isJumping;
	}

	public bool GetIsHorde()
	{
		return networkPhoton.isHorde;
	}

	public bool GetIsHovering()
	{
		return networkPhoton.isHovering;
	}

	public bool IsDead()
	{
		return GetHealth() <= 0f;
	}

	public float GetHealth()
	{
		return networkPhoton.health;
	}

	public byte GetIDX()
	{
		return networkPhoton.idx;
	}

	public bool IsSpecialAttacking()
	{
		return networkPhoton.doSpecialAttack1;
	}

	public bool IsNonActive()
	{
		return networkPhoton.isNonActive;
	}

	public short GetAngleDirection()
	{
		return (short)(networkPhoton.angleDirection * 45);
	}

	public void SetAngleDirection(float value)
	{
		if (value < 0f)
		{
			value += 360f;
		}
		value = Mathf.RoundToInt(value / 45f);
		networkPhoton.angleDirection = (byte)value;
	}

	public void SetAnimation(string animationName)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			if (animationName.IndexOf("Idle") >= 0)
			{
				networkPhoton.animationState = 0;
			}
			else if (animationName.IndexOf("Move") >= 0 && animationName.IndexOf("MoveAggro") < 0)
			{
				networkPhoton.animationState = 1;
			}
			else if (animationName.IndexOf("Attack") >= 0)
			{
				networkPhoton.animationState = 2;
			}
			else if (animationName.IndexOf("DeadFront") >= 0 && animationName.IndexOf("-0") >= 0)
			{
				networkPhoton.animationState = 3;
			}
			else if (animationName.IndexOf("Hurt") >= 0)
			{
				networkPhoton.animationState = 4;
			}
			else if (animationName.IndexOf("Knock") >= 0)
			{
				networkPhoton.animationState = 5;
			}
			else if (animationName.IndexOf("DeadFront") >= 0 && animationName.IndexOf("-1") >= 0)
			{
				networkPhoton.animationState = 6;
			}
			else if (animationName.IndexOf("DeadFront") >= 0 && animationName.IndexOf("-2") >= 0)
			{
				networkPhoton.animationState = 7;
			}
			else if (animationName.IndexOf("Jump") >= 0)
			{
				networkPhoton.animationState = 8;
			}
			else if (animationName.IndexOf("Land") >= 0 && animationName.IndexOf("-0") >= 0)
			{
				networkPhoton.animationState = 9;
			}
			else if (animationName.IndexOf("MoveAggro") >= 0)
			{
				enemyController.whisper.SetActive(value: false);
				networkPhoton.animationState = 10;
			}
			else if (animationName.IndexOf("Hovering") >= 0)
			{
				networkPhoton.animationState = 11;
			}
			else if (animationName.IndexOf("Dead2") >= 0)
			{
				networkPhoton.animationState = 12;
			}
			else if (animationName.IndexOf("Rise") >= 0)
			{
				networkPhoton.animationState = 13;
			}
			else if (animationName.IndexOf("Land") >= 0 && animationName.IndexOf("-2") >= 0)
			{
				networkPhoton.animationState = 14;
			}
			else if (animationName.IndexOf("StartAggro") >= 0)
			{
				networkPhoton.animationState = 15;
			}
			else if (animationName.IndexOf("Special1") >= 0)
			{
				networkPhoton.animationState = 16;
			}
			else if (animationName.IndexOf("Special2") >= 0)
			{
				networkPhoton.animationState = 17;
			}
			else if (animationName.IndexOf("bite") >= 0)
			{
				networkPhoton.animationState = 18;
			}
			else if (animationName.IndexOf("DeadBack") >= 0)
			{
				networkPhoton.animationState = 19;
			}
			else if (animationName.IndexOf("Special3") >= 0)
			{
				networkPhoton.animationState = 20;
			}
			enemyController.animator.Play(animationName);
		}
		else
		{
			enemyController.animator.Play(animationName);
		}
	}
}
