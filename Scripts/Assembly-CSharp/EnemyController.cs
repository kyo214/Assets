using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;
using Pathfinding;
using Pathfinding.RVO;
using TMPro;
using Toked;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.U2D.Animation;
using _Modules.Achievement.Scripts;
using _Modules.Effects.StatusEffectsScripts;
using _Modules.Enemies.BaseScripts;

public class EnemyController : MonoBehaviour
{
	public static string EMEMY_COLLIDER_TAG = "EnemyCollider";

	public static string EMEMY_TAG = "Enemy";

	public EnemyNetwork network;

	public EnemyData data;

	public EnemyMovement movement;

	public AIDestinationSetter aiTarget;

	[FormerlySerializedAs("AIisEnable")]
	public bool AIEnable;

	public AIPath aiPath;

	public Seeker aiSeeker;

	public RVOController RVOController;

	public EnemyAttack attack;

	public EnemyCharacterRenderController enemyCharacterRenderController;

	public Animator animator;

	public Animator meleeHitAnim;

	public FeedbackEnemyController feedback;

	public List<SpriteRenderer> allSpriteParts = new List<SpriteRenderer>();

	public List<SpriteResolver> allSpriteResolver = new List<SpriteResolver>();

	public List<SpriteRenderer> upperParts = new List<SpriteRenderer>();

	public SpriteLibrary headLib;

	public SpriteLibrary bodyLib;

	public SpriteLibrary hand1Lib;

	public SpriteLibrary hand2Lib;

	public SpriteLibrary leg1Lib;

	public SpriteLibrary leg2Lib;

	public SpriteLibrary weaponLib;

	public SpriteRenderer weaponSprite;

	public List<SpriteRenderer> eyeGlow = new List<SpriteRenderer>();

	public SpriteRenderer shadow;

	public Transform object2D;

	public SortingGroup object2DSortingGroup;

	public Transform targetObj;

	public Transform headObj;

	public SpriteRenderer bloodPool;

	public SpriteRenderer HeadSpriteSource;

	public SpriteRenderer HeadSprite;

	public SpriteRenderer HeadSpriteShadow;

	public MaterialPropertyBlock MPB;

	public List<Transform> hitPos = new List<Transform>();

	public Transform middlePos;

	public Transform headTransform;

	public Transform bodyTransform;

	public PosEnemy LastPosEnemy;

	[SerializeField]
	public EnemyState state;

	[SerializeField]
	public Animator animatorState;

	public CapsuleCollider bodyCollider;

	public Rigidbody myrigidbody;

	public Collider enemyCollider;

	public Collider stoperCollider;

	public EnemyLightCollider lightCollider;

	public Transform colliderFOV;

	public List<string> roomColliders = new List<string>();

	public LayerMask layerWallCollider;

	public LayerMask layerDoorCollider;

	public XTimer timerStunt;

	public XTimer timerAttackDoor;

	public XTimer timerAttackBarricade;

	public XTimer timerHeadShake;

	public bool isElite;

	public bool isEliteScore;

	public bool isAlwaysChasing;

	public GameObject whisper;

	public string AggroSoundName;

	public string SpecialAttackSoundName;

	public JumpEnemyCollider barricadeCollider;

	public bool initialized;

	public bool isHurt;

	public bool isDown;

	public bool isPlayerSighted;

	public bool isPlayerOnRange;

	public bool isTargetMoveEnable;

	public bool isOnDestinationTarget;

	public bool isAttacking;

	public bool needInitSprite;

	public bool isDestroyed;

	public bool isDead;

	public bool isDeadAnimationPlaying;

	public bool isSpriteInactive;

	public Vector3 dirJump;

	public bool isJumping;

	public bool isRoaming;

	public int roamingGroup;

	public int hoveringType;

	public bool hurtByThisNetwork;

	public bool isWaveSpawned;

	public int ctrHeadShake;

	public bool isMoveable;

	public UnityEvent EventSpecialDead;

	public bool isFakeDead;

	public NetworkPosition NetworkPos;

	public bool isNotInAOI;

	[SerializeField]
	private float _speedMultiplier = 1f;

	[SerializeField]
	private StatusEffectDebugUI _statusEffectDebugUIPrefab;

	private StatusEffectDebugUI _statsDebugUI;

	private TMP_Text _statsDebugText;

	public StatusEffectDebugUI StatsDebugUI
	{
		get
		{
			if (_statsDebugUI == null)
			{
				InitStatsValueDebug();
			}
			return _statsDebugUI;
		}
	}

	public TMP_Text StatsDebugText
	{
		get
		{
			if (_statsDebugText == null)
			{
				_statsDebugText = StatsDebugUI?.CreateTextDebug(network.GetHealth().ToString());
			}
			return _statsDebugText;
		}
	}

	private void Awake()
	{
		if (!NetworkGameManager.Instance.isServer)
		{
			object2D.parent = base.transform.parent;
		}
		foreach (SpriteRenderer item in eyeGlow)
		{
			item.enabled = false;
		}
		MPB = new MaterialPropertyBlock();
	}

	private void OnEnable()
	{
		bloodPool.transform.localEulerAngles = new Vector3(bloodPool.transform.localEulerAngles.x, UnityEngine.Random.Range(0, 360), bloodPool.transform.localEulerAngles.z);
	}

	private void Start()
	{
		Created();
		Init();
	}

