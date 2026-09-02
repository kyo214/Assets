using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;
using Toked;
using UnityEngine;
using UnityEngine.Scripting;

[NetworkBehaviourWeaved(22)]
public class EnemyNetworkPhoton : NetworkBehaviour
{
	[SerializeField]
	private EnemyNetwork enemyNetwork;

	[SerializeField]
	[DefaultForProperty("idx", 0, 1)]
	private byte _idx;

	[SerializeField]
	[DefaultForProperty("type", 1, 1)]
	private byte _type;

	[SerializeField]
	[DefaultForProperty("skinType", 2, 1)]
	private byte _skinType;

	[SerializeField]
	[DefaultForProperty("animationState", 3, 1)]
	private byte _animationState;

	[SerializeField]
	[DefaultForProperty("angleDirection", 4, 1)]
	private byte _angleDirection;

	[SerializeField]
	[DefaultForProperty("deadType", 5, 1)]
	private byte _deadType;

	[SerializeField]
	[DefaultForProperty("health", 6, 1)]
	private short _health;

	[SerializeField]
	[DefaultForProperty("isNonActive", 7, 1)]
	private bool _isNonActive;

	[SerializeField]
	[DefaultForProperty("isMoveToJump", 8, 1)]
	private bool _isMoveToJump;

	[SerializeField]
	[DefaultForProperty("isJumping", 9, 1)]
	private bool _isJumping;

	[SerializeField]
	[DefaultForProperty("isChasing", 10, 1)]
	private bool _isChasing;

	[SerializeField]
	[DefaultForProperty("isDeaf", 11, 1)]
	private bool _isDeaf;

	[SerializeField]
	[DefaultForProperty("isDisableCollider", 12, 1)]
	private bool _isDisableCollider;

	[SerializeField]
	[DefaultForProperty("isHovering", 13, 1)]
	private bool _isHovering;

	[SerializeField]
	[DefaultForProperty("isFallingHovering", 14, 1)]
	private bool _isFallingHovering;

	[SerializeField]
	[DefaultForProperty("isRisingHovering", 15, 1)]
	private bool _isRisingHovering;

	[SerializeField]
	[DefaultForProperty("isHorde", 16, 1)]
	private bool _isHorde;

	[SerializeField]
	[DefaultForProperty("doSpecialAttack1", 17, 1)]
	private bool _doSpecialAttack1;

	[SerializeField]
	[DefaultForProperty("PosTarget", 18, 3)]
	private Vector3 _PosTarget;

	[SerializeField]
	[DefaultForProperty("AttackSeed", 21, 1)]
	private int _AttackSeed;

	private const int CHECK_INTERVAL = 1;

	private float timer;

	private static Changed<EnemyNetworkPhoton> _0024IL2CPP_CHANGED;

	private static ChangedDelegate<EnemyNetworkPhoton> _0024IL2CPP_CHANGED_DELEGATE;

	private static NetworkBehaviourCallbacks<EnemyNetworkPhoton> _0024IL2CPP_NETWORK_BEHAVIOUR_CALLBACKS;

