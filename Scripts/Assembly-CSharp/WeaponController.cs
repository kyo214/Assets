using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Toked;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class WeaponController : MonoBehaviour
{
	public PlayerData playerData;

	public PlayerController playerController;

	public GameObject muzzle;

	public float attackInterval;

	public float meleeInterval;

	public float radiusSpread;

	public int shotsPerAttack;

	public XTimer attackTimer;

	public XTimer halfChargeTimer;

	public XTimer chargeTimer;

	public GameObject meleeObject;

	public GameObject rangeObject;

	public SpriteLibrary meleeSpriteLib;

	public SpriteLibrary rangeSpriteLib;

	public SpriteRenderer meleeSprite;

	public int idxSkinWeapon0 = -1;

	public int idxSkinWeapon1 = -1;

	public int idWeaponMelee = -1;

	public int idWeaponRange = -1;

	public int idBaseWeaponRange = -1;

	public int prevIdWeaponMelee = -1;

	public int prevIdWeaponRange = -1;

	public int prevAmmoWeaponRange = -1;

	public int idxWeaponMelee;

	public int idxWeaponRange = 1;

	public int dirAimOtherPlayer;

	public float dmgWeapon0;

	public float dmgWeaponHalfCharge0;

	public float dmgWeaponFullCharge0;

	public float dmgWeapon1;

	public float stuntWeapon0;

	public float stuntWeapon1;

	public float speedAim;

	public int needStaminaWeapon0;

	public int needStaminaWeapon1;

	public float shakeAmplitudeWeapon0;

	public float shakeAmplitudeWeapon1;

	public float shakeDurWeapon0;

	public float shakeDurWeapon1;

	public float timerRelaseAttack;

	public int deadTypeWeapon0;

	public int deadTypeWeapon1;

	public string weaponStyle;

	public int weaponSelect = 1;

	public float accuracy;

	public float attackReleaseAnimSpeed;

	public float minRangeAccuracy;

	public float maxRangeAccuracy;

	public float timeAccuracy = 1f;

	public bool isHalfMeleeCharging;

	public bool isMeleeCharging;

	public bool isOneHitKnockback;

	public bool isDisableHalfCharge;

	public RangeWeaponType rangeWeaponType;

	private static readonly int IsShootingAnim = Animator.StringToHash("isShooting");

	private static readonly int IsMeleeAnim = Animator.StringToHash("isMelee");

	private static readonly int IsReloadAnim = Animator.StringToHash("isReloading");

	private static readonly int IsThrowingAnim = Animator.StringToHash("isThrowing");

	public LayerMask obstacleMask;

	public LayerMask obstacleMaskExceptWindow;

	[SerializeField]
	private LayerMask _layerBulletCollider;

	[SerializeField]
	private LayerMask _layerBulletColliderFriendlyFire;

	public XTimer timerReload;

	public XTimer reloadStateTimer;

	public XTimer timerResetCombo;

	public XTimer ReleaseAttackTimer;

	public int MaxEnemyHit = 3;

	public int MaxEnemyHitHalfCharge = 3;

	public int MaxEnemyHitFullCharge = 3;

	public int idxAttackCombo;

	public int maxAttackCombo = 2;

	public bool isResetCombo;

	public XTimer timerDelayShoot;

	public int ctrBulletShoot;

	public XTimer timerDelayAttackEnd;

	public GLauncherBulletControl GLauncherControl;

	private float _percentageMagSize;

	public Tweener MeleeTween;

	private Material _meleeMat;

	private void Start()
	{
		meleeSprite = meleeSpriteLib.GetComponent<SpriteRenderer>();
		if (meleeSprite != null)
		{
			_meleeMat = meleeSprite.material;
		}
		idxAttackCombo = -1;
	}

	private void FixedUpdate()
	{
		if (halfChargeTimer.isCompleted() && playerController.isAttacking)
		{
			_meleeMat.DOKill();
			_meleeMat.DOColor(new Color(0f, 0f, 0f), "_Tint", 0f);
			_meleeMat.DOFloat(3f, "_Brightness", 0f);
			_meleeMat.DOFloat(0.05f, "_Brightness", 0.2f);
			playerController.animUpperChar.transform.DOLocalRotate(new Vector3(0f, 0f, 0.5f), 0.05f).SetLoops(-1, LoopType.Yoyo);
			isHalfMeleeCharging = true;
		}
		if (chargeTimer.isCompleted() && playerController.isAttacking)
		{
			_meleeMat.DOKill();
			_meleeMat.DOColor(new Color(0f, 0f, 0f), "_Tint", 0f);
			_meleeMat.DOFloat(3f, "_Brightness", 0f);
			_meleeMat.DOFloat(0.1f, "_Brightness", 0.2f);
			_meleeMat.DOFloat(0.4f, "_Brightness", 0.2f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuint)
				.SetDelay(0.2f);
			playerController.animUpperChar.transform.DOKill();
			playerController.animUpperChar.transform.DOLocalRotate(new Vector3(0f, 0f, 2.5f), 0.05f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
			isMeleeCharging = true;
		}
		if (attackTimer.isCompleted() && idWeaponRange > 0)
		{
			if (BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).isPumpAction)
			{
				if (playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("Shoot"))
				{
					playerController.shell.EjectShell(rangeObject.transform.position, new Vector3(0f, playerController.origin.localEulerAngles.y - 45f, 0f), rangeWeaponType);
					playerController.animUpperChar.Play("KokangShotgun" + playerController.angleRot, -1, 0f);
				}
			}
			else if (!playerController.isAttackMelee)
			{
				playerController.fsmUpperBody.SetBool(IsShootingAnim, value: false);
				playerController.isShooting = false;
				playerController.isAttacking = false;
			}
			bool flag = playerController.isAttackBtnPressed;
			if (BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).Type == "Rifle" && ctrBulletShoot > 0)
			{
				flag = true;
			}
			if (flag && playerController.isRangeActive && playerController.isAiming && !playerController.fsmUpperBody.GetBool(IsReloadAnim))
			{
				if (BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).IsAutoFire)
				{
					float y = Quaternion.LookRotation(playerController.angleInput - playerController.weaponPos.position, Vector3.up).eulerAngles.y;
					if (y < 0f)
					{
						y += 360f;
					}
					ShootTriggered();
				}
			}
			else if (playerController.isRangeActive && !playerController.fsmUpperBody.GetBool(IsReloadAnim) && reloadStateTimer.isRunning)
			{
				TriggerReload();
			}
		}
		if (playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("PumpAction") && playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
		{
			if (playerController.isShooting)
			{
				playerController.isShooting = false;
				playerController.fsmUpperBody.SetBool(IsShootingAnim, value: false);
			}
			playerController.isAttacking = false;
			playerController.fsmUpperBody.SetBool(IsReloadAnim, value: false);
			if (playerController.isRangeActive && !playerController.fsmUpperBody.GetBool(IsReloadAnim) && reloadStateTimer.isRunning)
			{
				TriggerReload();
			}
		}
		if (playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("AttackMelee") && !timerDelayAttackEnd.isRunning && playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
		{
			playerController.fsmUpperBody.SetBool(IsMeleeAnim, value: false);
			playerController.fsmUpperBody.SetBool(IsThrowingAnim, value: false);
			if (playerController.isRangeActive && (playerController.isAiming || playerController.isRMBDown))
			{
				playerData.SetCurrentMoveSpeed(playerData.GetMoveAimSpeed());
			}
			else if (!playerController.isSprintDown && !playerController.isSprinting)
			{
				playerData.SetCurrentMoveSpeed(playerData.GetInitialMoveSpeed());
			}
			playerController.isThrowing = false;
		}
		if (playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("Reload") && playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
		{
			if (playerController.fsmUpperBody.GetBool(IsMeleeAnim))
			{
				playerController.fsmUpperBody.SetBool(IsMeleeAnim, value: false);
			}
			if (!timerReload.isRunning && playerController.fsmUpperBody.GetBool(IsReloadAnim))
			{
				timerReload.StartDuration(0.1f);
				int num = GetMagazineSize(equipedWeapon: true) - playerData.arrInventory[idxWeaponRange].Ammo;
				if (idWeaponRange > 0 && BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).ReloadPerAmmo && num > 1 && GetTotalAmmoWeapon() > 1)
				{
					playerController.animUpperChar.Play("Reload" + weaponStyle + playerController.angleRot, -1, 0f);
					AudioManager.PlaySFXTransform("rangedReload_" + idBaseWeaponRange, playerController.transform, playerController.network.isLocalPlayer);
					Reload();
				}
				else if (idWeaponRange > 0)
				{
					Reload();
					if (BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).isPumpAction)
					{
						playerController.animUpperChar.Play("KokangShotgun" + playerController.angleRot, -1, 0f);
					}
					else
					{
						playerController.fsmUpperBody.SetBool(IsReloadAnim, value: false);
					}
				}
			}
		}
		if (playerController.isThrowing)
		{
			playerController.animUpperChar.Play("Throw" + playerController.angleRot, -1, 0f);
		}
		if (playerController.isAiming != playerController.isRMBDown && playerController.network.GetEnableControl() && playerController.enableMoveChar && playerController.isRangeActive && !playerController.isAttackMelee)
		{
			playerController.SetAiming(playerController.isRMBDown, isWithoutCheckRMBDown: true);
		}
		if (playerController.fsmUpperBody.GetBool(IsMeleeAnim) && (playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("Idle") || playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("Move")) && !playerController.isAttackMelee)
		{
			playerController.fsmUpperBody.SetBool(IsMeleeAnim, value: false);
		}
		if (!playerController.isAiming && playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("AttackMelee") && playerController.isAttacking && (playerController.isLMBDown || ReleaseAttackTimer.isRunning))
		{
			if (playerController.weaponController.idWeaponMelee < 0)
			{
				playerController.animUpperChar.Play("AttackWeaponless" + playerController.angleRot, -1, playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).normalizedTime);
			}
			else if (playerController.animUpperChar.GetCurrentAnimatorClipInfo(0)[0].clip.name != "AttackMelee" + playerController.angleRot + "-" + idxAttackCombo && !timerDelayAttackEnd.isRunning)
			{
				playerController.animUpperChar.Play("AttackMelee" + playerController.angleRot + "-" + idxAttackCombo, -1, playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).normalizedTime);
			}
		}
		if (playerController.isAttackMelee && !playerController.isLMBDown && playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
		{
			ReleaseAttack();
		}
		if (timerResetCombo.isCompleted())
		{
			isResetCombo = true;
		}
		if (timerDelayShoot.isCompleted() && ctrBulletShoot <= 0)
		{
			ctrBulletShoot = shotsPerAttack;
		}
		if (ReleaseAttackTimer.isCompleted() && playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("AttackMelee") && playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.89f && !playerController.isLMBDown)
		{
			ReleaseAttack();
		}
	}

	private void Update()
	{
		if (playerController.isLMBDown != playerController.isAttackBtnPressed)
		{
			if (playerController.isAiming)
			{
				ReleaseAttack();
			}
			if (!playerController.isLMBDown && playerController.isAttackBtnPressed)
			{
				playerController.isAttackBtnPressed = false;
			}
		}
	}

	public void AttackTriggered(byte ammo, short aimDirection)
	{
		dirAimOtherPlayer = aimDirection;
		if (!playerController.network.GetEnableControl() || !playerController.enableMoveChar)
		{
			return;
		}
		playerController.isAttackBtnPressed = true;
		if (playerController.isAiming && playerController.isRangeActive)
		{
			playerController.network.ExecSyncAmmoWeapon(playerData.arrInventory[idxWeaponRange].Ammo);
			if (!playerController.weaponController.attackTimer.isRunning)
			{
				if (ctrBulletShoot <= 0 && !timerDelayShoot.isRunning)
				{
					ctrBulletShoot = shotsPerAttack;
				}
				ShootTriggered();
			}
		}
		else if (!playerController.fsmUpperBody.GetBool(IsMeleeAnim) && !playerController.isAiming && playerData.GetStamina() > 0f)
		{
			MeleeTriggered();
		}
	}

	public void ReleaseAttack(bool isAnimOnly = true)
	{
		if (ReleaseAttackTimer.isRunning || !(playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("AttackMelee") & isAnimOnly) || !(playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.89f) || playerController.meleeCollider.gameObject.activeSelf)
		{
			return;
		}
		ReleaseAttackTimer.CancelDuration();
		attackTimer.StartDuration(meleeInterval);
		meleeSprite.material.DOKill();
		playerController.animUpperChar.transform.DOKill();
		playerController.animUpperChar.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0f);
		meleeSprite.material.DOFloat(0f, "_Brightness", 0f);
		playerController.weaponController.meleeSprite.material.DOColor(new Color(0f, 0f, 0f), "_Tint", 0f);
		if (playerController.network.isLocalPlayer)
		{
			UIGameManager.Instance.ArrPlayerInfo[playerController.network.GetIDX()].ChargeMeleeProgressObject.SetActive(value: false);
			playerController.weaponController.MeleeTween.Kill();
		}
		if (playerController.fsmUpperBody.GetBool("isReloading"))
		{
			return;
		}
		if (playerController.fsmUpperBody.GetBool(IsMeleeAnim) || playerController.isAttackMelee)
		{
			if (!playerController.isAiming)
			{
				if (playerController.animUpperChar.GetCurrentAnimatorClipInfo(0).Length != 0)
				{
					TriggerAnimAttack();
				}
				if (playerController.network.isLocalPlayer && playerController.data.GetStamina() > 0f)
				{
					if (isMeleeCharging)
					{
						playerData.AddSubCurrentStamina(-Mathf.RoundToInt((float)needStaminaWeapon0 * 1.5f * playerController.PlayerMultiplyStatsData.GetMultiplyStaminaMeleeConsumption()));
					}
					else
					{
						playerData.AddSubCurrentStamina((float)(-needStaminaWeapon0) * playerController.PlayerMultiplyStatsData.GetMultiplyStaminaMeleeConsumption());
					}
					UIGameManager.Instance.barStamina.DOValue(playerData.GetStamina() / playerData.GetMaxStamina(), 0.15f);
				}
				playerController.SetAnimLowerSpeed(playerController.animspeed);
				playerController.isAttackMelee = false;
				playerController.isAttackMeleeSwing = true;
			}
			playerController.isAttacking = false;
		}
		playerController.isAttackBtnPressed = false;
	}

	private void TriggerAnimAttack()
	{
		bool dashBasicAttack = BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).DashBasicAttack;
		if (!playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("AttackMelee"))
		{
			if (dashBasicAttack)
			{
				playerController.directionDash = MathFunc.AngleToVector3(playerController.angleRot + (float)CameraGame.Instance.camRotate);
				playerController.Dash(playerController.directionDash, isUsingStamina: false, isDashAttack: true, isTrailEffectEnable: false, 0f, 0.05f, 0f).Forget();
			}
			playerController.animUpperChar.Play("AttackMelee" + playerController.angleRot + "-" + playerController.weaponController.idxAttackCombo, 0, 0.84f);
			playerController.SetAnimUpperSpeed(attackReleaseAnimSpeed);
			ShowMeleeCollider();
			return;
		}
		if (playerController.animUpperChar.speed != 0f && playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.8f)
		{
			if (dashBasicAttack)
			{
				playerController.directionDash = MathFunc.AngleToVector3(playerController.angleRot + (float)CameraGame.Instance.camRotate);
				playerController.Dash(playerController.directionDash, isUsingStamina: false, isDashAttack: true, isTrailEffectEnable: false, 0f, 0.05f, 0f).Forget();
			}
			playerController.animUpperChar.Play(playerController.animUpperChar.GetCurrentAnimatorClipInfo(0)[0].clip.name, 0, 0.84f);
			playerController.SetAnimUpperSpeed(attackReleaseAnimSpeed);
			ShowMeleeCollider();
			return;
		}
		string specialFullCharge = BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).SpecialFullCharge;
		if (isMeleeCharging)
		{
			switch (specialFullCharge)
			{
			case "DashAtk":
				playerController.directionDash = MathFunc.AngleToVector3(playerController.angleRot + (float)CameraGame.Instance.camRotate);
				playerController.Dash(playerController.directionDash, isUsingStamina: false, isDashAttack: true).Forget();
				playerController.SetAnimUpperSpeed(0f);
				UniTaskUtil.DelayedCall(this, 0.15f, () =>
				{
					ShowMeleeCollider();
					playerController.animUpperChar.Play(playerController.animUpperChar.GetCurrentAnimatorClipInfo(0)[0].clip.name, 0, 0.84f);
					playerController.SetAnimUpperSpeed(attackReleaseAnimSpeed);
				}).Forget();
				break;
			case "Spin":
				playerController.animUpperChar.Play("AttackCircular" + playerController.angleRot, 0, 0.78f);
				playerController.SetAnimUpperSpeed(1f);
				ShowMeleeCollider(isRoundCollider: true);
				break;
			case "DoubleAttack":
				playerController.directionDash = MathFunc.AngleToVector3(playerController.angleRot + (float)CameraGame.Instance.camRotate);
				playerController.Dash(playerController.directionDash, isUsingStamina: false, isDashAttack: true).Forget();
				playerController.SetAnimUpperSpeed(0f);
				UniTaskUtil.DelayedCall(this, 0.15f, () =>
				{
					ShowMeleeCollider();
					playerController.animUpperChar.Play(playerController.animUpperChar.GetCurrentAnimatorClipInfo(0)[0].clip.name, 0, 0.84f);
					playerController.SetAnimUpperSpeed(attackReleaseAnimSpeed);
					UniTaskUtil.DelayedCall(this, 0.15f, () =>
					{
						isMeleeCharging = true;
						playerController.isAttackMeleeSwing = true;
						ShowMeleeCollider();
						idxAttackCombo++;
						if (idxAttackCombo >= maxAttackCombo)
						{
							idxAttackCombo = 0;
						}
						playerController.animUpperChar.Play("AttackMelee" + playerController.angleRot + "-" + playerController.weaponController.idxAttackCombo, 0, 0.84f);
						playerController.SetAnimUpperSpeed(attackReleaseAnimSpeed);
					}).Forget();
				}).Forget();
				break;
			default:
				playerController.SetAnimUpperSpeed(attackReleaseAnimSpeed);
				ShowMeleeCollider();
				break;
			}
		}
		else
		{
			if (dashBasicAttack)
			{
				playerController.directionDash = MathFunc.AngleToVector3(playerController.angleRot + (float)CameraGame.Instance.camRotate);
				playerController.Dash(playerController.directionDash, isUsingStamina: false, isDashAttack: true, isTrailEffectEnable: false, 0f, 0.05f, 0f).Forget();
			}
			playerController.SetAnimUpperSpeed(attackReleaseAnimSpeed);
			ShowMeleeCollider();
		}
	}

	private void MeleeTriggered()
	{
		if (playerController.fsmUpperBody.GetBool(IsShootingAnim) || playerController.fsmUpperBody.GetCurrentAnimatorStateInfo(0).IsName("Melee"))
		{
			return;
		}
		if (playerController.fsmUpperBody.GetBool(IsReloadAnim))
		{
			playerController.fsmUpperBody.SetBool(IsReloadAnim, value: false);
		}
		ReleaseAttackTimer.StartDuration(timerRelaseAttack);
		if (idWeaponMelee != -1)
		{
			AudioManager.PlaySFXTransform("melee-sword-hold", playerController.transform, playerController.network.isLocalPlayer);
		}
		playerData.SetCurrentMoveSpeed(playerData.GetMoveAimSpeed());
		playerController.SetAnimLowerSpeed(0.6f);
		playerController.isAttacking = true;
		playerController.isAttackMelee = true;
		playerController.fsmUpperBody.SetBool(IsMeleeAnim, value: true);
		if (!isDisableHalfCharge)
		{
			halfChargeTimer.StartDuration(0.6f);
		}
		chargeTimer.StartDuration(1.2f);
		if (playerController.isSprinting && playerController.network.isLocalPlayer)
		{
			playerController.network.StopSprint();
		}
		if (playerController.network.isLocalPlayer)
		{
			UIGameManager.Instance.ArrPlayerInfo[playerController.network.GetIDX()].ChargeMeleeProgressObject.SetActive(value: true);
			UIGameManager.Instance.ArrPlayerInfo[playerController.network.GetIDX()].ChargeMeleeProgressRadial.fillAmount = 0f;
			MeleeTween = DOTween.To(() => UIGameManager.Instance.ArrPlayerInfo[playerController.network.GetIDX()].ChargeMeleeProgressRadial.fillAmount, (float x) =>
			{
				UIGameManager.Instance.ArrPlayerInfo[playerController.network.GetIDX()].ChargeMeleeProgressRadial.fillAmount = x;
			}, 1f, 1.3f).SetEase(Ease.Linear);
		}
	}

	private void ShootTriggered()
	{
		playerController.AnglePlayerAim(playerController.inputRotation, towardsFunctionOn: false);
		if (playerData.arrInventory[idxWeaponRange].Ammo > 0)
		{
			if (playerController.isAttacking || playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("AttackMelee"))
			{
				return;
			}
			playerController.isAttacking = true;
			attackTimer.StartDuration(attackInterval);
			if (timerDelayShoot.isRunning || playerController.fsmUpperBody.GetCurrentAnimatorStateInfo(0).IsName("Shoot") || ctrBulletShoot <= 0)
			{
				return;
			}
			if (BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).Type == "Rifle")
			{
				ctrBulletShoot--;
				if (ctrBulletShoot <= 0)
				{
					timerDelayShoot.StartDuration(0.4f);
				}
			}
			if (BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).IsAutoFire && playerController.network.isLocalPlayer)
			{
				float num = Quaternion.LookRotation(playerController.angleInput - playerController.weaponPos.position, Vector3.up).eulerAngles.y;
				if (num < 0f)
				{
					num += 360f;
				}
				playerController.network.SetAimDirection((short)num);
			}
			playerController.fsmUpperBody.SetBool(IsMeleeAnim, value: false);
			playerController.fsmUpperBody.SetBool(IsReloadAnim, value: false);
			playerController.fsmUpperBody.SetBool(IsShootingAnim, value: true);
		}
		else
		{
			if (rangeWeaponType == RangeWeaponType.Pistol)
			{
				AudioManager.PlaySFXTransform("pistol-empty", playerController.object2D.transform, playerController.network.isLocalPlayer);
			}
			else if (rangeWeaponType == RangeWeaponType.SMG)
			{
				AudioManager.PlaySFXTransform("rifle-empty", playerController.object2D.transform, playerController.network.isLocalPlayer);
			}
			else if (rangeWeaponType == RangeWeaponType.Crossbow)
			{
				AudioManager.PlaySFXTransform("rifle-empty", playerController.object2D.transform, playerController.network.isLocalPlayer);
			}
			else if (rangeWeaponType == RangeWeaponType.Shotgun)
			{
				AudioManager.PlaySFXTransform("shotgun-empty", playerController.object2D.transform, playerController.network.isLocalPlayer);
			}
			playerController.network.StopShoot();
		}
	}

	public void EquipWeaponID(int idWeapon, int idxInventoryWeapon)
	{
		if (idWeapon <= 0)
		{
			return;
		}
		if (BGDatabase_Weapon.GetEntityByKeyid(idWeapon).WeaponType == "Melee" && idxInventoryWeapon == 0)
		{
			idWeaponMelee = idWeapon;
			dmgWeapon0 = BGDatabase_Weapon.GetEntityByKeyid(idWeapon).Damage;
			dmgWeaponHalfCharge0 = BGDatabase_Weapon.GetEntityByKeyid(idWeapon).DamageHalfCharge;
			dmgWeaponFullCharge0 = BGDatabase_Weapon.GetEntityByKeyid(idWeapon).DamageFullCharge;
			stuntWeapon0 = BGDatabase_Weapon.GetEntityByKeyid(idWeapon).StuntTime;
			needStaminaWeapon0 = BGDatabase_Weapon.GetEntityByKeyid(idWeapon).NeedStamina;
			shakeAmplitudeWeapon0 = BGDatabase_Weapon.GetEntityByKeyid(idWeapon).AmplitudeShake;
			deadTypeWeapon0 = BGDatabase_Weapon.GetEntityByKeyid(idWeapon).DeadEnemyType;
			shakeDurWeapon0 = BGDatabase_Weapon.GetEntityByKeyid(idWeapon).DurShake;
			meleeInterval = BGDatabase_Weapon.GetEntityByKeyid(idWeapon).AttackInterval;
			timerRelaseAttack = BGDatabase_Weapon.GetEntityByKeyid(idWeapon).TimeReleaseAttack;
			isOneHitKnockback = BGDatabase_Weapon.GetEntityByKeyid(idWeapon).OneHitKnockback;
			MaxEnemyHit = BGDatabase_Weapon.GetEntityByKeyid(idWeapon).MaxHitEnemy;
			MaxEnemyHitHalfCharge = BGDatabase_Weapon.GetEntityByKeyid(idWeapon).MaxHitEnemyHalfCharge;
			MaxEnemyHitFullCharge = BGDatabase_Weapon.GetEntityByKeyid(idWeapon).MaxHitEnemyFullCharge;
			isDisableHalfCharge = BGDatabase_Weapon.GetEntityByKeyid(idWeapon).IsDisableHalfCharge;
			speedAim = BGDatabase_Weapon.GetEntityByKeyid(idWeapon).AimSpeed;
			attackReleaseAnimSpeed = BGDatabase_Weapon.GetEntityByKeyid(idWeapon).ReleaseAttackAnimSpeed;
			if (BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).SpecialFullCharge == "Spin")
			{
				maxAttackCombo = 1;
			}
			else
			{
				maxAttackCombo = 2;
			}
			for (int i = 0; i < SkinManager.Instance.listMeleeWeapon.Count; i++)
			{
				if (SkinManager.Instance.listMeleeWeapon[i].name == "Melee_" + idWeaponMelee)
				{
					idxSkinWeapon0 = i;
					meleeSpriteLib.spriteLibraryAsset = SkinManager.Instance.listMeleeWeapon[i];
				}
			}
			if (idxSkinWeapon0 == -1)
			{
				meleeSpriteLib.spriteLibraryAsset = null;
			}
		}
		else if (idxInventoryWeapon == 1 && idWeapon > 0)
		{
			idWeaponRange = idWeapon;
			idBaseWeaponRange = DataManager.Instance.GetBaseWeapon(idWeapon);
			dmgWeapon1 = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).Damage;
			stuntWeapon1 = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).StuntTime;
			needStaminaWeapon1 = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).NeedStamina;
			shakeAmplitudeWeapon1 = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).AmplitudeShake;
			deadTypeWeapon1 = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).DeadEnemyType;
			shakeDurWeapon1 = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).DurShake;
			attackInterval = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).AttackInterval;
			radiusSpread = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).RadiusBulletSpread;
			shotsPerAttack = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).ShotsPerAttack;
			speedAim = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).AimSpeed;
			minRangeAccuracy = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).MinRangeAccuracy;
			maxRangeAccuracy = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).MaxRangeAccuracy;
			timeAccuracy = 1f * playerController.PlayerMultiplyStatsData.GetMultiplyTimerGunAccuracy();
			if (BGDatabase_Weapon.GetEntityByKeyid(idWeapon).Type == "Pistol")
			{
				rangeWeaponType = RangeWeaponType.Pistol;
				weaponStyle = "Pistol";
			}
			else if (BGDatabase_Weapon.GetEntityByKeyid(idWeapon).Type == "SMG" || BGDatabase_Weapon.GetEntityByKeyid(idWeapon).Type == "Rifle")
			{
				rangeWeaponType = RangeWeaponType.SMG;
				weaponStyle = "Rifle";
			}
			else if (BGDatabase_Weapon.GetEntityByKeyid(idWeapon).Type == "Crossbow")
			{
				rangeWeaponType = RangeWeaponType.Crossbow;
				weaponStyle = "Rifle";
			}
			else if (BGDatabase_Weapon.GetEntityByKeyid(idWeapon).Type == "Shotgun")
			{
				weaponStyle = "Rifle";
				rangeWeaponType = RangeWeaponType.Shotgun;
			}
			else if (BGDatabase_Weapon.GetEntityByKeyid(idWeapon).Type == "GrenadeLauncher")
			{
				weaponStyle = "Rifle";
				rangeWeaponType = RangeWeaponType.GrenadeLauncher;
			}
			for (int j = 0; j < SkinManager.Instance.listRangeWeapon.Count; j++)
			{
				if (SkinManager.Instance.listRangeWeapon[j].name == "Range_" + idBaseWeaponRange)
				{
					idxSkinWeapon1 = j;
					rangeSpriteLib.spriteLibraryAsset = SkinManager.Instance.listRangeWeapon[j];
				}
			}
			if (idxSkinWeapon1 == -1)
			{
				rangeSpriteLib.spriteLibraryAsset = null;
			}
			playerController.isRangeActive = true;
			BuffWeaponRange(idWeaponRange);
		}
		if (playerController.network.isLocalPlayer)
		{
			meleeObject.SetActive(value: true);
		}
	}

	public void EquipWeaponInventory(int idxInventory, int ammo = -1, bool init = false)
	{
		if (idxInventory >= playerData.GetMaxInventory() || idxInventory >= playerData.arrInventory.Count || idxInventory < 0 || playerData.arrInventory.Count <= 0 || playerData.arrInventory[idxInventory].ID == -1)
		{
			return;
		}
		playerData.arrInventory[idxInventory].equip = true;
		BGDatabase_Weapon entityByKeyid = BGDatabase_Weapon.GetEntityByKeyid(playerData.arrInventory[idxInventory].ID);
		if (entityByKeyid != null)
		{
			if (entityByKeyid.WeaponType == "Melee")
			{
				idWeaponMelee = playerData.arrInventory[idxInventory].ID;
				dmgWeapon0 = BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).Damage;
				dmgWeaponHalfCharge0 = BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).DamageHalfCharge;
				dmgWeaponFullCharge0 = BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).DamageFullCharge;
				stuntWeapon0 = BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).StuntTime;
				needStaminaWeapon0 = BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).NeedStamina;
				shakeAmplitudeWeapon0 = BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).AmplitudeShake;
				deadTypeWeapon0 = BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).DeadEnemyType;
				shakeDurWeapon0 = BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).DurShake;
				meleeInterval = BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).AttackInterval;
				timerRelaseAttack = BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).TimeReleaseAttack;
				isOneHitKnockback = BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).OneHitKnockback;
				MaxEnemyHit = BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).MaxHitEnemy;
				MaxEnemyHitHalfCharge = BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).MaxHitEnemyHalfCharge;
				MaxEnemyHitFullCharge = BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).MaxHitEnemyFullCharge;
				isDisableHalfCharge = BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).IsDisableHalfCharge;
				speedAim = BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).AimSpeed;
				attackReleaseAnimSpeed = BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).ReleaseAttackAnimSpeed;
				if (BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).SpecialFullCharge == "Spin")
				{
					maxAttackCombo = 1;
				}
				else
				{
					maxAttackCombo = 2;
				}
				List<InventoryObject> arrInventory = playerData.arrInventory;
				int index = idxWeaponMelee;
				List<InventoryObject> arrInventory2 = playerData.arrInventory;
				int index2 = idxInventory;
				InventoryObject inventoryObject = playerData.arrInventory[idxInventory];
				InventoryObject inventoryObject2 = playerData.arrInventory[idxWeaponMelee];
				InventoryObject inventoryObject3 = (arrInventory[index] = inventoryObject);
				inventoryObject3 = (arrInventory2[index2] = inventoryObject2);
				playerData.arrInventory[idxWeaponRange].IdxInventory = idxWeaponRange;
				playerData.arrInventory[idxInventory].IdxInventory = idxInventory;
				if (playerController.network.isLocalPlayer)
				{
					UIGameManager.Instance.ammoIconList[idxWeaponMelee].gameObject.SetActive(value: false);
					UIGameManager.Instance.txtAmountList[idxWeaponMelee].gameObject.SetActive(value: false);
					for (int i = 0; i < SkinManager.Instance.listMeleeWeapon.Count; i++)
					{
						if (SkinManager.Instance.listMeleeWeapon[i].name == "Melee_" + playerData.arrInventory[idxWeaponMelee].ID)
						{
							idxSkinWeapon0 = i;
							meleeSpriteLib.spriteLibraryAsset = SkinManager.Instance.listMeleeWeapon[i];
						}
					}
					if (idxSkinWeapon0 == -1)
					{
						meleeSpriteLib.spriteLibraryAsset = null;
					}
					playerController.network.SetWeapon0(idWeaponMelee, init);
					if (!init)
					{
						playerController.network.EquipWeaponInventory(idxInventory);
					}
				}
			}
			else
			{
				idWeaponRange = playerData.arrInventory[idxInventory].ID;
				idBaseWeaponRange = DataManager.Instance.GetBaseWeapon(idWeaponRange);
				dmgWeapon1 = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).Damage;
				stuntWeapon1 = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).StuntTime;
				needStaminaWeapon1 = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).NeedStamina;
				shakeAmplitudeWeapon1 = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).AmplitudeShake;
				deadTypeWeapon1 = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).DeadEnemyType;
				shakeDurWeapon1 = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).DurShake;
				List<InventoryObject> arrInventory = playerData.arrInventory;
				int index2 = idxWeaponRange;
				List<InventoryObject> arrInventory3 = playerData.arrInventory;
				int index = idxInventory;
				InventoryObject inventoryObject2 = playerData.arrInventory[idxInventory];
				InventoryObject inventoryObject = playerData.arrInventory[idxWeaponRange];
				InventoryObject inventoryObject3 = (arrInventory[index2] = inventoryObject2);
				inventoryObject3 = (arrInventory3[index] = inventoryObject);
				inventoryObject = playerData.arrInventory[idxWeaponRange];
				InventoryObject inventoryObject8 = playerData.arrInventory[idxInventory];
				index = playerData.arrInventory[idxInventory].IdxInventory;
				index2 = playerData.arrInventory[idxWeaponRange].IdxInventory;
				inventoryObject.IdxInventory = index;
				inventoryObject8.IdxInventory = index2;
				if ((bool)playerController.inventoryManager && (bool)playerController.inventoryManager.txtAmountList[idxInventory])
				{
					playerController.inventoryManager.txtAmountList[idxInventory].text = playerData.arrInventory[idxInventory].Ammo.ToString();
				}
				if ((bool)playerController.inventoryManager && (bool)playerController.inventoryManager.txtAmountList[idxWeaponRange])
				{
					playerController.inventoryManager.txtAmountList[idxWeaponRange].text = playerData.arrInventory[idxWeaponRange].Ammo.ToString();
				}
				attackInterval = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).AttackInterval;
				radiusSpread = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).RadiusBulletSpread;
				shotsPerAttack = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).ShotsPerAttack;
				speedAim = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).AimSpeed;
				minRangeAccuracy = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).MinRangeAccuracy;
				maxRangeAccuracy = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).MaxRangeAccuracy;
				timeAccuracy = 1f * playerController.PlayerMultiplyStatsData.GetMultiplyTimerGunAccuracy();
				if (BGDatabase_Weapon.GetEntityByKeyid(playerData.arrInventory[idxWeaponRange].ID).Type == "Pistol")
				{
					rangeWeaponType = RangeWeaponType.Pistol;
					weaponStyle = "Pistol";
				}
				else if (BGDatabase_Weapon.GetEntityByKeyid(playerData.arrInventory[idxWeaponRange].ID).Type == "SMG" || BGDatabase_Weapon.GetEntityByKeyid(playerData.arrInventory[idxWeaponRange].ID).Type == "Rifle")
				{
					rangeWeaponType = RangeWeaponType.SMG;
					weaponStyle = "Rifle";
				}
				else if (BGDatabase_Weapon.GetEntityByKeyid(playerData.arrInventory[idxWeaponRange].ID).Type == "Crossbow")
				{
					rangeWeaponType = RangeWeaponType.Crossbow;
					weaponStyle = "Rifle";
				}
				else if (BGDatabase_Weapon.GetEntityByKeyid(playerData.arrInventory[idxWeaponRange].ID).Type == "Shotgun")
				{
					rangeWeaponType = RangeWeaponType.Shotgun;
					weaponStyle = "Rifle";
				}
				else if (BGDatabase_Weapon.GetEntityByKeyid(playerData.arrInventory[idxWeaponRange].ID).Type == "GrenadeLauncher")
				{
					rangeWeaponType = RangeWeaponType.GrenadeLauncher;
					weaponStyle = "Rifle";
				}
				playerController.isRangeActive = true;
				if (playerController.network.isLocalPlayer)
				{
					playerController.inventoryManager.ammoIconList[idxWeaponRange].gameObject.SetActive(value: true);
					playerController.inventoryManager.txtAmountList[idxWeaponRange].gameObject.SetActive(value: true);
					UIGameManager.Instance.ammoIconList[idxWeaponRange].gameObject.SetActive(value: true);
					UIGameManager.Instance.txtAmountList[idxWeaponRange].gameObject.SetActive(value: true);
					if (ammo != -1)
					{
						playerData.arrInventory[idxWeaponRange].Ammo = ammo;
					}
					UIGameManager.Instance.txtAmountList[idxWeaponRange].text = playerData.arrInventory[idxWeaponRange].Ammo + "/" + playerController.weaponController.GetTotalAmmoWeaponString();
					if (playerData.arrInventory[idxInventory].Name == "Null")
					{
						playerController.inventoryManager.ammoIconList[idxInventory].gameObject.SetActive(value: false);
						playerController.inventoryManager.txtAmountList[idxInventory].gameObject.SetActive(value: false);
					}
					for (int j = 0; j < SkinManager.Instance.listRangeWeapon.Count; j++)
					{
						if (SkinManager.Instance.listRangeWeapon[j].name == "Range_" + idBaseWeaponRange)
						{
							idxSkinWeapon1 = j;
							rangeSpriteLib.spriteLibraryAsset = SkinManager.Instance.listRangeWeapon[j];
						}
					}
					if (idxSkinWeapon1 == -1)
					{
						rangeSpriteLib.spriteLibraryAsset = null;
					}
					playerController.network.SetWeapon1(idWeaponRange, init);
					if (!init)
					{
						playerController.network.EquipWeaponInventory(idxInventory);
					}
					BuffWeaponRange(idWeaponRange);
				}
			}
			playerData.arrInventory[idxInventory].equip = false;
		}
		meleeObject.SetActive(value: true);
		if (playerController.network.isLocalPlayer)
		{
			playerData?.InitImageInventoryLocal();
		}
	}

	public void BuffWeaponRange(int idWeapon)
	{
		if (idWeapon <= 0 || BGDatabase_Weapon.GetEntityByKeyid(idWeapon).Buff == null)
		{
			return;
		}
		foreach (string item in BGDatabase_Weapon.GetEntityByKeyid(idWeapon).Buff)
		{
			if (item.Contains("Dmg"))
			{
				dmgWeapon1 = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).Damage;
				if (item.Contains("%"))
				{
					dmgWeapon1 = Mathf.RoundToInt(dmgWeapon1 * (((float)MathFunc.ExtractNumber(item) + 100f) / 100f));
				}
				else
				{
					dmgWeapon1 += MathFunc.ExtractNumber(item);
				}
			}
			else if (item.Contains("Acc"))
			{
				if (item.Contains("%"))
				{
					int num = MathFunc.ExtractNumber(item);
					timeAccuracy = 1f * playerController.PlayerMultiplyStatsData.GetMultiplyTimerGunAccuracy();
					minRangeAccuracy = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).MinRangeAccuracy;
					maxRangeAccuracy = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).MaxRangeAccuracy;
					timeAccuracy *= 100f / (float)num;
					minRangeAccuracy *= 100f / (float)(num / 2);
					maxRangeAccuracy *= 100f / (float)(num / 2);
				}
			}
			else if (item.Contains("SpdAim"))
			{
				speedAim = BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).AimSpeed;
				if (item.Contains("%"))
				{
					speedAim *= ((float)MathFunc.ExtractNumber(item) + 100f) / 100f;
				}
				else
				{
					speedAim += MathFunc.ExtractNumber(item);
				}
			}
		}
	}

	public void UnEquipWeapon(int idxInventory, bool fromServer)
	{
		if (!fromServer)
		{
			switch (idxInventory)
			{
			case 0:
				playerController.network.UnequipWeapon0();
				break;
			case 1:
				playerController.network.UnequipWeapon1();
				break;
			}
		}
		else if (idxInventory < playerData.arrInventory.Count && idxInventory != -1 && playerData.arrInventory[idxInventory].Name != "Null")
		{
			switch (idxInventory)
			{
			case 0:
			{
				idWeaponMelee = -1;
				idxSkinWeapon0 = -1;
				meleeObject.SetActive(value: false);
				for (int j = 2; j < playerData.GetMaxInventory(); j++)
				{
					if (playerData.arrInventory[j].Name == "Null")
					{
						InventoryObject value2 = playerData.arrInventory[idxInventory];
						playerData.arrInventory[idxInventory] = playerData.arrInventory[j];
						playerData.arrInventory[j] = value2;
						playerData.arrInventory[j].equip = false;
						break;
					}
				}
				if (playerController.network.isLocalPlayer)
				{
					playerData?.InitImageInventoryLocal();
				}
				break;
			}
			case 1:
			{
				playerData.arrInventory[1].Name = "Null";
				playerData.arrInventory[1].ID = -1;
				playerData.arrInventory[1].Ammo = 0;
				playerData.arrInventory[1].Amount = 0;
				playerData.arrInventory[1].Durability = -1f;
				idWeaponRange = -1;
				idBaseWeaponRange = -1;
				idxSkinWeapon1 = -1;
				playerController.isRangeActive = false;
				if (!playerController.network.isLocalPlayer)
				{
					break;
				}
				UIGameManager.Instance.ammoIconList[idxWeaponRange].gameObject.SetActive(value: false);
				UIGameManager.Instance.txtAmountList[idxWeaponRange].gameObject.SetActive(value: false);
				for (int i = 2; i < playerData.GetMaxInventory(); i++)
				{
					if (playerData.arrInventory[i].Name == "Null")
					{
						InventoryObject value = playerData.arrInventory[idxInventory];
						playerController.inventoryManager.ammoIconList[idxInventory].gameObject.SetActive(value: false);
						playerController.inventoryManager.txtAmountList[idxInventory].gameObject.SetActive(value: false);
						playerData.arrInventory[idxInventory] = playerData.arrInventory[i];
						playerData.arrInventory[i] = value;
						playerData.arrInventory[i].equip = false;
						playerController.inventoryManager.ammoIconList[i].gameObject.SetActive(value: true);
						playerController.inventoryManager.txtAmountList[i].gameObject.SetActive(value: true);
						playerController.inventoryManager.txtAmountList[i].text = playerData.arrInventory[i].Ammo.ToString();
						break;
					}
				}
				playerData?.InitImageInventoryLocal();
				break;
			}
			}
		}
		if (idxInventory == 1)
		{
			playerController.isRangeActive = false;
			idWeaponRange = -1;
			idBaseWeaponRange = -1;
			idxSkinWeapon1 = -1;
		}
	}

	public async UniTask Shoot()
	{
		CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
		int baseWeaponRangeId = idBaseWeaponRange;
		int weaponRangeId = playerController.network.GetIdWeapon1();
		await UniTask.Delay(TimeSpan.FromSeconds(0.017000000923871994), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationTokenOnDestroy);
		if (playerController.network.isLocalPlayer)
		{
			if (playerData.arrInventory[idxWeaponRange].Ammo > 0)
			{
				playerData.arrInventory[idxWeaponRange].Ammo--;
				playerController.network.ExecSubtractAmmo();
			}
			playerController.inventoryManager.txtAmountList[idxWeaponRange].text = playerData.arrInventory[idxWeaponRange].Ammo.ToString();
			UIGameManager.Instance.txtAmountList[idxWeaponRange].text = playerData.arrInventory[idxWeaponRange].Ammo + "/" + playerController.weaponController.GetTotalAmmoWeaponString();
		}
		Vector3 initDir;
		Vector3 direction;
		if (playerController.network.isLocalPlayer)
		{
			initDir = (new Vector3(playerController.angleInput.x, 0f, playerController.angleInput.z) - new Vector3(playerController.weaponPos.position.x, 0f, playerController.weaponPos.position.z)).normalized;
			direction = (new Vector3(playerController.angleGround.x, 0f, playerController.angleGround.z) - new Vector3(playerController.weaponPos.position.x, 0f, playerController.weaponPos.position.z)).normalized;
		}
		else
		{
			float num = dirAimOtherPlayer;
			initDir = new Vector3(Mathf.Sin(MathF.PI / 180f * num), 0f, Mathf.Cos(MathF.PI / 180f * num)).normalized.normalized;
			direction = initDir;
		}
		if (playerController.network.isLocalPlayer)
		{
			CameraGame.Instance.CameraShake(shakeDurWeapon1, shakeAmplitudeWeapon1);
		}
		int num2 = shotsPerAttack;
		if (BGDatabase_Weapon.GetEntityByKeyid(baseWeaponRangeId)?.Type == "Rifle")
		{
			num2 = 1;
		}
		for (int i = 0; i < num2; i++)
		{
			Vector3 vector = initDir + UnityEngine.Random.insideUnitSphere * radiusSpread / 50f;
			Vector3 vector2 = vector + UnityEngine.Random.insideUnitSphere * 0.002f * accuracy;
			vector = new Vector3(vector2.x, vector.y, vector2.z);
			ShotEffect(initDir).Forget();
			BulletImpactPool bulletObject = null;
			if (BGDatabase_Weapon.GetEntityByKeyid(baseWeaponRangeId).isNotUsingGunPowder)
			{
				bulletObject = ArrowSpawner.Instance.Get();
				bulletObject.transform.rotation = muzzle.transform.rotation;
			}
			else
			{
				bulletObject = BulletSpawner.Instance.Get();
			}
			bulletObject.transform.position = playerController.weaponPosSprite.position;
			if (rangeWeaponType == RangeWeaponType.GrenadeLauncher)
			{
				if (playerController.network.isLocalPlayer)
				{
					playerController.network.ExecGrenadeLauncher(direction);
				}
			}
			else
			{
				bool flag = false;
				foreach (EnemyController item in GameManager.Instance.arrEnemyController)
				{
					if (!item.isSpriteInactive && item.enemyCollider.bounds.Contains(playerController.weaponPos.position))
					{
						flag = true;
						FinishBullet(bulletObject, isCollision: true, playerController.weaponPos.position, item.enemyCollider, initDir, item.enemyCollider.transform);
						break;
					}
				}
				if (!flag)
				{
					playerController.playerColliderComponent.enabled = false;
					RaycastHit hit2;
					if (GameModes.Instance.friendlyFire)
					{
						if (Physics.SphereCast(playerController.weaponPos.position, 0.0001f, vector, out var hit, 50f, _layerBulletColliderFriendlyFire))
						{
							bulletObject.transform.DOMove(hit.point, 40f).SetSpeedBased(isSpeedBased: true).OnComplete(() =>
							{
								FinishBullet(bulletObject, isCollision: true, hit.point, hit.collider, initDir, hit.transform);
							});
						}
						else
						{
							bulletObject.transform.DOMove(playerController.weaponPosSprite.position + vector * 50f, 40f).SetSpeedBased(isSpeedBased: true).OnComplete(() =>
							{
								FinishBullet(bulletObject, isCollision: false, hit.point, hit.collider, initDir, hit.transform);
							});
						}
					}
					else if (Physics.SphereCast(playerController.weaponPos.position, 0.0001f, vector, out hit2, 50f, _layerBulletCollider))
					{
						bulletObject.transform.DOMove(hit2.point, 40f).SetSpeedBased(isSpeedBased: true).OnComplete(() =>
						{
							FinishBullet(bulletObject, isCollision: true, hit2.point, hit2.collider, initDir, hit2.transform);
						});
					}
					else
					{
						bulletObject.transform.DOMove(playerController.weaponPosSprite.position + vector * 50f, 40f).SetSpeedBased(isSpeedBased: true).OnComplete(() =>
						{
							FinishBullet(bulletObject, isCollision: false, hit2.point, hit2.collider, initDir, hit2.transform);
						});
					}
					playerController.playerColliderComponent.enabled = true;
				}
			}
			AudioManager.PlaySFXTransform("ranged_" + DataManager.Instance.GetBaseWeapon(weaponRangeId), playerController.object2D.transform, playerController.network.isLocalPlayer);
		}
		if (playerController.network.isLocalPlayer)
		{
			playerController.SetAimingSpeed(isFirstShoot: false);
		}
		if (!NetworkGameManager.Instance.isServer)
		{
			return;
		}
		foreach (EnemyController item2 in GameManager.Instance.arrEnemyController)
		{
			float num3 = MathFunc.Distance(item2.middlePos.position, playerController.weaponPos.position);
			if (!(num3 < 13f) || !(item2.network.GetHealth() > 0f) || item2.GetCurrentStateHash() == AnimatorHashManager.ChasingHash || item2.GetCurrentStateHash() == AnimatorHashManager.AlertChasingHash || item2.isAttacking)
			{
				continue;
			}
			if (item2.network.GetIsHovering())
			{
				if (!item2.network.networkPhoton.isFallingHovering && num3 < 8f)
				{
					item2.attack.targetChasing = playerController.targetedPoint;
					item2.network.networkPhoton.isFallingHovering = true;
				}
			}
			else
			{
				if (item2.network.networkPhoton.isDeaf || item2.attack.fov.isDisable)
				{
					continue;
				}
				if (item2.animatorState.HasParam("IsEscapeDanger"))
				{
					item2.animatorState.SetFloat("DangerPos", MathFunc.PositionToFloat(playerController.targetedPoint.position));
					item2.SetState(EnemyState.EscapeDanger);
					continue;
				}
				Vector3 normalized = (item2.middlePos.position - playerController.weaponPos.position).normalized;
				if (!item2.isDown && !Physics.Raycast(playerController.weaponPos.position, normalized, num3, GameManager.Instance.wallFloorCollider))
				{
					item2.network.networkPhoton.RpcEnemyAggro();
					item2.ChasingPlayer(playerController);
				}
				if (item2.isFakeDead && item2.attack.targetChasing == null && num3 < 8f)
				{
					playerController.targetedPoint.position = new Vector3(playerController.targetedPoint.position.x, item2.transform.position.y, playerController.targetedPoint.position.z);
					item2.attack.targetChasing = playerController.targetedPoint;
					item2.timerStunt.StartDuration(UnityEngine.Random.Range(0.5f, 1.5f));
				}
			}
		}
	}

	private void FinishBullet(BulletImpactPool bulletObj, bool isCollision, Vector3 hitPosition, Collider collider, Vector3 initDir, Transform hitTransform)
	{
		_ = initDir + UnityEngine.Random.insideUnitSphere * radiusSpread / 50f;
		PlayerController playerController = null;
		bool flag = true;
		if (isCollision)
		{
			if (collider.CompareTag("EnemyCollider"))
			{
				CheckDamageToEnemy(collider.GetComponent<EnemyCollider>().enemyControler, isRange: true);
				flag = false;
			}
			else if (GameModes.Instance.friendlyFire && collider.CompareTag("PlayerCollider"))
			{
				playerController = collider.transform.parent.GetComponent<PlayerController>();
				CheckDamageToOtherPlayer(playerController, isRange: true);
				flag = false;
			}
			if (hitTransform.GetComponent<ObjectCollisionBullet>() != null)
			{
				ObjectCollisionBullet component = hitTransform.GetComponent<ObjectCollisionBullet>();
				CheckDamageToBreakableObject(component, isRange: true, hitPosition);
				if (component.isDisabled)
				{
					flag = false;
				}
			}
		}
		if (BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).isNotUsingGunPowder)
		{
			if (flag)
			{
				UniTaskUtil.DelayedCall(this, 2f, () =>
				{
					ArrowSpawner.Instance.Release(bulletObj);
				}).Forget();
			}
			else
			{
				ArrowSpawner.Instance.Release(bulletObj);
			}
		}
		else
		{
			UniTaskUtil.DelayedCall(this, 0.7f, () =>
			{
				BulletSpawner.Instance.Release(bulletObj);
			}).Forget();
		}
	}

	private void ImpactEffect(Vector3 impactPos, string typeCollision = "")
	{
		ObjectImpactPool objectImpactPool = ImpactSpawner.Instance.Get();
		objectImpactPool.transform.position = impactPos;
		objectImpactPool.transform.rotation = muzzle.transform.rotation;
		objectImpactPool.typeImpact = typeCollision.ParseEnum<ObjectImpactPool.ImpactType>();
		objectImpactPool.initType();
	}

	private async UniTask ShotEffect(Vector3 muzzleDirection)
	{
		CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
		muzzle.transform.position = playerController.weaponPosSprite.position;
		muzzle.transform.rotation = Quaternion.LookRotation(muzzleDirection);
		muzzle.SetActive(value: false);
		if (!BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).isNotUsingGunPowder)
		{
			muzzle.SetActive(value: true);
		}
		await UniTask.Delay(TimeSpan.FromSeconds(0.019999999552965164), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationTokenOnDestroy);
		if (idWeaponRange > 0 && !BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).isPumpAction)
		{
			playerController.shell.EjectShell(rangeObject.transform.position, new Vector3(0f, playerController.origin.localEulerAngles.y - 45f, 0f), rangeWeaponType);
		}
	}

	public void ShowMeleeCollider(bool isRoundCollider = false)
	{
		ColliderMelee(isRoundCollider).Forget();
	}

	private async UniTask ColliderMelee(bool isRoundCollider = false)
	{
		CancellationToken token = this.GetCancellationTokenOnDestroy();
		await UniTask.Delay(TimeSpan.FromSeconds(0.05000000074505806), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		if (!playerController.isAttacking)
		{
			chargeTimer.StopDuration();
			halfChargeTimer.StopDuration();
		}
		playerController.punchCollider.eulerAngles = playerController.meleeCollider.eulerAngles;
		Transform transform = playerController.meleeCollider.GetChild(0).transform;
		if (playerController.meleeCollider.localEulerAngles.y == 0f)
		{
			transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 2.2f);
		}
		if (playerController.meleeCollider.localEulerAngles.y == 180f)
		{
			transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1.7f);
		}
		if (playerController.meleeCollider.localEulerAngles.y == 135f || playerController.meleeCollider.localEulerAngles.y == 225f)
		{
			transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1.3f);
		}
		else if (playerController.meleeCollider.localEulerAngles.y >= 90f && playerController.meleeCollider.localEulerAngles.y != 315f)
		{
			transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1.1f);
		}
		else
		{
			transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 1.7f);
		}
		if (idWeaponMelee == -1)
		{
			AudioManager.PlaySFXTransform("player-punch", playerController.transform, playerController.network.isLocalPlayer);
			playerController.punchCollider.gameObject.SetActive(value: true);
		}
		else if (isRoundCollider)
		{
			playerController.RoundMeleeCollider.gameObject.SetActive(value: true);
		}
		else
		{
			playerController.meleeCollider.gameObject.SetActive(value: true);
		}
		await UniTask.Delay(TimeSpan.FromSeconds(0.019999999552965164), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		if (idWeaponMelee == -1)
		{
			playerController.punchCollider.gameObject.SetActive(value: false);
		}
		else if (isRoundCollider)
		{
			playerController.RoundMeleeCollider.gameObject.SetActive(value: false);
		}
		else
		{
			playerController.meleeCollider.gameObject.SetActive(value: false);
		}
		meleeSprite.material.DOKill();
		meleeSprite.material.DOFloat(0f, "_Brightness", 0.3f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuint)
			.SetDelay(0.2f);
		meleeSprite.material.DOColor(new Color(0f, 0f, 0f), "_Tint", 0f);
		await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, token);
		if (!playerController.isAttacking)
		{
			isMeleeCharging = false;
			isHalfMeleeCharging = false;
		}
	}

	public string GetTotalAmmoWeaponString()
	{
		string result = "";
		int num = 0;
		if (playerData.arrInventory[idxWeaponRange].Name != "Null")
		{
			for (int i = 2; i < playerData.GetMaxInventory(); i++)
			{
				if (playerData.arrInventory[i].Name != "Null" && playerData.arrInventory[i].ID >= 100 && BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).AmmoTypeID == playerData.arrInventory[i].ID)
				{
					num += playerData.arrInventory[i].Amount;
				}
			}
			result = num.ToString();
			if (BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).IsTrainingWeapon)
			{
				result = "∞";
			}
		}
		return result;
	}

	public int GetTotalAmmoWeapon()
	{
		int num = 0;
		if (playerData.arrInventory[idxWeaponRange].Name != "Null")
		{
			for (int i = 2; i < playerData.GetMaxInventory(); i++)
			{
				if (playerData.arrInventory[i].Name != "Null" && playerData.arrInventory[i].ID >= 100 && BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).AmmoTypeID == playerData.arrInventory[i].ID)
				{
					num += playerData.arrInventory[i].Amount;
				}
			}
		}
		return num;
	}

	public void TriggerReload()
	{
		if (playerController.fsmUpperBody.GetBool(IsMeleeAnim))
		{
			playerController.fsmUpperBody.SetBool(IsMeleeAnim, value: false);
		}
		if (playerData.arrInventory[idxWeaponRange].Name != "Null" && playerData.arrInventory[idxWeaponRange].Ammo < GetMagazineSize(equipedWeapon: true) && (GetTotalAmmoWeapon() > 0 || BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).IsTrainingWeapon))
		{
			playerController.fsmUpperBody.SetBool(IsReloadAnim, value: true);
			AudioManager.PlaySFXTransform("rangedReload_" + idBaseWeaponRange, playerController.transform, playerController.network.isLocalPlayer);
		}
		else
		{
			if (GetTotalAmmoWeapon() != 0)
			{
				return;
			}
			if (playerData.arrInventory[idxWeaponRange].Name != "Null")
			{
				if (rangeWeaponType == RangeWeaponType.Pistol)
				{
					AudioManager.PlaySFXTransform("pistol-empty", playerController.object2D.transform, playerController.network.isLocalPlayer);
				}
				else if (rangeWeaponType == RangeWeaponType.SMG)
				{
					AudioManager.PlaySFXTransform("rifle-empty", playerController.object2D.transform, playerController.network.isLocalPlayer);
				}
				else if (rangeWeaponType == RangeWeaponType.Crossbow)
				{
					AudioManager.PlaySFXTransform("rifle-empty", playerController.object2D.transform, playerController.network.isLocalPlayer);
				}
				else if (rangeWeaponType == RangeWeaponType.Shotgun)
				{
					AudioManager.PlaySFXTransform("shotgun-empty", playerController.object2D.transform, playerController.network.isLocalPlayer);
				}
			}
			playerController.fsmUpperBody.SetBool(IsReloadAnim, value: false);
		}
	}

	public void Reload()
	{
		int num = GetMagazineSize(equipedWeapon: true) - playerData.arrInventory[idxWeaponRange].Ammo;
		if (idWeaponRange > 0 && BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).ReloadPerAmmo)
		{
			num = 1;
		}
		if (BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).IsTrainingWeapon)
		{
			playerData.arrInventory[idxWeaponRange].Ammo = GetMagazineSize(equipedWeapon: true);
			playerController.inventoryManager.txtAmountList[idxWeaponRange].text = playerData.arrInventory[idxWeaponRange].Ammo.ToString();
		}
		for (int i = 2; i < playerData.GetMaxInventory(); i++)
		{
			if (!(playerData.arrInventory[i].Name != "Null") || playerData.arrInventory[i].ID < 100 || BGDatabase_Weapon.GetEntityByKeyid(idBaseWeaponRange).AmmoTypeID != playerData.arrInventory[i].ID)
			{
				continue;
			}
			if (playerData.arrInventory[i].Amount > num)
			{
				playerData.arrInventory[i].Amount -= num;
				playerData.arrInventory[idxWeaponRange].Ammo += num;
				if (playerController.network.isLocalPlayer)
				{
					playerController.inventoryManager.txtAmountList[idxWeaponRange].text = playerData.arrInventory[idxWeaponRange].Ammo.ToString();
					playerController.inventoryManager.txtAmountList[i].text = playerData.arrInventory[i].Amount.ToString();
				}
				playerController.network.ExecSyncDataInventory(idxWeaponRange, playerData.arrInventory[idxWeaponRange].Ammo);
				playerController.network.ExecSyncDataInventory(i, playerData.arrInventory[i].Amount);
				num = 0;
			}
			else
			{
				playerData.arrInventory[idxWeaponRange].Ammo += playerData.arrInventory[i].Amount;
				num -= playerData.arrInventory[i].Amount;
				if (playerController.network.isLocalPlayer)
				{
					playerController.inventoryManager.txtAmountList[idxWeaponRange].text = playerData.arrInventory[idxWeaponRange].Ammo.ToString();
				}
				playerData.RemoveInventory(i);
			}
			if (num == 0)
			{
				break;
			}
		}
		if (UIGameManager.Instance != null)
		{
			playerData.InitImageInventoryLocal();
		}
	}

	public void ReloadAnimation()
	{
		if (playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).IsTag("Reload") && playerController.fsmUpperBody.GetBool(IsReloadAnim) && playerController.angleRot != playerController.prevAngleRot)
		{
			playerController.animUpperChar.Play("Reload" + weaponStyle + playerController.angleRot, -1, playerController.animUpperChar.GetCurrentAnimatorStateInfo(0).normalizedTime);
		}
	}

	public async UniTask HitDummy(Material mat)
	{
		CancellationToken cancellationTokenOnDestroy = this.GetCancellationTokenOnDestroy();
		mat.EnableKeyword("_EMISSION");
		await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationTokenOnDestroy);
		mat.DisableKeyword("_EMISSION");
	}

	public void CheckExplosionDamage(Transform objTransform)
	{
		GameManager.Instance.CheckModifierExplosionCallHorde();
		foreach (EnemyController item in GameManager.Instance.arrEnemyController)
		{
			if (!item.animatorState.HasParam("IsEscapeDanger") && MathFunc.Distance(item.transform.position, objTransform.position) < 3.5f && item.network.GetHealth() > 0f && !item.isDead && item.GetCurrentStateHash() != AnimatorHashManager.HoveringHash)
			{
				Vector3 vector = new Vector3(objTransform.transform.position.x, item.middlePos.position.y, objTransform.position.z);
				Vector3 normalized = (item.middlePos.position - vector).normalized;
				float maxDistance = Vector3.Distance(vector, item.middlePos.position);
				if (!Physics.Raycast(vector, normalized, maxDistance, obstacleMask))
				{
					item.Hurt(200f, 0.2f, playerController.network.isLocalPlayer, playerController.network.GetIDX(), 2, isGrenade: true);
				}
			}
		}
		if (!GameModes.Instance.isGrenadeFriendlyFire)
		{
			return;
		}
		PlayerController ownPlayer = NetworkGameManager.Instance.ownPlayer;
		if (MathFunc.Distance(ownPlayer.transform.position, objTransform.position) < 1.75f)
		{
			Vector3 vector2 = new Vector3(objTransform.transform.position.x, ownPlayer.weaponPos.position.y, objTransform.position.z);
			Vector3 normalized2 = (ownPlayer.weaponPos.position - vector2).normalized;
			float maxDistance2 = Vector3.Distance(vector2, ownPlayer.weaponPos.position);
			if (!Physics.Raycast(vector2, normalized2, maxDistance2, obstacleMask))
			{
				ownPlayer.network.ExecHurtEffect(ownPlayer.network.GetIDX());
				ownPlayer.network.AddSubHealth(-45f * ownPlayer.PlayerMultiplyStatsData.GetMultiplyDamageExplosion());
			}
		}
	}

	public void CheckEnemyAggro(Transform objTransform, float minDist = 13f)
	{
		if (!NetworkGameManager.Instance.isServer)
		{
			return;
		}
		foreach (EnemyController item in GameManager.Instance.arrEnemyController)
		{
			float num = MathFunc.Distance(item.transform.position, objTransform.position);
			if (item.network.networkPhoton.isDeaf || !(num < minDist))
			{
				continue;
			}
			if (item.animatorState.HasParam("IsEscapeDanger"))
			{
				item.animatorState.SetFloat("DangerPos", MathFunc.PositionToFloat(objTransform.position));
				item.SetState(EnemyState.EscapeDanger);
			}
			else
			{
				if (item.GetCurrentStateHash() == AnimatorHashManager.AttackingHash || !(item.network.GetHealth() > 0f) || item.GetCurrentStateHash() == AnimatorHashManager.ChasingHash || item.GetCurrentStateHash() == AnimatorHashManager.AlertChasingHash || item.isAttacking)
				{
					continue;
				}
				if (item.network.GetIsHovering())
				{
					if (!item.network.networkPhoton.isFallingHovering)
					{
						item.attack.isChasingSound = true;
						item.attack.targetChasing = objTransform;
						item.network.networkPhoton.isFallingHovering = true;
					}
				}
				else
				{
					if (item.attack.fov.isDisable)
					{
						continue;
					}
					Vector3 normalized = (item.middlePos.position - playerController.weaponPos.position).normalized;
					bool flag = false;
					foreach (string roomCollider in item.roomColliders)
					{
						if (roomCollider == playerController.RoomName)
						{
							flag = true;
							break;
						}
					}
					if (!item.isDown && (!Physics.Raycast(playerController.weaponPos.position, normalized, num, GameManager.Instance.wallFloorCollider) | flag))
					{
						item.network.networkPhoton.RpcEnemyAggro();
						item.ChasingObject(objTransform);
					}
					if (item.isFakeDead && item.attack.targetChasing == null)
					{
						item.attack.targetChasing = objTransform;
						item.timerStunt.StartDuration(UnityEngine.Random.Range(0.5f, 1.5f));
					}
				}
			}
		}
	}

	public void FireCannonAtPoint(Vector3 point, Rigidbody objectThrowed)
	{
		float h = 1.5f;
		objectThrowed.velocity = Vector3.zero;
		objectThrowed.rotation = Quaternion.identity;
		objectThrowed.position = base.transform.position;
		Vector3 velocity = GrenadeVelocity(point, objectThrowed, Physics.gravity.y, h);
		objectThrowed.velocity = velocity;
	}

	private Vector3 GrenadeVelocity(Vector3 destination, Rigidbody ball, float gravity, float h)
	{
		float num = destination.y - ball.position.y;
		Vector3 vector = new Vector3(destination.x - ball.position.x, 0f, destination.z - ball.position.z);
		float num2 = Mathf.Sqrt(-2f * h / gravity) + Mathf.Sqrt(2f * (num - h) / gravity);
		Vector3 vector2 = Vector3.up * Mathf.Sqrt(-2f * gravity * h);
		return vector / num2 + vector2 * (0f - Mathf.Sign(gravity));
	}

	private Vector3 BallisticVelocity(Vector3 destination, float angle)
	{
		Vector3 vector = destination - base.transform.position;
		float y = vector.y;
		vector.y = 0f;
		float magnitude = vector.magnitude;
		float num = angle * (MathF.PI / 180f);
		vector.y = magnitude * Mathf.Tan(num);
		magnitude += y / Mathf.Tan(num);
		return Mathf.Sqrt(magnitude * Physics.gravity.magnitude / Mathf.Sin(2f * num)) * vector.normalized;
	}

	public void CheckDamageToEnemy(EnemyController enemyController, bool isRange = false, float damage = 0f, Vector3 posObject = default(Vector3))
	{
		LayerMask layerMask = obstacleMask;
		if (isRange)
		{
			layerMask = obstacleMaskExceptWindow;
		}
		if (posObject == Vector3.zero)
		{
			posObject = playerController.weaponPos.position;
		}
		Vector3 normalized = (enemyController.middlePos.position - posObject).normalized;
		float maxDistance = Vector3.Distance(posObject, enemyController.middlePos.position);
		if (!(enemyController.enemyCollider.transform.localScale != Vector3.zero) || Physics.Raycast(playerController.weaponPos.position, normalized, maxDistance, layerMask) || enemyController.network.GetIsHovering() || !(enemyController.enemyCollider.transform.localScale != Vector3.zero))
		{
			return;
		}
		if (enemyController.network.IsNonActive())
		{
			enemyController.Dead(1).Forget();
		}
		else if (enemyController.network.GetHealth() > 0f && !enemyController.isDead)
		{
			EnemyPartPool enemyPartPool = EnemyPartSpawner.Instance.Get();
			enemyPartPool.transform.position = enemyController.bodyTransform.position;
			enemyPartPool.initType(-1);
			if (damage == 0f)
			{
				if (isRange)
				{
					damage = dmgWeapon1;
				}
				else
				{
					damage = ((isHalfMeleeCharging && !isMeleeCharging) ? dmgWeaponHalfCharge0 : ((!isMeleeCharging) ? dmgWeapon0 : dmgWeaponFullCharge0));
					if (enemyController.isDown)
					{
						damage *= 2f;
					}
				}
			}
			if (playerController.IsDoubleDamage)
			{
				damage *= 2f;
			}
			if (isRange)
			{
				enemyController.Hurt(damage, stuntWeapon1, playerController.network.isLocalPlayer, playerController.network.GetIDX(), 1);
			}
			else if (playerController.isAttackMeleeSwing)
			{
				PlaySFX(idWeaponMelee);
				damage *= playerController.PlayerMultiplyStatsData.GetMultiplyMeleeDamage();
				if (BGDatabase_Weapon.GetEntityByKeyid(idWeaponMelee).HeadOff)
				{
					enemyController.Hurt(damage, stuntWeapon0, playerController.network.isLocalPlayer, playerController.network.GetIDX(), 0, isGrenade: false, isHeadOff: true);
				}
				else
				{
					enemyController.Hurt(damage, stuntWeapon0, playerController.network.isLocalPlayer, playerController.network.GetIDX(), 0);
				}
			}
			else if (playerController.isDashing)
			{
				PlaySFX();
				damage = playerController.PlayerMultiplyStatsData.GetDashAttackDamage();
				enemyController.Hurt(damage, stuntWeapon0, playerController.network.isLocalPlayer, playerController.network.GetIDX(), -1);
			}
			if (playerController.network.isLocalPlayer)
			{
				CameraGame.Instance.CameraShake(shakeDurWeapon0, shakeAmplitudeWeapon0);
			}
		}
		else if (enemyController.network.GetHealth() <= 0f && enemyController.enemyCollider.enabled)
		{
			enemyController.Dead(1).Forget();
		}
	}

	public void CheckDamageToOtherPlayer(PlayerController otherPlayerController, bool isRange = false, float damage = 0f, Vector3 posObject = default(Vector3))
	{
		if (posObject == Vector3.zero)
		{
			posObject = playerController.weaponPos.position;
		}
		Vector3 normalized = (otherPlayerController.weaponPos.position - posObject).normalized;
		float maxDistance = Vector3.Distance(posObject, otherPlayerController.weaponPos.position);
		if (Physics.Raycast(playerController.weaponPos.position, normalized, maxDistance, obstacleMask) || !(otherPlayerController.network.GetHealth() > 0f))
		{
			return;
		}
		if (damage == 0f)
		{
			damage = ((!isRange) ? dmgWeapon0 : dmgWeapon1);
		}
		if (playerController.network.isLocalPlayer)
		{
			if (isRange)
			{
				otherPlayerController.network.AddSubHealth((0f - damage) * GameModes.Instance.friendlyFireDmgMultiply);
			}
			else
			{
				if (isHalfMeleeCharging && !isMeleeCharging)
				{
					damage = dmgWeaponHalfCharge0;
				}
				else if (isMeleeCharging)
				{
					damage = dmgWeaponFullCharge0;
				}
				otherPlayerController.network.AddSubHealth((0f - damage) * GameModes.Instance.friendlyFireDmgMultiply);
			}
		}
		otherPlayerController.network.ExecHurtEffect(otherPlayerController.network.GetIDX());
		if (playerController.network.isLocalPlayer && !isRange)
		{
			CameraGame.Instance.CameraShake();
		}
	}

	public bool CheckDamageToBreakableObject(ObjectCollisionBullet objCollision, bool isRange = false, Vector3 hitpos = default(Vector3), float damage = 10f)
	{
		bool result = false;
		if (!objCollision.isDisabled)
		{
			string text = objCollision.typeCollision;
			if (objCollision.typeCollision == "")
			{
				text = objCollision.typeCollisionBullet.ToString();
			}
			Vector3 normalized = (new Vector3(objCollision.transform.position.x, playerController.weaponPos.position.y, objCollision.transform.position.z) - playerController.weaponPos.position).normalized;
			float maxDistance = Vector3.Distance(playerController.weaponPos.position, new Vector3(objCollision.transform.position.x, playerController.weaponPos.position.y, objCollision.transform.position.z));
			if (objCollision.typeCollisionBullet != EnumCollisionBullet.None)
			{
				text = objCollision.typeCollisionBullet.ToString();
			}
			LayerMask layerMask = obstacleMask;
			if (isRange)
			{
				layerMask = obstacleMaskExceptWindow;
				ImpactEffect(hitpos, text);
			}
			else
			{
				Physics.Raycast(playerController.weaponPos.position, normalized, out var hitInfo, maxDistance, 512);
				if (text == "Blood")
				{
					text += "Omni";
				}
				ImpactEffect(hitInfo.point, text);
			}
			if (playerController.network.isLocalPlayer && !Physics.Raycast(playerController.weaponPos.position, normalized, maxDistance, layerMask))
			{
				result = true;
				if (objCollision.destructibleComp != null && objCollision.destructibleComp.currentHitPoints <= 0f && objCollision.activateObject != null)
				{
					objCollision.activateObject.transform.parent = objCollision.transform.parent;
					objCollision.activateObject.SetActive(value: true);
					UniTaskUtil.DelayedCall(this, objCollision.delayDestroy, () =>
					{
						UnityEngine.Object.Destroy(objCollision.activateObject);
					}).Forget();
				}
				if (objCollision.isExplosiveObject)
				{
					if (isRange && MathFunc.Distance(objCollision.transform.position, playerController.transform.position) < 4f)
					{
						CameraGame.Instance.CameraShake(0.7f, 0.7f);
					}
				}
				else
				{
					CameraGame.Instance.CameraShake(0.3f);
				}
				DestructibleObject destructObject = objCollision.destructObject;
				if (objCollision.destructibleComp != null)
				{
					if (!objCollision.isExplosiveObject)
					{
						if (!isRange)
						{
							PlaySFX(idWeaponMelee);
						}
						else if (objCollision.SFXName != "")
						{
							AudioManager.PlaySFXTransform(objCollision.SFXName, objCollision.transform, isLocalPlayerTrigger: false);
						}
						else
						{
							AudioManager.PlaySFXTransform("impactBullet_Metal", objCollision.transform, isLocalPlayerTrigger: false);
						}
						if (destructObject != null && destructObject.Object != null && destructObject.Object.IsValid)
						{
							destructObject.RPCHitObject(playerController.network.GetIDX(), (int)damage);
						}
						else
						{
							objCollision.HitDestructibleObject(damage);
						}
					}
					else
					{
						AudioManager.PlaySFXTransform("impactBullet_Metal", objCollision.transform, isLocalPlayerTrigger: false);
						objCollision.parentObject.GetComponent<MeshRenderer>().material.DOColor(new Color(10f, 0f, 0f), 2.5f);
						objCollision.transform.DOShakeRotation(3f, 2f, 50, 90f, fadeOut: false).SetEase(Ease.InQuint);
						if (isRange)
						{
							GameManager.Instance.ObjectExplosion(objCollision, this);
						}
						if (objCollision.destructObject != null)
						{
							if (objCollision.destructibleComp.currentHitPoints <= 0f)
							{
								objCollision.isDisabled = true;
							}
						}
						else
						{
							objCollision.isDisabled = true;
						}
						if (destructObject != null)
						{
							destructObject.RPCSetExplode(playerController.network.GetIDX());
						}
					}
				}
			}
		}
		return result;
	}

	private void PlaySFX(int idMeleeWeapon = -1)
	{
		if (idMeleeWeapon == -1)
		{
			AudioManager.PlaySFXTransform("player-punch", playerController.object2D.transform, playerController.network.isLocalPlayer);
			return;
		}
		AudioManager.PlaySFXTransform("barricade-hammering", playerController.object2D.transform, playerController.network.isLocalPlayer);
		AudioManager.PlaySFXTransform("melee_" + idMeleeWeapon, playerController.object2D.transform, playerController.network.isLocalPlayer);
	}

	public int GetMagazineSize(bool equipedWeapon, int pidWeapon = 0, int pidBaseWeapon = 0)
	{
		if (equipedWeapon)
		{
			pidWeapon = idWeaponRange;
			pidBaseWeapon = idBaseWeaponRange;
		}
		int num = 0;
		if (pidWeapon > 0)
		{
			BGDatabase_Weapon entityByKeyid = BGDatabase_Weapon.GetEntityByKeyid(pidBaseWeapon);
			if (entityByKeyid != null)
			{
				num = entityByKeyid.MagazineSize;
				BGDatabase_Weapon entityByKeyid2 = BGDatabase_Weapon.GetEntityByKeyid(pidWeapon);
				if (entityByKeyid2.Buff != null)
				{
					foreach (string item in entityByKeyid2.Buff)
					{
						if (item.Contains("Mag"))
						{
							num = ((!item.Contains("%")) ? (num + MathFunc.ExtractNumber(item)) : Mathf.RoundToInt((float)num * ((100f + (float)MathFunc.ExtractNumber(item)) / 100f)));
							break;
						}
					}
				}
			}
		}
		return num;
	}

	public void SyncWeaponLocalVariable()
	{
		idWeaponMelee = playerController.network.GetIdWeapon0();
		idWeaponRange = playerController.network.GetIdWeapon1();
		EquipWeaponID(idWeaponMelee, 0);
		EquipWeaponID(idWeaponRange, 1);
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (!hasFocus && playerController.network.isLocalPlayer)
		{
			if (playerController.isRMBDown)
			{
				Cursor.visible = true;
				playerController.isRMBDown = false;
			}
			else if (playerController.isLMBDown)
			{
				playerController.isLMBDown = false;
				playerController.network.ExecReleaseAttack();
			}
		}
	}
}