	private void FixedUpdate()
	{
		bool isServer = NetworkGameManager.Instance.isServer;
		AnimatorStateInfo currentAnimatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
		float health = network.GetHealth();
		bool flag = currentAnimatorStateInfo.IsTag("Moving");
		bool flag2 = currentAnimatorStateInfo.IsTag("Hurt");
		Transform child = object2D.GetChild(0);
		Vector3 position = base.transform.position;
		Vector3 position2 = object2D.position;
		float normalizedTime = currentAnimatorStateInfo.normalizedTime;
		if (isServer)
		{
			if (position.y < -100f)
			{
				network.AddSubHealth(-5000f);
				position.y = 0f;
				base.transform.position = position;
			}
			AttackBarricade();
		}
		if (health > 0f)
		{
			if ((bool)headObj && !isSpriteInactive && timerHeadShake.isCompleted())
			{
				ctrHeadShake++;
				if (!isHurt)
				{
					if (ctrHeadShake > 25)
					{
						ctrHeadShake = 0;
						if (!isSpriteInactive)
						{
							timerHeadShake.StartDuration(UnityEngine.Random.Range(2, 6));
						}
					}
					else if (!isSpriteInactive)
					{
						timerHeadShake.StartDuration(0.02f);
					}
					if (!currentAnimatorStateInfo.IsTag("Down") && !currentAnimatorStateInfo.IsTag("Hovering") && !isDown && !isDead)
					{
						headObj.localEulerAngles = new Vector3(0f, 0f, UnityEngine.Random.Range(-15, 15));
					}
				}
			}
			if (timerStunt.isCompleted() && !isDead)
			{
				object2DSortingGroup.sortingLayerName = "Default";
				network.SetAnimation("Rise" + data.arrWeaponState[data.weaponState] + movement.angleAnim);
			}
			if (normalizedTime >= 1f)
			{
				if (currentAnimatorStateInfo.IsTag("Rise"))
				{
					WakeUpFromDown();
				}
				if (currentAnimatorStateInfo.IsTag("Landing2") && !isDead)
				{
					network.SetAnimation("Move" + data.arrWeaponState[data.weaponState] + movement.angleAnim);
					if (isServer)
					{
						attack.fov.enabled = true;
						child.DOKill();
						Vector3 localPosition = child.localPosition;
						localPosition.y = 0.742f;
						child.localPosition = localPosition;
						network.networkPhoton.isRisingHovering = true;
						myrigidbody.isKinematic = false;
						bodyCollider.isTrigger = false;
						enemyCollider.enabled = true;
						stoperCollider.enabled = true;
						aiPath.destination = aiPath.transform.position;
						AIEnable = true;
						SetEnableAI(value: true);
						SetAISpeed(data.GetSpeed());
						movement.MoveRandomPath();
						network.SetIsHovering(value: false);
						if ((bool)attack.targetChasing)
						{
							ChasingObject(attack.targetChasing, isSightPlayer: true);
							attack.SetAITarget(attack.targetChasing);
						}
						else
						{
							isMoveable = true;
							SetState(EnemyState.Patrol);
						}
					}
				}
			}
		}
		else if (network.IsNonActive() && !isDead && ((bool)GameManagerPhoton.Instance.HostLoadingGameComplete || LobbyManager.Instance != null))
		{
			whisper.SetActive(value: false);
			network.syncController.SetSync(_isSync: true);
			network.enemyController.Dead(1).Forget();
		}
		if (flag && !Mathf.Approximately(child.localPosition.y, 0.742f))
		{
			Vector3 localPosition2 = child.localPosition;
			localPosition2.y = 0.742f;
			child.localPosition = localPosition2;
		}
		if (network.GetIsJumping())
		{
			if (MathFunc.Distance(position2, position) > 3f)
			{
				object2D.position = position;
			}
			else
			{
				object2D.position = Vector3.Lerp(object2D.position, position, 0.25f);
			}
		}
		else if (!isDown && !isHurt && !isDead && (((myrigidbody.velocity != Vector3.zero || aiPath.desiredVelocity != Vector3.zero) | flag) || currentAnimatorStateInfo.IsTag("Attack")) && (!flag2 || (!hurtByThisNetwork | isServer)))
		{
			if (MathFunc.Distance(position2, position) > 5f)
			{
				object2D.position = position;
			}
			else if (!movement.isIdle)
			{
				if (isElite)
				{
					object2D.position = Vector3.Lerp(position2, position, 10f * Time.deltaTime);
				}
				else
				{
					object2D.position = Vector3.Lerp(position2, position, 3f * Time.deltaTime);
				}
			}
		}
		if (!flag2 && isHurt)
		{
			isHurt = false;
		}
		if (flag2 && isHurt && normalizedTime >= 1f)
		{
			if (isServer)
			{
				AnimationHurtEnd();
			}
			else
			{
				UniTaskUtil.DelayedCall(this, 0.1f, AnimationHurtEnd).Forget();
			}
		}
	}

	private void OnDestroy()
	{
		if (NetworkGameManager.Instance != null)
		{
			if (this != null && object2D != null)
			{
				object2D.DOKill();
				UnityEngine.Object.Destroy(object2D.gameObject);
			}
			GameManager.Instance.arrEnemyController.Remove(this);
		}
	}

	public void ChangeType(int type)
	{
		int index = -1;
		switch (type)
		{
		case 1:
			data.type = 0;
			data.weaponState = 0;
			break;
		case 2:
			data.type = 1;
			data.weaponState = 0;
			index = 7;
			break;
		default:
			data.type = type - 1;
			data.weaponState = 1;
			break;
		}
		enemyCharacterRenderController.ChangeSkin(SkinManager.Instance.GetEnemySkinByType(data.type, network.GetIDX()));
		enemyCharacterRenderController.ChangeWeaponSkin(SkinManager.Instance.GetEnemyWeaponSkin(index));
	}

	public void ChangeSkin(int type)
	{
		enemyCharacterRenderController.ChangeSkin(SkinManager.Instance.GetEnemySkin(type));
	}

	public void Created()
	{
		if (!isElite && NetworkGameManager.Instance.isServer)
		{
			network.SetType((byte)Mathf.FloorToInt(data.type + 1));
			if (data.type == 0)
			{
				int num = UnityEngine.Random.Range(0, 100);
				for (int i = 0; i < SkinManager.Instance.ListSkinZombieModifier.Count; i++)
				{
					if (num < SkinManager.Instance.ListSkinZombieModifier[i].GetPercentageShow())
					{
						network.networkPhoton.skinType = (byte)SkinManager.Instance.ListSkinZombieModifier[i].IdxSkin;
						break;
					}
					num = UnityEngine.Random.Range(0, 100);
				}
			}
		}
		targetObj.parent = base.transform.parent;
		if (NetworkGameManager.Instance.isServer)
		{
			network.SetIdxEnemy(GameManager.Instance.arrEnemyController.Count);
		}
		GameManager.Instance.arrEnemyController.Add(this);
	}