	[Networked]
	[NetworkedWeaved(0, 1)]
	public unsafe byte idx
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.idx. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[0];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.idx. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[0] = (sbyte)value;
		}
	}

	[Networked(OnChanged = "OnTypeChanged")]
	[NetworkedWeaved(1, 1)]
	public unsafe byte type
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.type. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[4];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.type. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[4] = (sbyte)value;
		}
	}

	[Networked(OnChanged = "OnSkinChanged")]
	[NetworkedWeaved(2, 1)]
	public unsafe byte skinType
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.skinType. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[8];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.skinType. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[8] = (sbyte)value;
		}
	}

	[Networked(OnChanged = "OnAnimationChanged")]
	[NetworkedWeaved(3, 1)]
	public unsafe byte animationState
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.animationState. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[12];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.animationState. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[12] = (sbyte)value;
		}
	}

	[Networked(OnChanged = "OnDirectionChanged")]
	[NetworkedWeaved(4, 1)]
	public unsafe byte angleDirection
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.angleDirection. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[16];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.angleDirection. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[16] = (sbyte)value;
		}
	}

	[Networked]
	[NetworkedWeaved(5, 1)]
	public unsafe byte deadType
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.deadType. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((byte*)Ptr)[20];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.deadType. Networked properties can only be accessed when Spawned() has been called.");
			}
			((sbyte*)Ptr)[20] = (sbyte)value;
		}
	}

	[Networked(OnChanged = "OnHealthChanged")]
	[NetworkedWeaved(6, 1)]
	public unsafe short health
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.health. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ((short*)Ptr)[12];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.health. Networked properties can only be accessed when Spawned() has been called.");
			}
			((short*)Ptr)[12] = value;
		}
	}

	[Networked(OnChanged = "OnNonActiveChanged")]
	[NetworkedWeaved(7, 1)]
	public unsafe bool isNonActive
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isNonActive. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 7);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isNonActive. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 7, value);
		}
	}

	[Networked]
	[NetworkedWeaved(8, 1)]
	public unsafe bool isMoveToJump
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isMoveToJump. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 8);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isMoveToJump. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 8, value);
		}
	}

	[Networked]
	[NetworkedWeaved(9, 1)]
	public unsafe bool isJumping
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isJumping. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 9);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isJumping. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 9, value);
		}
	}

	[Networked]
	[NetworkedWeaved(10, 1)]
	public unsafe bool isChasing
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isChasing. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 10);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isChasing. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 10, value);
		}
	}

	[Networked]
	[NetworkedWeaved(11, 1)]
	public unsafe bool isDeaf
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isDeaf. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 11);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isDeaf. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 11, value);
		}
	}

	[Networked(OnChanged = "OnDisableCollider")]
	[NetworkedWeaved(12, 1)]
	public unsafe bool isDisableCollider
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isDisableCollider. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 12);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isDisableCollider. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 12, value);
		}
	}

	[Networked(OnChanged = "OnHovering")]
	[NetworkedWeaved(13, 1)]
	public unsafe bool isHovering
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isHovering. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 13);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isHovering. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 13, value);
		}
	}

	[Networked(OnChanged = "OnFallingHovering")]
	[NetworkedWeaved(14, 1)]
	public unsafe bool isFallingHovering
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isFallingHovering. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 14);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isFallingHovering. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 14, value);
		}
	}

	[Networked(OnChanged = "OnRisingHovering")]
	[NetworkedWeaved(15, 1)]
	public unsafe bool isRisingHovering
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isRisingHovering. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 15);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isRisingHovering. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 15, value);
		}
	}

	[Networked]
	[NetworkedWeaved(16, 1)]
	public unsafe bool isHorde
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isHorde. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 16);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.isHorde. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 16, value);
		}
	}

	[Networked(OnChanged = "OnDoSpecialAttack1")]
	[NetworkedWeaved(17, 1)]
	public unsafe bool doSpecialAttack1
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.doSpecialAttack1. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadBoolean(Ptr + 17);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.doSpecialAttack1. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteBoolean(Ptr + 17, value);
		}
	}

	[Networked]
	[NetworkedWeaved(18, 3)]
	public unsafe Vector3 PosTarget
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.PosTarget. Networked properties can only be accessed when Spawned() has been called.");
			}
			return ReadWriteUtilsForWeaver.ReadVector3(Ptr + 18, 0.001f);
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.PosTarget. Networked properties can only be accessed when Spawned() has been called.");
			}
			ReadWriteUtilsForWeaver.WriteVector3(Ptr + 18, 999.99994f, value);
		}
	}

	[Networked]
	[NetworkedWeaved(21, 1)]
	public unsafe int AttackSeed
	{
		get
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.AttackSeed. Networked properties can only be accessed when Spawned() has been called.");
			}
			return Ptr[21];
		}
		set
		{
			if (Ptr == null)
			{
				throw new InvalidOperationException("Error when accessing EnemyNetworkPhoton.AttackSeed. Networked properties can only be accessed when Spawned() has been called.");
			}
			Ptr[21] = value;
		}
	}

	private void Awake()
	{
		if ((object)enemyNetwork == null)
		{
			enemyNetwork = GetComponent<EnemyNetwork>();
		}
		timer = UnityEngine.Random.Range(0f, 1f);
	}

	public override void Spawned()
	{
		timer = UnityEngine.Random.Range(0f, 1f);
	}

	public static bool IsInAOI(Vector3 a, Vector3 b, float sqrRange)
	{
		float num = a.x - b.x;
		float num2 = a.y - b.y;
		float num3 = a.z - b.z;
		return num * num + num2 * num2 + num3 * num3 <= sqrRange;
	}

	[Preserve]
	public static void OnDirectionChanged(Changed<EnemyNetworkPhoton> changed)
	{
		EnemyController enemyController = changed.Behaviour.enemyNetwork.enemyController;
		if (!NetworkGameManager.Instance.isServer)
		{
			enemyController.isNotInAOI = false;
		}
		enemyController.network.syncController.SetSync(_isSync: true);
		enemyController.movement.angleDirection = enemyController.network.GetAngleDirection() - (CameraGame.Instance.camRotate - 45);
		if (enemyController.movement.angleDirection < 0)
		{
			enemyController.movement.angleDirection += 360;
		}
		if (!NetworkGameManager.Instance.isServer)
		{
			enemyController.movement.angleAnim = enemyController.movement.SetAngleByCam(enemyController.movement.angleDirection);
		}
		enemyController.movement.direction = new Vector3(Mathf.Sin(MathF.PI / 180f * (float)enemyController.movement.angleDirection), 0f, Mathf.Cos(MathF.PI / 180f * (float)enemyController.movement.angleDirection)).normalized;
		enemyController.movement.direction = MathFunc.IsoDirection(enemyController.movement.direction);
		if (!changed.Behaviour.enemyNetwork.enemyController.isDead && !changed.Behaviour.enemyNetwork.enemyController.isHurt)
		{
			if (changed.Behaviour.animationState == 0)
			{
				if (!enemyController.timerStunt.isRunning)
				{
					enemyController.animator.Play("Idle" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
				}
			}
			else if (changed.Behaviour.animationState == 1)
			{
				if (!enemyController.timerStunt.isRunning)
				{
					enemyController.movement.angleAnim = enemyController.movement.angleAnim % 360;
					enemyController.animator.Play("Move" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
				}
			}
			else if (changed.Behaviour.animationState == 2)
			{
				enemyController.animator.Play("Attack" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 3)
			{
				enemyController.animator.Play("DeadFront" + enemyController.movement.angleAnim + "-0");
			}
			else if (changed.Behaviour.animationState == 6)
			{
				enemyController.animator.Play("DeadFront" + enemyController.movement.angleAnim + "-1");
			}
			else if (changed.Behaviour.animationState == 7)
			{
				enemyController.animator.Play("DeadFront" + enemyController.movement.angleAnim + "-2");
			}
			else if (changed.Behaviour.animationState == 10)
			{
				enemyController.movement.angleAnim = enemyController.movement.angleAnim % 360;
				enemyController.animator.Play("MoveAggro" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
				enemyController.whisper.SetActive(value: false);
			}
			else if (changed.Behaviour.animationState == 11)
			{
				enemyController.animator.Play("Hovering" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim + "-" + enemyController.hoveringType);
			}
			else if (changed.Behaviour.animationState == 13)
			{
				enemyController.animator.Play("Rise" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
			if (changed.Behaviour.animationState == 18)
			{
				enemyController.stoperCollider.transform.localScale = new Vector3(2f, 2f, 2f);
				enemyController.animator.Play("bite" + enemyController.movement.angleAnim);
			}
			else
			{
				enemyController.stoperCollider.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
		if (NetworkGameManager.Instance.isServer)
		{
			if (changed.Behaviour.animationState == 3)
			{
				enemyController.animator.Play("DeadFront" + enemyController.movement.angleAnim + "-0");
			}
			else if (changed.Behaviour.animationState == 6)
			{
				enemyController.animator.Play("DeadFront" + enemyController.movement.angleAnim + "-1");
			}
			else if (changed.Behaviour.animationState == 7)
			{
				enemyController.animator.Play("DeadFront" + enemyController.movement.angleAnim + "-2");
			}
			else if (changed.Behaviour.animationState == 4)
			{
				enemyController.animator.Play("Hurt" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 5)
			{
				enemyController.animator.Play("Knock" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 11)
			{
				enemyController.animator.Play("Hovering" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim + "-" + enemyController.hoveringType);
			}
			else if (changed.Behaviour.animationState == 13)
			{
				enemyController.animator.Play("Rise" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
		}
	}

	[Preserve]
	public static void OnAnimationChanged(Changed<EnemyNetworkPhoton> changed)
	{
		EnemyController enemyController = changed.Behaviour.enemyNetwork.enemyController;
		if (!NetworkGameManager.Instance.isServer)
		{
			enemyController.isNotInAOI = false;
		}
		enemyController.network.syncController.SetSync(_isSync: true);
		if (enemyController.movement.angleAnim == 0 && changed.Behaviour.animationState != 12 && changed.Behaviour.animationState != 13 && changed.Behaviour.animationState != 18)
		{
			return;
		}
		if (!NetworkGameManager.Instance.isServer && (changed.Behaviour.animationState <= 2 || changed.Behaviour.animationState == 10))
		{
			if (!enemyController.network.IsNonActive() && !enemyController.isDeadAnimationPlaying && enemyController.isDead)
			{
				enemyController.lightCollider.lightCollider.enabled = true;
				enemyController.isDead = false;
				enemyController.colliderFOV.gameObject.SetActive(value: true);
				enemyController.enemyCollider.enabled = true;
				enemyController.VisibleSprite();
			}
			if (!enemyController.isDead)
			{
				if (!enemyController.colliderFOV.gameObject.activeSelf)
				{
					enemyController.colliderFOV.gameObject.SetActive(value: true);
				}
				if (!enemyController.colliderFOV.gameObject.activeSelf)
				{
					enemyController.colliderFOV.gameObject.SetActive(value: true);
				}
			}
		}
		if (!changed.Behaviour.enemyNetwork.enemyController.isDead && (!changed.Behaviour.enemyNetwork.enemyController.isHurt || changed.Behaviour.animationState == 12))
		{
			if (changed.Behaviour.animationState == 0)
			{
				if (!enemyController.timerStunt.isRunning)
				{
					enemyController.animator.Play("Idle" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
					if (enemyController.isDown)
					{
						enemyController.WakeUpFromDown();
					}
				}
			}
			else if (changed.Behaviour.animationState == 1)
			{
				if (!enemyController.timerStunt.isRunning)
				{
					enemyController.animator.Play("Move" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
					if (enemyController.isDown)
					{
						enemyController.WakeUpFromDown();
					}
				}
			}
			else if (changed.Behaviour.animationState == 2)
			{
				enemyController.animator.Play("Attack" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 3)
			{
				enemyController.animator.Play("DeadFront" + enemyController.movement.angleAnim + "-0");
			}
			else if (changed.Behaviour.animationState == 6)
			{
				enemyController.animator.Play("DeadFront" + enemyController.movement.angleAnim + "-1");
			}
			else if (changed.Behaviour.animationState == 7)
			{
				enemyController.animator.Play("DeadFront" + enemyController.movement.angleAnim + "-2");
			}
			else if (changed.Behaviour.animationState == 8)
			{
				enemyController.animator.Play("Jump" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 9)
			{
				enemyController.animator.Play("Land" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 10)
			{
				enemyController.animator.Play("MoveAggro" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
				enemyController.whisper.SetActive(value: false);
				if (enemyController.isDown)
				{
					enemyController.WakeUpFromDown();
				}
			}
			else if (changed.Behaviour.animationState == 11)
			{
				enemyController.animator.Play("Hovering" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim + "-" + enemyController.hoveringType);
			}
			else if (changed.Behaviour.animationState == 12)
			{
				if (!enemyController.isDown && enemyController.movement.angleAnim != 0)
				{
					enemyController.feedback.EnemyKnockDown();
				}
				if (enemyController.movement.angleAnim != 0)
				{
					enemyController.animator.Play("Dead2" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
				}
				else
				{
					enemyController.movement.angleAnim = UnityEngine.Random.Range(0, 4) * 90 + 45;
					enemyController.animator.Play("Dead2" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
					enemyController.feedback.EnemyKnockDown(isFromAttack: false);
				}
			}
			else if (changed.Behaviour.animationState == 13)
			{
				if (enemyController.enemyCollider.transform.localScale == Vector3.zero)
				{
					changed.Behaviour.isDisableCollider = false;
					enemyController.enemyCollider.transform.localScale = new Vector3(1f, 1f, 1f);
				}
				enemyController.animator.Play("Rise" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 14)
			{
				enemyController.animator.Play("Land" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim + "-2");
			}
			else if (changed.Behaviour.animationState == 15)
			{
				enemyController.animator.Play("StartAggro" + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 16)
			{
				enemyController.animator.Play("Special1-" + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 17)
			{
				if (!NetworkGameManager.Instance.isServer)
				{
					enemyController.object2D.position = enemyController.transform.position;
				}
				enemyController.animator.Play("Special2-" + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 18)
			{
				if (!NetworkGameManager.Instance.isServer)
				{
					enemyController.object2D.position = enemyController.transform.position;
				}
				enemyController.stoperCollider.transform.localScale = new Vector3(2f, 2f, 2f);
				enemyController.animator.Play("bite" + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 19)
			{
				enemyController.animator.Play("DeadBack" + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 20)
			{
				enemyController.animator.Play("Special3-" + enemyController.movement.angleAnim);
			}
			else
			{
				enemyController.stoperCollider.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
		if (NetworkGameManager.Instance.isServer)
		{
			if (changed.Behaviour.animationState == 3)
			{
				enemyController.animator.Play("DeadFront" + enemyController.movement.angleAnim + "-0");
			}
			else if (changed.Behaviour.animationState == 6)
			{
				enemyController.animator.Play("DeadFront" + enemyController.movement.angleAnim + "-1");
			}
			else if (changed.Behaviour.animationState == 7)
			{
				enemyController.animator.Play("DeadFront" + enemyController.movement.angleAnim + "-2");
			}
			else if (changed.Behaviour.animationState == 4)
			{
				enemyController.animator.Play("Hurt" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim, -1, 0f);
			}
			else if (changed.Behaviour.animationState == 5)
			{
				enemyController.animator.Play("Knock" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 8)
			{
				enemyController.animator.Play("Jump" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 9)
			{
				enemyController.animator.Play("Land" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 11)
			{
				enemyController.animator.Play("Hovering" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim + "-" + enemyController.hoveringType);
			}
			else if (changed.Behaviour.animationState == 12)
			{
				enemyController.animator.Play("Dead2" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
				if (!enemyController.isDown)
				{
					enemyController.feedback.EnemyKnockDown();
				}
			}
			else if (changed.Behaviour.animationState == 13)
			{
				enemyController.animator.Play("Rise" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 14)
			{
				enemyController.animator.Play("Land" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim + "-2");
			}
			else if (changed.Behaviour.animationState == 15)
			{
				enemyController.animator.Play("StartAggro" + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 16)
			{
				enemyController.animator.Play("Special1-" + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 17)
			{
				enemyController.animator.Play("Special2-" + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 18)
			{
				enemyController.stoperCollider.transform.localScale = new Vector3(2f, 2f, 2f);
				enemyController.animator.Play("bite" + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 19)
			{
				enemyController.animator.Play("DeadBack" + enemyController.movement.angleAnim);
			}
			else if (changed.Behaviour.animationState == 20)
			{
				enemyController.animator.Play("Special3-" + enemyController.movement.angleAnim);
			}
			else
			{
				enemyController.stoperCollider.transform.localScale = new Vector3(1f, 1f, 1f);
			}
		}
		else if (changed.Behaviour.animationState == 5 && !enemyController.hurtByThisNetwork)
		{
			enemyController.animator.Play("Knock" + enemyController.data.arrWeaponState[enemyController.data.weaponState] + enemyController.movement.angleAnim);
			enemyController.feedback.HurtVFX().Forget();
		}
	}

	[Preserve]
	public static void OnNonActiveChanged(Changed<EnemyNetworkPhoton> changed)
	{
		if (!NetworkGameManager.Instance.isServer)
		{
			changed.Behaviour.enemyNetwork.enemyController.HideSprite(isCreated: false, isHideBlood: false);
		}
	}

	[Preserve]
	public static void OnHealthChanged(Changed<EnemyNetworkPhoton> changed)
	{
		EnemyController enemyController = changed.Behaviour.enemyNetwork.enemyController;
		if (!NetworkGameManager.Instance.isServer)
		{
			enemyController.isNotInAOI = false;
		}
		changed.LoadOld();
		short num = changed.Behaviour.health;
		changed.LoadNew();
		if ((bool)GameManagerPhoton.Instance)
		{
			if (num > 0 && changed.Behaviour.health <= 0 && !enemyController.isDead && ((bool)GameManagerPhoton.Instance.HostLoadingGameComplete || LobbyManager.Instance != null))
			{
				enemyController.whisper.SetActive(value: false);
				changed.Behaviour.enemyNetwork.syncController.SetSync(_isSync: true);
				changed.Behaviour.enemyNetwork.enemyController.Dead(1).Forget();
			}
		}
		else if (changed.Behaviour.health > 0 && !enemyController.animator.GetCurrentAnimatorStateInfo(0).IsTag("Hovering") && !enemyController.isDown)
		{
			enemyController.bodyCollider.enabled = true;
			if (NetworkGameManager.Instance.isServer)
			{
				enemyController.myrigidbody.isKinematic = false;
			}
		}
	}

	[Preserve]
	public static void OnHovering(Changed<EnemyNetworkPhoton> changed)
	{
		EnemyController enemyController = changed.Behaviour.enemyNetwork.enemyController;
		if (changed.Behaviour.isHovering)
		{
			enemyController.bodyCollider.excludeLayers = 0;
		}
	}

	[Preserve]
	public static void OnFallingHovering(Changed<EnemyNetworkPhoton> changed)
	{
		EnemyController enemy = changed.Behaviour.enemyNetwork.enemyController;
		enemy.bodyCollider.excludeLayers = LayerMask.GetMask("Character");
		Transform child = enemy.object2D.GetChild(0);
		child.DOKill();
		UnityEngine.Random.InitState(changed.Behaviour.idx);
		child.DOLocalMoveY(0.742f, UnityEngine.Random.Range(0.5f, 0.6f)).SetEase(Ease.InQuad).SetDelay(UnityEngine.Random.Range(0f, 2f))
			.OnComplete(() =>
			{
				enemy.Rise();
			});
		UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
	}

	[Preserve]
	public static void OnRisingHovering(Changed<EnemyNetworkPhoton> changed)
	{
		EnemyController enemyController = changed.Behaviour.enemyNetwork.enemyController;
		enemyController.bodyCollider.isTrigger = false;
		enemyController.enemyCollider.enabled = true;
		enemyController.stoperCollider.enabled = true;
	}

	[Preserve]
	public static void OnDoSpecialAttack1(Changed<EnemyNetworkPhoton> changed)
	{
		if (changed.Behaviour.doSpecialAttack1 && changed.Behaviour.enemyNetwork.enemyController.SpecialAttackSoundName != "")
		{
			AudioManager.PlaySFXTransform(changed.Behaviour.enemyNetwork.enemyController.SpecialAttackSoundName, changed.Behaviour.transform, isLocalPlayerTrigger: false);
		}
	}

	[Preserve]
	public static void OnTypeChanged(Changed<EnemyNetworkPhoton> changed)
	{
		changed.Behaviour.enemyNetwork.enemyController.ChangeType(changed.Behaviour.type);
	}

	[Preserve]
	public static void OnSkinChanged(Changed<EnemyNetworkPhoton> changed)
	{
		changed.Behaviour.enemyNetwork.enemyController.ChangeSkin(changed.Behaviour.skinType);
	}

	[Preserve]
	public static void OnDisableCollider(Changed<EnemyNetworkPhoton> changed)
	{
		if (changed.Behaviour.isDisableCollider)
		{
			changed.Behaviour.enemyNetwork.enemyController.enemyCollider.transform.localScale = Vector3.zero;
		}
		else
		{
			changed.Behaviour.enemyNetwork.enemyController.enemyCollider.transform.localScale = Vector3.one;
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.All)]
	public unsafe void RpcExecHurt(byte idx, short stuntTime, byte animaTionType, byte playerID)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 2) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void EnemyNetworkPhoton::RpcExecHurt(System.Byte,System.Int16,System.Byte,System.Byte)", Object, 2);
				return;
			}
			int num = 8;
			num += 4;
			num += 4;
			num += 4;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void EnemyNetworkPhoton::RpcExecHurt(System.Byte,System.Int16,System.Byte,System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 1), data);
				data[num2] = idx;
				num2 += 4 & -4;
				*(short*)(data + num2) = stuntTime;
				num2 += 5 & -4;
				data[num2] = animaTionType;
				num2 += 4 & -4;
				data[num2] = playerID;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		PlayerController player = NetworkGameManager.Instance.GetPlayer(playerID);
		if (!player.network.isLocalPlayer)
		{
			float stuntTime2 = stuntTime / 100;
			GameManager.Instance.GetEnemy(idx).feedback.Hurt(stuntTime2, animaTionType, player.weaponController.muzzle.transform.localEulerAngles.y, player).Forget();
		}
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void RpcSetHealth(short value)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 2) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void EnemyNetworkPhoton::RpcSetHealth(System.Int16)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void EnemyNetworkPhoton::RpcSetHealth(System.Int16)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 2), data);
					*(short*)(data + num2) = value;
					num2 += 5 & -4;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		health = value;
	}

	[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
	public unsafe void RpcAddHealth(short value)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 2) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void EnemyNetworkPhoton::RpcAddHealth(System.Int16)", Object, 2);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void EnemyNetworkPhoton::RpcAddHealth(System.Int16)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 3), data);
					*(short*)(data + num2) = value;
					num2 += 5 & -4;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		if (health + value < 0)
		{
			health = 0;
		}
		else
		{
			health += value;
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcExecDoorBroken(short uidInteractObj, ulong sourcePos, byte type)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void EnemyNetworkPhoton::RpcExecDoorBroken(System.Int16,System.UInt64,System.Byte)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			num += 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void EnemyNetworkPhoton::RpcExecDoorBroken(System.Int16,System.UInt64,System.Byte)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 4), data);
				*(short*)(data + num2) = uidInteractObj;
				num2 += 5 & -4;
				*(ulong*)(data + num2) = sourcePos;
				num2 += 8;
				data[num2] = type;
				num2 += 4 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		ItemInteractable itemInteractable = GameManager.Instance.GetItemInteractable(uidInteractObj);
		if (itemInteractable.animatorTrigger1 != null && itemInteractable.animatorTrigger1.gameObject.TryGetComponent<DoorControl>(out var component))
		{
			component.ExecuteDoorBroken(MathFunc.DecodeVector3FromULong(sourcePos), type);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcExecDoorAttacked(short uidInteractObj)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void EnemyNetworkPhoton::RpcExecDoorAttacked(System.Int16)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void EnemyNetworkPhoton::RpcExecDoorAttacked(System.Int16)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 5), data);
				*(short*)(data + num2) = uidInteractObj;
				num2 += 5 & -4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		ItemInteractable itemInteractable = GameManager.Instance.GetItemInteractable(uidInteractObj);
		if (itemInteractable.animatorTrigger1 != null && itemInteractable.animatorTrigger1.gameObject.TryGetComponent<DoorControl>(out var component))
		{
			component.ExecuteDoorAttacked();
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcEnemyAggro()
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void EnemyNetworkPhoton::RpcEnemyAggro()", Object, 7);
				return;
			}
			int num = 8;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void EnemyNetworkPhoton::RpcEnemyAggro()", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 6), data);
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		if (enemyNetwork.enemyController.AggroSoundName != "")
		{
			AudioManager.PlaySFXTransform(enemyNetwork.enemyController.AggroSoundName, base.transform, isLocalPlayerTrigger: false);
		}
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RpcCheckEnemyAggro(int index)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void EnemyNetworkPhoton::RpcCheckEnemyAggro(System.Int32)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void EnemyNetworkPhoton::RpcCheckEnemyAggro(System.Int32)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 7), data);
				*(int*)(data + num2) = index;
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		PlayerController player = NetworkGameManager.Instance.GetPlayer(index);
		enemyNetwork.enemyController.CheckEnemyAgro(player);
	}

	[Rpc(RpcSources.All, RpcTargets.StateAuthority)]
	public unsafe void RPCSetEntanglePlayer(byte getIdx)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void EnemyNetworkPhoton::RPCSetEntanglePlayer(System.Byte)", Object, 7);
				return;
			}
			if ((localAuthorityMask & 1) != 1)
			{
				int num = 8;
				num += 4;
				if (!SimulationMessage.CanAllocateUserPayload(num))
				{
					NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void EnemyNetworkPhoton::RPCSetEntanglePlayer(System.Byte)", num);
					return;
				}
				if (Runner.HasAnyActiveConnections())
				{
					SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
					byte* data = SimulationMessage.GetData(ptr);
					int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 8), data);
					data[num2] = getIdx;
					num2 += 4 & -4;
					ptr->Offset = num2 * 8;
					Runner.SendRpc(ptr);
				}
				if ((localAuthorityMask & 1) == 0)
				{
					return;
				}
			}
		}
		PlayerController player = NetworkGameManager.Instance.GetPlayer(getIdx);
		enemyNetwork.enemyController.attack.targetPlayer = player;
		enemyNetwork.enemyController.attack.EventSpecialAttack1Effect.Invoke();
	}

	[Rpc(RpcSources.All, RpcTargets.All)]
	public unsafe void RPCPlayAnimation(int nameAnimHash)
	{
		if (InvokeRpc)
		{
			InvokeRpc = false;
		}
		else
		{
			NetworkBehaviourUtils.ThrowIfBehaviourNotInitialized(this);
			if (Runner.Stage == SimulationStages.Resimulate)
			{
				return;
			}
			int localAuthorityMask = Object.GetLocalAuthorityMask();
			if ((localAuthorityMask & 7) == 0)
			{
				NetworkBehaviourUtils.NotifyLocalSimulationNotAllowedToSendRpc("System.Void EnemyNetworkPhoton::RPCPlayAnimation(System.Int32)", Object, 7);
				return;
			}
			int num = 8;
			num += 4;
			if (!SimulationMessage.CanAllocateUserPayload(num))
			{
				NetworkBehaviourUtils.NotifyRpcPayloadSizeExceeded("System.Void EnemyNetworkPhoton::RPCPlayAnimation(System.Int32)", num);
				return;
			}
			if (Runner.HasAnyActiveConnections())
			{
				SimulationMessage* ptr = SimulationMessage.Allocate(Runner.Simulation, num);
				byte* data = SimulationMessage.GetData(ptr);
				int num2 = RpcHeader.Write(RpcHeader.Create(Object.Id, ObjectIndex, 9), data);
				*(int*)(data + num2) = nameAnimHash;
				num2 += 4;
				ptr->Offset = num2 * 8;
				Runner.SendRpc(ptr);
			}
			if ((localAuthorityMask & 7) == 0)
			{
				return;
			}
		}
		animationState = byte.MaxValue;
		enemyNetwork.enemyController.animator.Play(nameAnimHash);
	}

	public override void CopyBackingFieldsToState(bool P_0)
	{
		idx = _idx;
		type = _type;
		skinType = _skinType;
		animationState = _animationState;
		angleDirection = _angleDirection;
		deadType = _deadType;
		health = _health;
		isNonActive = _isNonActive;
		isMoveToJump = _isMoveToJump;
		isJumping = _isJumping;
		isChasing = _isChasing;
		isDeaf = _isDeaf;
		isDisableCollider = _isDisableCollider;
		isHovering = _isHovering;
		isFallingHovering = _isFallingHovering;
		isRisingHovering = _isRisingHovering;
		isHorde = _isHorde;
		doSpecialAttack1 = _doSpecialAttack1;
		PosTarget = _PosTarget;
		AttackSeed = _AttackSeed;
	}

	public override void CopyStateToBackingFields()
	{
		_idx = idx;
		_type = type;
		_skinType = skinType;
		_animationState = animationState;
		_angleDirection = angleDirection;
		_deadType = deadType;
		_health = health;
		_isNonActive = isNonActive;
		_isMoveToJump = isMoveToJump;
		_isJumping = isJumping;
		_isChasing = isChasing;
		_isDeaf = isDeaf;
		_isDisableCollider = isDisableCollider;
		_isHovering = isHovering;
		_isFallingHovering = isFallingHovering;
		_isRisingHovering = isRisingHovering;
		_isHorde = isHorde;
		_doSpecialAttack1 = doSpecialAttack1;
		_PosTarget = PosTarget;
		_AttackSeed = AttackSeed;
	}

	[NetworkRpcWeavedInvoker(1, 2, 7)]
	[Preserve]
	protected unsafe static void RpcExecHurt_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte b = num2;
		short num3 = *(short*)(data + num);
		num += 5 & -4;
		short stuntTime = num3;
		byte num4 = data[num];
		num += 4 & -4;
		byte animaTionType = num4;
		byte num5 = data[num];
		num += 4 & -4;
		byte playerID = num5;
		behaviour.InvokeRpc = true;
		((EnemyNetworkPhoton)behaviour).RpcExecHurt(b, stuntTime, animaTionType, playerID);
	}

	[NetworkRpcWeavedInvoker(2, 2, 1)]
	[Preserve]
	protected unsafe static void RpcSetHealth_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short value = num2;
		behaviour.InvokeRpc = true;
		((EnemyNetworkPhoton)behaviour).RpcSetHealth(value);
	}

	[NetworkRpcWeavedInvoker(3, 2, 1)]
	[Preserve]
	protected unsafe static void RpcAddHealth_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short value = num2;
		behaviour.InvokeRpc = true;
		((EnemyNetworkPhoton)behaviour).RpcAddHealth(value);
	}

	[NetworkRpcWeavedInvoker(4, 7, 7)]
	[Preserve]
	protected unsafe static void RpcExecDoorBroken_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short uidInteractObj = num2;
		long num3 = *(long*)(data + num);
		num += 8;
		ulong sourcePos = (ulong)num3;
		byte num4 = data[num];
		num += 4 & -4;
		byte b = num4;
		behaviour.InvokeRpc = true;
		((EnemyNetworkPhoton)behaviour).RpcExecDoorBroken(uidInteractObj, sourcePos, b);
	}

	[NetworkRpcWeavedInvoker(5, 7, 7)]
	[Preserve]
	protected unsafe static void RpcExecDoorAttacked_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		short num2 = *(short*)(data + num);
		num += 5 & -4;
		short uidInteractObj = num2;
		behaviour.InvokeRpc = true;
		((EnemyNetworkPhoton)behaviour).RpcExecDoorAttacked(uidInteractObj);
	}

	[NetworkRpcWeavedInvoker(6, 7, 7)]
	[Preserve]
	protected unsafe static void RpcEnemyAggro_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		behaviour.InvokeRpc = true;
		((EnemyNetworkPhoton)behaviour).RpcEnemyAggro();
	}

	[NetworkRpcWeavedInvoker(7, 7, 7)]
	[Preserve]
	protected unsafe static void RpcCheckEnemyAggro_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int index = num2;
		behaviour.InvokeRpc = true;
		((EnemyNetworkPhoton)behaviour).RpcCheckEnemyAggro(index);
	}

	[NetworkRpcWeavedInvoker(8, 7, 1)]
	[Preserve]
	protected unsafe static void RPCSetEntanglePlayer_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		byte num2 = data[num];
		num += 4 & -4;
		byte getIdx = num2;
		behaviour.InvokeRpc = true;
		((EnemyNetworkPhoton)behaviour).RPCSetEntanglePlayer(getIdx);
	}

	[NetworkRpcWeavedInvoker(9, 7, 7)]
	[Preserve]
	protected unsafe static void RPCPlayAnimation_0040Invoker(NetworkBehaviour behaviour, SimulationMessage* message)
	{
		byte* data = SimulationMessage.GetData(message);
		int num = (RpcHeader.ReadSize(data) + 3) & -4;
		int num2 = *(int*)(data + num);
		num += 4;
		int nameAnimHash = num2;
		behaviour.InvokeRpc = true;
		((EnemyNetworkPhoton)behaviour).RPCPlayAnimation(nameAnimHash);
	}
}
