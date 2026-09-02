using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Weapon.Throwable.Landmine;

namespace Toked.Weapon.Throwable;

public class LandmineItem : MonoBehaviour
{
	[SerializeField]
	private PlayerController _playerController;

	[SerializeField]
	private Landmine _landmine;

	[SerializeField]
	private float _delayLandmineActive = 0.3f;

	[SerializeField]
	private float _delayLandmineExplode = 0.3f;

	[SerializeField]
	private DOTweenAnimation _landmineAnimation;

	[SerializeField]
	private string _beepSfx;

	[SerializeField]
	private List<Collider> _listCollider = new List<Collider>();

	private bool _isColliderActive;

	private float _initStartSize;

	public PlayerController PlayerController
	{
		get
		{
			return _playerController;
		}
		set
		{
			_playerController = value;
		}
	}

	public void Init(PlayerController playerController)
	{
		Reset();
		_playerController = playerController;
		base.gameObject.SetActive(value: true);
		UniTaskUtil.DelayedCall(this, _delayLandmineActive, () =>
		{
			foreach (Collider item in _listCollider)
			{
				item.enabled = false;
				item.enabled = true;
			}
			SetActiveCollider(active: true);
		}, ignoreTimeScale: false).Forget();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_isColliderActive && (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("PlayerCollider")) && (bool)_playerController)
		{
			UniTaskUtil.DelayedCall(this, _delayLandmineExplode, Explode, ignoreTimeScale: false).Forget();
			_landmineAnimation.duration = 0.15f;
			_landmineAnimation.onStepComplete.RemoveListener(OnOnStepAnimationAction);
			_landmineAnimation.RewindThenRecreateTweenAndPlay();
			_landmineAnimation.onStepComplete.AddListener(OnOnStepAnimationAction);
			SetActiveCollider(active: false);
		}
	}

	private void Explode()
	{
		ObjectImpactPool objectImpactPool = ImpactSpawner.Instance.Get();
		Vector3 position = base.transform.position;
		objectImpactPool.transform.position = new Vector3(position.x, 0f, position.z);
		objectImpactPool.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		objectImpactPool.typeImpact = ObjectImpactPool.ImpactType.Grenade;
		objectImpactPool.initType();
		if (MathFunc.Distance(objectImpactPool.transform.position, _playerController.transform.position) < 10f)
		{
			CameraGame.Instance.CameraShake(_landmine.WeaponData.DurShake, _landmine.WeaponData.DurShake);
		}
		GameManager.Instance.CheckModifierExplosionCallHorde();
		if (_playerController.network.isLocalPlayer)
		{
			CheckExplosionDamage(_playerController, position);
		}
		AudioManager.PlaySFXTransform("grenade", objectImpactPool.transform, isLocalPlayerTrigger: false);
		_playerController.weaponController.CheckEnemyAggro(objectImpactPool.transform);
		Release();
	}

	private void Release()
	{
		Reset(isRelease: true);
		LandmineSpawner.Instance.Release(this);
	}

	private void Reset(bool isRelease = false)
	{
		SetActiveCollider(active: false);
		base.gameObject.SetActive(value: false);
		_landmineAnimation.onStepComplete.RemoveListener(OnOnStepAnimationAction);
		_landmineAnimation.DOKill();
		if (!isRelease)
		{
			_landmineAnimation.duration = 1f;
			_landmineAnimation.RewindThenRecreateTweenAndPlay();
			_landmineAnimation.onStepComplete.AddListener(OnOnStepAnimationAction);
		}
	}

	private void SetActiveCollider(bool active)
	{
		_isColliderActive = active;
	}

	private void CheckExplosionDamage(PlayerController playerController, Vector3 objPosition)
	{
		foreach (EnemyController item in GameManager.Instance.arrEnemyController)
		{
			if (!item.animatorState.HasParam("IsEscapeDanger") && MathFunc.Distance(item.transform.position, objPosition) < _landmine.WeaponData.ImpactAoESize && item.network.GetHealth() > 0f && !item.isDead && item.GetCurrentStateHash() != AnimatorHashManager.HoveringHash)
			{
				Vector3 position = item.middlePos.position;
				Vector3 vector = new Vector3(objPosition.x, position.y, objPosition.z);
				Vector3 normalized = (position - vector).normalized;
				float maxDistance = Vector3.Distance(vector, position);
				if (!Physics.Raycast(vector, normalized, maxDistance, _landmine.ObstacleMask))
				{
					item.Hurt(_landmine.WeaponData.Damage, _landmine.WeaponData.StuntTime, playerController.network.isLocalPlayer, playerController.network.GetIDX(), 2, isGrenade: true);
				}
			}
		}
		if (GameModes.Instance.isGrenadeFriendlyFire)
		{
			foreach (PlayerController item2 in NetworkGameManager.Instance.arrPlayerController)
			{
				if (!(MathFunc.Distance(item2.transform.position, objPosition) < _landmine.WeaponData.ImpactAoESize))
				{
					continue;
				}
				Vector3 position2 = item2.weaponPos.position;
				Vector3 vector2 = new Vector3(objPosition.x, position2.y, objPosition.z);
				Vector3 normalized2 = (position2 - vector2).normalized;
				float maxDistance2 = Vector3.Distance(vector2, position2);
				if (!Physics.Raycast(vector2, normalized2, maxDistance2, _landmine.ObstacleMask))
				{
					item2.network.ExecHurtEffect(item2.network.GetIDX());
					if (playerController.network.isLocalPlayer)
					{
						item2.network.AddSubHealth((float)(int)(0f - _landmine.WeaponData.Damage / 3f) * item2.PlayerMultiplyStatsData.GetMultiplyDamageExplosion());
					}
				}
			}
		}
		foreach (DestructibleObject item3 in GameManager.Instance.arrDestructibleObject)
		{
			if ((bool)item3 && MathFunc.Distance(item3.transform.position, objPosition) < _landmine.WeaponData.ImpactAoESize)
			{
				playerController.weaponController.CheckDamageToBreakableObject(item3.GetComponent<ObjectCollisionBullet>(), isRange: true, default, _landmine.WeaponData.Damage);
			}
		}
	}

	public void OnOnStepAnimationAction()
	{
		if (!string.IsNullOrWhiteSpace(_beepSfx))
		{
			AudioManager.PlaySFXTransform(_beepSfx, base.transform, isLocalPlayerTrigger: false);
		}
	}
}