	public void Init(Transform targetPos = null)
	{
		if (NetworkGameManager.Instance.isServer && !isFakeDead)
		{
			enemyCollider.transform.localScale = new Vector3(1f, 1f, 1f);
			network.networkPhoton.isDisableCollider = false;
		}
		attack.isFoundPlayer = false;
		SetEnableAI(value: false);
		SetMultiplySpeed(1f);
		roomColliders.Clear();
		network.SetIsJumping(value: false);
		isJumping = false;
		lightCollider.lightCollider.enabled = true;
		isRoaming = false;
		animator.speed = 1f;
		colliderFOV.gameObject.SetActive(value: true);
		object2D.position = base.transform.position;
		isDead = false;
		isDeadAnimationPlaying = false;
		if (weaponSprite != null)
		{
			weaponSprite.enabled = true;
		}
		bloodPool.gameObject.SetActive(value: false);
		needInitSprite = false;
		network.SetInactiveEnemy(value: false);
		if (!isDown)
		{
			object2DSortingGroup.sortingLayerName = "Default";
			bodyCollider.enabled = true;
			if (NetworkGameManager.Instance.isServer)
			{
				myrigidbody.isKinematic = false;
			}
			else
			{
				myrigidbody.isKinematic = true;
			}
		}
		if (isDown)
		{
			bodyCollider.enabled = true;
			myrigidbody.isKinematic = false;
			UniTaskUtil.DelayedCall(this, 1f, () =>
			{
				myrigidbody.isKinematic = true;
				bodyCollider.enabled = false;
			}).Forget();
		}
		isSpriteInactive = true;
		HideSprite(isCreated: true);
		GameManager.Instance.totEnemySpawn++;
		object2D.transform.DOLocalRotate(new Vector3(0f, CameraGame.Instance.camRotate, 0f), 0.02f);
		if (GameManager.Instance.enemyStartChasing)
		{
			if (BGDatabase_GameConfig.GetEntityByKeyid(GameModes.Instance.modeGame).EnemyAlwaysChasing)
			{
				isTargetMoveEnable = false;
				attack.StartChasing(playerSighted: false, targetPos);
			}
			else
			{
				isTargetMoveEnable = true;
				attack.StartChasing(playerSighted: false, targetPos);
			}
		}
		else if (targetPos != null || (isElite && isAlwaysChasing))
		{
			attack.StartChasing(playerSighted: false, targetPos);
			isTargetMoveEnable = true;
		}
		else if (attack.targetChasing == null && GetCurrentStateHash() != AnimatorHashManager.HoveringHash && !network.GetIsHovering())
		{
			isMoveable = true;
			if (GetCurrentStateHash() != AnimatorHashManager.IdleHash)
			{
				SetState(EnemyState.Patrol);
			}
		}
		if (GetCurrentStateHash() == AnimatorHashManager.HoveringHash || network.GetIsHovering())
		{
			movement.SetAngle();
			hoveringType = UnityEngine.Random.Range(1, 4);
			network.SetAnimation("Hovering" + data.arrWeaponState[data.weaponState] + movement.angleAnim + "-" + hoveringType);
			Transform child = object2D.GetChild(0);
			child.position = new Vector3(child.position.x, UnityEngine.Random.Range(3f, 4f), child.position.z);
			bodyCollider.isTrigger = true;
			myrigidbody.isKinematic = true;
			child.DOLocalMoveY(child.position.y + 0.15f, UnityEngine.Random.Range(2f, 3f)).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine)
				.SetDelay(UnityEngine.Random.Range(0f, 1f));
		}
		attack.nextSpecialAttack1 = false;
		network.SetDoSpesialAttack(value: false);
		RVOController.enabled = true;
	}

	public void InitForClient()
	{
		if (!network.networkPhoton.isDisableCollider)
		{
			enemyCollider.transform.localScale = new Vector3(1f, 1f, 1f);
		}
		SetMultiplySpeed(1f);
		attack.isFoundPlayer = false;
		lightCollider.lightCollider.enabled = true;
		if (!isDown)
		{
			object2DSortingGroup.sortingLayerName = "Default";
		}
		object2D.position = base.transform.position;
		if (!NetworkGameManager.Instance.isServer)
		{
			network.networkTransform.enabled = true;
		}
		if (weaponSprite != null)
		{
			weaponSprite.enabled = true;
		}
		isDead = false;
		isDeadAnimationPlaying = false;
		bloodPool.gameObject.SetActive(value: false);
		needInitSprite = true;
		colliderFOV.gameObject.SetActive(value: true);
		isMoveable = true;
		SetState(EnemyState.Patrol);
	}

	public void VisibleSprite()
	{
		if (!(network.GetHealth() > 0f) || network.IsNonActive())
		{
			return;
		}
		isDeadAnimationPlaying = false;
		if (isSpriteInactive)
		{
			object2D.position = base.transform.position;
		}
		if (!isDown)
		{
			object2DSortingGroup.sortingLayerName = "Default";
		}
		isSpriteInactive = false;
		needInitSprite = false;
		foreach (SpriteRenderer allSpritePart in allSpriteParts)
		{
			allSpritePart.DOKill();
			allSpritePart.color = new Color(allSpritePart.color.r, allSpritePart.color.r, allSpritePart.color.b, 1f);
			allSpritePart.enabled = true;
		}
		foreach (SpriteResolver item in allSpriteResolver)
		{
			item.enabled = true;
		}
		shadow.DOKill();
		shadow.DOFade(1f, 0f);
		if (!network.GetIsHovering() && !isDown)
		{
			if (NetworkGameManager.Instance.isServer)
			{
				myrigidbody.isKinematic = false;
			}
			bodyCollider.enabled = true;
			enemyCollider.enabled = true;
			stoperCollider.enabled = true;
		}
		if (isDown)
		{
			enemyCollider.enabled = true;
		}
		bloodPool.gameObject.SetActive(value: false);
		whisper.SetActive(value: true);
		if (network.GetIsHovering() || isDown)
		{
			foreach (SpriteRenderer item2 in eyeGlow)
			{
				item2.enabled = false;
			}
		}
		if (!timerHeadShake.isRunning && !isElite)
		{
			timerHeadShake.StartDuration(UnityEngine.Random.Range(2, 6));
		}
	}

	public async UniTask InitSprite()
	{
		CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
		if (!isDown)
		{
			object2DSortingGroup.sortingLayerName = "Default";
		}
		object2D.position = base.transform.position;
		isSpriteInactive = false;
		needInitSprite = false;
		await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationTokenOnDestroy);
		if (isSpriteInactive || needInitSprite)
		{
			return;
		}
		foreach (SpriteRenderer allSpritePart in allSpriteParts)
		{
			allSpritePart.DOKill();
			allSpritePart.color = new Color(allSpritePart.color.r, allSpritePart.color.r, allSpritePart.color.b, 1f);
			allSpritePart.enabled = true;
		}
		foreach (SpriteResolver item in allSpriteResolver)
		{
			item.enabled = true;
		}
		shadow.DOKill();
		shadow.DOFade(1f, 0f);
		if (!network.GetIsHovering())
		{
			if (NetworkGameManager.Instance.isServer)
			{
				myrigidbody.isKinematic = false;
			}
			bodyCollider.enabled = true;
			enemyCollider.enabled = true;
			stoperCollider.enabled = true;
		}
		bloodPool.gameObject.SetActive(value: false);
		object2D.position = base.transform.position;
	}

	public void HideSprite(bool isCreated = false, bool isHideBlood = true)
	{
		if (HeadSprite != null)
		{
			HeadSprite.enabled = false;
			HeadSpriteShadow.enabled = false;
		}
		foreach (SpriteRenderer allSpritePart in allSpriteParts)
		{
			allSpritePart.DOKill();
			allSpritePart.color = new Color(allSpritePart.color.r, allSpritePart.color.r, allSpritePart.color.b, 0f);
			allSpritePart.enabled = false;
		}
		foreach (SpriteResolver item in allSpriteResolver)
		{
			item.enabled = false;
		}
		foreach (SpriteRenderer item2 in eyeGlow)
		{
			item2.enabled = false;
		}
		shadow.DOKill();
		shadow.color = new Color(shadow.color.r, shadow.color.r, shadow.color.b, 0f);
		if ((network.GetHealth() <= 0f || isDead || network.IsNonActive()) && !isCreated)
		{
			bodyCollider.enabled = false;
			myrigidbody.isKinematic = true;
		}
		enemyCollider.enabled = false;
		stoperCollider.enabled = false;
		if (isHideBlood)
		{
			bloodPool.gameObject.SetActive(value: false);
		}
		object2D.position = base.transform.position;
		whisper.SetActive(value: false);
		isSpriteInactive = true;
	}

	public void Hide2DSprite()
	{
		foreach (SpriteRenderer allSpritePart in allSpriteParts)
		{
			allSpritePart.DOKill();
			allSpritePart.color = new Color(allSpritePart.color.r, allSpritePart.color.r, allSpritePart.color.b, 0f);
			allSpritePart.enabled = false;
		}
		foreach (SpriteResolver item in allSpriteResolver)
		{
			item.enabled = false;
		}
		foreach (SpriteRenderer item2 in eyeGlow)
		{
			item2.enabled = false;
		}
		shadow.DOKill();
		shadow.color = new Color(shadow.color.r, shadow.color.r, shadow.color.b, 0f);
	}

	public void Hurt(float damage, float stuntTime, bool execShakingCam, byte fromPlayer, short weaponType = 0, bool isGrenade = false, bool isHeadOff = false, bool isWithDeadAnimation = true, bool isActivateSpecialDead = true, bool isdamagingEnemy = true)
	{
		if (!(network.GetHealth() > 0f))
		{
			return;
		}
		bool flag = false;
		PlayerController player = NetworkGameManager.Instance.GetPlayer(fromPlayer);
		if (NetworkGameManager.Instance.isServer && (GetCurrentStateHash() == AnimatorHashManager.ChasingHash || GetCurrentStateHash() == AnimatorHashManager.AttackingHash) && !isElite && aiTarget.target != player.playerCollider.transform)
		{
			attack.targetChasing = player.targetedPoint;
			attack.prevTargetChasing = attack.targetChasing;
			isPlayerSighted = true;
			attack.SetAITarget(attack.targetChasing);
		}
		if (network.GetHealth() - damage <= 0f && !isDead && player.network.isLocalPlayer)
		{
			if (!NetworkGameManager.Instance.isServer)
			{
				network.networkTransform.enabled = false;
			}
			if (weaponType >= 0)
			{
				ObjectImpactPool objectImpactPool = ImpactSpawner.Instance.Get();
				if (hitPos.Count > 0)
				{
					objectImpactPool.transform.position = hitPos[UnityEngine.Random.Range(0, hitPos.Count)].position;
				}
				objectImpactPool.transform.position = new Vector3(animator.transform.position.x, objectImpactPool.transform.position.y, animator.transform.position.z);
				objectImpactPool.transform.localEulerAngles = new Vector3(0f, player.weaponController.muzzle.transform.localEulerAngles.y, 0f);
				objectImpactPool.transform.parent = object2D.transform;
				if (weaponType == 1)
				{
					objectImpactPool.typeImpact = ObjectImpactPool.ImpactType.Blood;
				}
				else
				{
					objectImpactPool.typeImpact = ObjectImpactPool.ImpactType.BloodOmni;
				}
				objectImpactPool.initType();
			}
			byte deadType = 0;
			if (weaponType == 1)
			{
				deadType = (byte)player.weaponController.deadTypeWeapon1;
			}
			if (isGrenade)
			{
				deadType = 2;
			}
			if (isHeadOff)
			{
				deadType = 3;
			}
			meleeHitAnim.gameObject.SetActive(value: true);
			meleeHitAnim.Play("ChargeMeleeHit");
			Dead(deadType, isWithDeadAnimation, isActivateSpecialDead).Forget();
			player.network.KillEnemy(middlePos.position, _speedMultiplier < 1f, isEliteScore);
			flag = true;
			GameStatistic.AddKillEnemy(this, (byte)weaponType);
		}
		if (player.network.isLocalPlayer)
		{
			hurtByThisNetwork = true;
		}
		if (player.network.isLocalPlayer)
		{
			FeedbackEnemyController feedbackEnemyController = GameManager.Instance.GetEnemy(network.GetIDX()).feedback;
			byte animationType = GetAnimationType(weaponType);
			if (!flag && !network.GetIsJumping() && !network.networkPhoton.isMoveToJump)
			{
				bool isForceKnockback = false;
				if (player.PlayerMultiplyStatsData.GetDashAttackDamage() > 0f && player.isDashing)
				{
					animationType = 2;
					isForceKnockback = true;
				}
				if (weaponType >= 0)
				{
					feedbackEnemyController.Hurt(stuntTime, animationType, player.weaponController.muzzle.transform.localEulerAngles.y, player, isShowBlood: true, isForceKnockback).Forget();
				}
				else
				{
					feedbackEnemyController.Hurt(stuntTime, 2, player.weaponController.muzzle.transform.localEulerAngles.y, player, isShowBlood: false, isForceKnockback).Forget();
				}
			}
			if (isdamagingEnemy)
			{
				player.network.HitEnemy(network.GetIDX(), damage, animationType);
			}
		}
		UpdateStatsValueDebug();
		byte GetAnimationType(int num)
		{
			byte result = 0;
			switch (num)
			{
			case 0:
				result = (byte)((!BGDatabase_Weapon.GetEntityByKeyid(player.weaponController.idWeaponMelee).KnockbackAnimTrigger) ? 3 : 2);
				break;
			case 1:
				if (player.weaponController.rangeWeaponType == RangeWeaponType.Shotgun)
				{
					result = 2;
				}
				break;
			case 2:
				result = 2;
				break;
			case 3:
				result = 3;
				break;
			}
			return result;
		}
	}

	public void ChasingPlayer(PlayerController player)
	{
		if (network.GetHealth() > 0f && GetCurrentStateHash() != AnimatorHashManager.ChasingHash && GetCurrentStateHash() != AnimatorHashManager.AlertChasingHash && (GetCurrentStateHash() != AnimatorHashManager.AttackingHash || !isElite))
		{
			SetState(EnemyState.AlertChasing);
			isPlayerSighted = true;
			if (isPlayerSighted)
			{
				player.targetedPoint.position = new Vector3(player.targetedPoint.position.x, base.transform.position.y, player.targetedPoint.position.z);
				attack.targetChasing = player.targetedPoint;
			}
			if (!isAttacking && !isHurt)
			{
				network.SetAnimation("Idle" + data.arrWeaponState[data.weaponState] + movement.angleAnim);
			}
			attack.timerIdleChasing.StopDuration();
			attack.timerRandomIdleChasing.StopDuration();
			attack.timerDelayChasing.StartDuration(0.2f);
			movement.SetCurrentMoveSpeed(0f);
			isOnDestinationTarget = false;
		}
	}

	public void ChasingObject(Transform objTransform, bool isSightPlayer = false)
	{
		if (network.GetHealth() > 0f && GetCurrentStateHash() != AnimatorHashManager.ChasingHash && GetCurrentStateHash() != AnimatorHashManager.AlertChasingHash)
		{
			SetState(EnemyState.AlertChasing);
			attack.timerIdleChasing.StopDuration();
			attack.timerRandomIdleChasing.StopDuration();
			isPlayerSighted = isSightPlayer;
			attack.targetChasing = objTransform;
			if (!isAttacking && !isHurt)
			{
				network.SetAnimation("Idle" + data.arrWeaponState[data.weaponState] + movement.angleAnim);
			}
			attack.timerDelayChasing.StartDuration(0.2f);
			movement.SetCurrentMoveSpeed(0f);
			isOnDestinationTarget = false;
			isTargetMoveEnable = true;
		}
	}

	public async UniTask Dead(byte deadType, bool isWithDeadAnimation = true, bool isActivateSpecialDead = true)
	{
		CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
		if (!isDeadAnimationPlaying)
		{
			if (deadType == 0)
			{
				deadType = 1;
			}
			if (deadType == 3)
			{
				if (HeadSprite != null && HeadSpriteSource != null)
				{
					if (animator.transform.localScale.x < 0f)
					{
						HeadSprite.transform.localScale = new Vector3(0f - HeadSprite.transform.localScale.x, HeadSprite.transform.localScale.y, HeadSprite.transform.localScale.z);
					}
					HeadSprite.sprite = HeadSpriteSource.sprite;
					HeadSprite.transform.position = HeadSpriteSource.transform.position;
					HeadSprite.enabled = true;
					HeadSpriteShadow.enabled = true;
				}
				float num = UnityEngine.Random.Range(-0.5f, 0.5f);
				float num2 = UnityEngine.Random.Range(-0.5f, 0.5f);
				if ((bool)HeadSprite)
				{
					HeadSprite.transform.DOJump(new Vector3(base.transform.position.x + num, base.transform.position.y, base.transform.position.z + num2), 0.5f, 1, 0.35f);
					HeadSpriteShadow.transform.DOMove(new Vector3(base.transform.position.x + num, base.transform.position.y, base.transform.position.z + num2), 0.35f);
					HeadSprite.transform.DOMove(new Vector3(base.transform.position.x + num * 1.5f, base.transform.position.y, base.transform.position.z + num2 * 1.5f), 0.5f).SetDelay(0.35f);
					HeadSprite.transform.DOLocalRotate(new Vector3(0f, CameraGame.Instance.camRotate, 0f), 0f, RotateMode.FastBeyond360);
					HeadSprite.transform.DOLocalRotate(new Vector3(0f, CameraGame.Instance.camRotate, UnityEngine.Random.Range(-120, 120)), 0.7f, RotateMode.FastBeyond360);
					HeadSpriteShadow.transform.DOMove(new Vector3(base.transform.position.x + num * 1.5f, base.transform.position.y, base.transform.position.z + num2 * 1.5f), 0.35f).SetDelay(0.35f);
				}
			}
			if (isWithDeadAnimation)
			{
				if (isDown || attack.DistanceExplosion > 0f)
				{
					network.SetAnimation("DeadBack" + movement.angleAnim);
				}
				else if (deadType == 3)
				{
					network.SetAnimation("DeadFront" + movement.angleAnim + "-1");
				}
				else
				{
					network.SetAnimation("DeadFront" + movement.angleAnim + "-" + deadType);
				}
			}
			if (weaponSprite != null)
			{
				weaponSprite.enabled = false;
			}
			whisper.SetActive(value: false);
			if (!network.IsNonActive() && !isSpriteInactive)
			{
				AudioManager.PlaySFXTransform("enemy0-explode", middlePos, isLocalPlayerTrigger: false);
				if (isElite && data.type == 100)
				{
					AudioManager.PlaySFXTransform("hairmaiden-explode", middlePos, isLocalPlayerTrigger: false);
				}
				if (MathFunc.Distance(middlePos.position, NetworkGameManager.Instance.ownPlayer.transform.position) < 12f)
				{
					CameraGame.Instance.CameraShake(0.2f, 0.4f);
				}
			}
			attack.meleeCollider.gameObject.SetActive(value: false);
			colliderFOV.gameObject.SetActive(value: false);
			isHurt = false;
			shadow.color = new Color(shadow.color.r, shadow.color.r, shadow.color.b, 0f);
			isDead = true;
			if (NetworkGameManager.Instance.isServer)
			{
				UniTaskUtil.DelayedCall(this, 2f, () =>
				{
					GameManagerPhoton.Instance.RPCSetEnemyDead(network.GetIDX());
				}).Forget();
				network.networkPhoton.isDisableCollider = false;
			}
			isDeadAnimationPlaying = true;
			foreach (SpriteRenderer item in eyeGlow)
			{
				item.enabled = false;
			}
			int num3 = 0;
			int num4 = 0;
			if (NetworkGameManager.Instance.isServer)
			{
				foreach (EnemyController item2 in GameManager.Instance.arrEnemyController)
				{
					if (!item2.network.IsNonActive() && item2.network.GetHealth() > 0f && !item2.isDead)
					{
						if (item2.network.GetIsHorde())
						{
							num4++;
						}
						if (item2.GetCurrentStateHash() == AnimatorHashManager.ChasingHash || item2.GetCurrentStateHash() == AnimatorHashManager.AttackingHash || item2.GetCurrentStateHash() == AnimatorHashManager.AlertChasingHash)
						{
							num3++;
						}
					}
				}
				if (num4 <= 1 && GameManager.Instance.isHordeMode && network.GetIsHorde() && !GameManager.Instance.waveManager.isSpawningHorde && !GameManager.Instance.gameManagerPhoton.objectiveComplete && !GameManager.Instance.isInfiniteHordeMode)
				{
					GameManager.Instance.waveManager.InitHorde();
					GameManager.Instance.isHordeMode = false;
					GameManager.Instance.gameManagerPhoton.RpcExecDisableHorde();
					network.SetIsHorde(value: false);
				}
			}
			if (!isSpriteInactive)
			{
				bloodPool.DOKill();
				bloodPool.color = new Color(bloodPool.color.r, bloodPool.color.g, bloodPool.color.b, 0f);
				bloodPool.transform.DOKill();
				bloodPool.transform.DOScale(0f, 0f);
				if (isWithDeadAnimation)
				{
					bloodPool.transform.DOScale(5f, 5f);
				}
				else
				{
					bloodPool.transform.DOScale(5f, 0.2f);
				}
				bloodPool.gameObject.SetActive(value: true);
				bloodPool.GetComponent<Animator>().Play("BloodPool" + UnityEngine.Random.Range(1, 3));
			}
			isFakeDead = false;
			isDown = false;
			RVOController.enabled = false;
			myrigidbody.isKinematic = true;
			bodyCollider.enabled = false;
			enemyCollider.enabled = false;
			stoperCollider.enabled = false;
			lightCollider.lightCollider.enabled = false;
			attack.DisableAllTimer();
			attack.SetAITargettoNull();
			attack.targetChasing = null;
			SetEnableAI(value: false);
			SetAISpeed(0f);
			attack.targetChasing = null;
			isAttacking = false;
			Transform child = object2D.GetChild(0);
			child.DOKill();
			child.localPosition = new Vector3(child.localPosition.x, 0.742f, child.localPosition.z);
			network.SetIsHovering(value: false);
			if (movement.angleAnim == 0)
			{
				movement.angleAnim = 45;
			}
			if (!isSpriteInactive)
			{
				switch (deadType)
				{
				case 1:
				{
					EnemyPartPool enemyPartPool2 = EnemyPartSpawner.Instance.Get();
					enemyPartPool2.transform.position = headTransform.position;
					enemyPartPool2.initType(0);
					break;
				}
				case 2:
				{
					EnemyPartPool enemyPartPool = EnemyPartSpawner.Instance.Get();
					enemyPartPool.transform.position = bodyTransform.position;
					enemyPartPool.initType(1);
					break;
				}
				}
			}
			SetState(EnemyState.Dead);
		}
		if (isActivateSpecialDead && EventSpecialDead.GetPersistentEventCount() > 0)
		{
			EventSpecialDead.Invoke();
		}
		else if (isWithDeadAnimation && !isSpriteInactive)
		{
			Fading().Forget();
		}
		else
		{
			Fading(0f).Forget();
		}
		await UniTask.Delay(TimeSpan.FromSeconds(0.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationTokenOnDestroy);
	}

	public async UniTask Fading(float delay = 4f)
	{
		CancellationToken token = this.GetCancellationTokenOnDestroy();
		await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		bloodPool.color = new Color(bloodPool.color.r, bloodPool.color.g, bloodPool.color.b, 1f);
		await UniTask.Delay(TimeSpan.FromSeconds(0.5), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		object2DSortingGroup.sortingLayerName = "Ground";
		await UniTask.Delay(TimeSpan.FromSeconds(1.5), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		isDeadAnimationPlaying = false;
		await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		if (HeadSprite != null && HeadSprite.enabled)
		{
			HeadSprite.DOFade(0f, 2f);
			HeadSpriteShadow.DOFade(0f, 2f);
		}
		if (network.GetHealth() <= 0f)
		{
			foreach (SpriteRenderer item in eyeGlow)
			{
				item.enabled = false;
			}
			foreach (SpriteRenderer allSpritePart in allSpriteParts)
			{
				allSpritePart.DOKill();
				allSpritePart.DOFade(0f, 2f).OnComplete(() =>
				{
					foreach (SpriteResolver item2 in allSpriteResolver)
					{
						item2.enabled = false;
					}
				});
			}
			shadow.DOKill();
			shadow.DOFade(0f, 2f);
		}
		await UniTask.Delay(TimeSpan.FromSeconds(2.0), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		if (network.GetHealth() <= 0f)
		{
			foreach (SpriteRenderer allSpritePart2 in allSpriteParts)
			{
				allSpritePart2.enabled = false;
			}
			foreach (SpriteResolver item3 in allSpriteResolver)
			{
				item3.enabled = false;
			}
		}
		await UniTask.Delay(TimeSpan.FromSeconds(delay), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		if (network.GetHealth() <= 0f)
		{
			bloodPool.DOKill();
			bloodPool.DOFade(0f, 2f);
		}
		await UniTask.Delay(TimeSpan.FromSeconds(2.0), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		if (network.GetHealth() <= 0f)
		{
			bloodPool.transform.DOKill();
			bloodPool.transform.DOScale(Vector3.zero, 0.1f);
		}
		await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		if (network.GetHealth() <= 0f)
		{
			bloodPool.gameObject.SetActive(value: false);
			isSpriteInactive = true;
		}
		await UniTask.Delay(TimeSpan.FromSeconds(2.0), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		if (network.GetHealth() <= 0f)
		{
			network.SetInactiveEnemy(value: true);
		}
	}

	public int GetCurrentStateHash()
	{
		return animatorState.GetCurrentAnimatorStateInfo(0).shortNameHash;
	}

	public void SetState(EnemyState _state)
	{
		if ((bool)animatorState)
		{
			animatorState.Play(_state.ToString());
		}
		state = _state;
	}

	public void AttackBarricade()
	{
		if (timerAttackBarricade.isCompleted() && network.GetHealth() > 0f && barricadeCollider != null && barricadeCollider.barricade.Hp > 0)
		{
			isAttacking = true;
			attack.timerDelayAggro1.StartDuration(1f);
			network.SetAnimation("Attack" + data.arrWeaponState[data.weaponState] + movement.angleAnim);
			SetAISpeed(0f);
			DelayBarricadeHit().Forget();
			SetState(EnemyState.Attacking);
		}
	}

	public async UniTask DelayBarricadeHit()
	{
		CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
		await UniTask.Delay(TimeSpan.FromSeconds(0.4000000059604645), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationTokenOnDestroy);
		if (barricadeCollider.barricade.Hp > 0)
		{
			GameManagerPhoton.Instance.RPCBarricadeAttacked((byte)barricadeCollider.barricade.UniqueID);
			timerAttackBarricade.StartDuration(UnityEngine.Random.Range(1, 2));
		}
	}

	public async UniTask StopAttackBarricade()
	{
		CancellationToken token = this.GetCancellationTokenOnDestroy();
		if (NetworkGameManager.Instance.isServer)
		{
			timerAttackBarricade.PauseDuration();
			await UniTask.Delay(TimeSpan.FromSeconds(timerAttackBarricade.interval + 1f), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
			network.networkPhoton.isMoveToJump = false;
			SetEnableAI(value: false);
			enemyCollider.enabled = false;
			stoperCollider.enabled = false;
			network.SetIsJumping(value: true);
			network.SetAnimation("Jump" + data.arrWeaponState[data.weaponState] + movement.angleAnim);
			myrigidbody.isKinematic = true;
			Vector3 normalized = (barricadeCollider.ObstaclePath.transform.position - base.transform.position).normalized;
			myrigidbody.DOKill();
			base.transform.DOKill();
			base.transform.DOJump(base.transform.position + new Vector3(normalized.x, 0f, normalized.z).normalized * 2.5f, 1.2f, 1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
			{
				movement.StartMove().Forget();
			});
			bodyCollider.enabled = false;
			barricadeCollider = null;
		}
		await UniTask.Delay(TimeSpan.FromSeconds(0.0), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
	}

	public void Rise()
	{
		AudioManager.PlaySFXTransform("enemy0-body-drop", base.transform, isLocalPlayerTrigger: false);
		if (NetworkGameManager.Instance.isServer)
		{
			network.SetAnimation("Land" + data.arrWeaponState[data.weaponState] + movement.angleAnim + "-2");
		}
	}

	public void SetAISpeed(float newSpeed)
	{
		aiPath.maxSpeed = newSpeed * _speedMultiplier;
	}

	public void WakeUpFromDown()
	{
		attack.fov.enabled = true;
		if (NetworkGameManager.Instance.isServer)
		{
			myrigidbody.isKinematic = false;
		}
		bodyCollider.enabled = true;
		enemyCollider.enabled = true;
		stoperCollider.enabled = true;
		isHurt = false;
		isDown = false;
		isAttacking = false;
		network.SetAnimation("Move" + data.arrWeaponState[data.weaponState] + movement.angleAnim);
		shadow.color = new Color(shadow.color.r, shadow.color.r, shadow.color.b, 1f);
		foreach (SpriteRenderer item in eyeGlow)
		{
			item.enabled = true;
		}
		if (NetworkGameManager.Instance.isServer)
		{
			attack.timerTriggerAttack.StartDuration(0.2f);
			SetAISpeed(data.aggroSpeed);
			AIEnable = true;
			SetEnableAI(value: true);
			if (!isFakeDead)
			{
				attack.timerDelayAggro2.StartDuration(2f);
			}
			else if (attack.targetChasing != null)
			{
				ChasingObject(attack.targetChasing, isSightPlayer: true);
				attack.SetAITarget(attack.targetChasing);
			}
		}
		if (isFakeDead)
		{
			object2DSortingGroup.sortingLayerName = "Default";
			isFakeDead = false;
			isMoveable = true;
		}
	}

	public void AnimationHurtEnd()
	{
		hurtByThisNetwork = false;
		isHurt = false;
		animator.speed = 1f;
		attack.timerDelayAggro2.StartDuration(UnityEngine.Random.Range(4f, 6f));
		attack.timerDelayAggro1.StartDuration(UnityEngine.Random.Range(1f, 2.5f));
		if (isAttacking)
		{
			isAttacking = false;
			network.SetAnimation("Move" + data.arrWeaponState[data.weaponState] + movement.angleAnim);
			if (NetworkGameManager.Instance.isServer)
			{
				AIEnable = true;
				SetEnableAI(value: true);
				attack.timerTriggerAttack.StartDuration(1f);
			}
		}
		if (NetworkGameManager.Instance.isServer)
		{
			AIEnable = true;
			SetEnableAI(value: true);
			attack.timerTriggerAttack.ResumeDuration();
		}
		if (isDead || isAttacking)
		{
			return;
		}
		if (NetworkGameManager.Instance.isServer)
		{
			if (GetCurrentStateHash() == AnimatorHashManager.ChasingHash || aiTarget.target != null || attack.targetChasing != null)
			{
				network.SetAnimation("Idle" + data.arrWeaponState[data.weaponState] + movement.angleAnim);
				return;
			}
			GetCurrentStateHash();
			_ = AnimatorHashManager.AlertChasingHash;
		}
		else if (network.networkPhoton.animationState == 0 || network.networkPhoton.animationState == 2)
		{
			network.SetAnimation("Idle" + data.arrWeaponState[data.weaponState] + movement.angleAnim);
		}
		else if (network.networkPhoton.animationState == 1)
		{
			network.SetAnimation("Move" + data.arrWeaponState[data.weaponState] + movement.angleAnim);
		}
	}

	public void SetEnableAI(bool value)
	{
		if (value && network.networkPhoton.isMoveToJump)
		{
			value = false;
		}
		if (AIEnable || !value)
		{
			aiPath.enabled = value;
		}
	}

	public void ExecuteDead()
	{
		Dead(1).Forget();
	}

	public void CheckEnemyAggroNetwork(PlayerController playerController)
	{
		if (NetworkGameManager.Instance.isServer)
		{
			CheckEnemyAgro(playerController);
		}
		else
		{
			network.networkPhoton.RpcCheckEnemyAggro(playerController.network.GetIDX());
		}
	}

	public void CheckEnemyAgro(PlayerController playerController)
	{
		if (!NetworkGameManager.Instance.isServer)
		{
			return;
		}
		Transform origin = playerController.origin;
		float num = MathFunc.Distance(transform.position, playerController.origin.position);
		if (network.networkPhoton.isDeaf || !(num < 13f) || GetCurrentStateHash() == AnimatorHashManager.AttackingHash || !(network.GetHealth() > 0f) || GetCurrentStateHash() == AnimatorHashManager.ChasingHash || GetCurrentStateHash() == AnimatorHashManager.AlertChasingHash || isAttacking)
		{
			return;
		}
		if (network.GetIsHovering())
		{
			if (!network.networkPhoton.isFallingHovering)
			{
				attack.isChasingSound = true;
				attack.targetChasing = origin;
				network.networkPhoton.isFallingHovering = true;
			}
		}
		else
		{
			if (attack.fov.isDisable)
			{
				return;
			}
			Vector3 normalized = (middlePos.position - playerController.weaponPos.position).normalized;
			bool flag = false;
			foreach (string roomCollider in roomColliders)
			{
				if (roomCollider == playerController.RoomName)
				{
					flag = true;
					break;
				}
			}
			if (!isDown && (!Physics.Raycast(playerController.weaponPos.position, normalized, num, GameManager.Instance.wallFloorCollider) | flag))
			{
				network.networkPhoton.RpcEnemyAggro();
				ChasingObject(origin);
			}
			if (isFakeDead && attack.targetChasing == null)
			{
				attack.targetChasing = origin;
				timerStunt.StartDuration(UnityEngine.Random.Range(0.5f, 1.5f));
			}
		}
	}

	public void SetMultiplySpeed(float multiplySpeed)
	{
		_speedMultiplier = multiplySpeed;
	}

	private void InitStatsValueDebug()
	{
		if (GameModes.Instance.isDebug && !(_statusEffectDebugUIPrefab == null))
		{
			if (_statsDebugUI == null)
			{
				_statsDebugUI = UnityEngine.Object.Instantiate(_statusEffectDebugUIPrefab, enemyCharacterRenderController.transform);
			}
			_statsDebugUI.gameObject.SetActive(GameDebug.Instance.ShowEnemyDebug);
			UpdateStatsValueDebug();
		}
	}

	private void UpdateStatsValueDebug()
	{
		if (GameModes.Instance.isDebug && StatsDebugText != null)
		{
			StatsDebugText.text = $"Health {network.GetHealth()}";
		}
	}
}
