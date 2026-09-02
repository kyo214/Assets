using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Toked.Weapon.Throwable;

[CreateAssetMenu(fileName = "Grenade", menuName = "WMO/ScriptableObjects/Weapons/Grenade", order = 1)]
public class Grenade : ThrowableWeapon
{
	[SerializeField]
	protected float _timeFactor = 0.3f;

	[Tooltip("| k (multiplier) | Hasil                        |\n| -------------- | ---------------------------- |\n| 0.1            | sangat datar (kayak peluru)  |\n| 0.3 – 0.5      | natural (lemparan biasa) ✅   |\n| 0.7            | tinggi (arc jelas)           |\n| 1.0+           | sangat tinggi (kayak mortar) |\n")]
	[SerializeField]
	protected float _arcProjectory = 0.6f;

	[SerializeField]
	protected float _explodeTime = 1.5f;

	[SerializeField]
	protected ObjectImpactPool.ImpactType _impactType = ObjectImpactPool.ImpactType.Grenade;

	[SerializeField]
	protected LayerMask obstacleMask;

	protected override void OnThrow(PlayerController playerController, ObjectThrowable objectThrow, Vector3 targetPosition)
	{
		Vector3 position = playerController.weaponPos.position;
		objectThrow.transform.position = position;
		objectThrow.rigidBody.DOMove(position, 0f);
		FireCannonAtPoint(playerController, targetPosition, objectThrow.rigidBody);
		UniTaskUtil.DelayedCall(_explodeTime, () =>
		{
			OnThrew(playerController, objectThrow, targetPosition);
		}).Forget();
	}

	protected void FireCannonAtPoint(PlayerController playerController, Vector3 point, Rigidbody objectThrow)
	{
		objectThrow.rotation = Quaternion.identity;
		objectThrow.position = playerController.weaponPos.position;
		Vector3 velocity = CalculateItemVelocity(point, objectThrow.position);
		objectThrow.velocity = velocity;
	}

	protected override Vector3 CalculateItemVelocity(Vector3 destination, Vector3 currentPosition)
	{
		return MathFunc.CalculateParabolicVelocity(destination, currentPosition, _arcProjectory, _timeFactor);
	}

	protected override void OnThrew(PlayerController playerController, ObjectThrowable objectThrow, Vector3 targetPosition)
	{
		Vector3 position = objectThrow.transform.position;
		GrenadeExplode(playerController, position);
		ThrowableSpawner.Instance.Release(objectThrow);
	}

	protected void GrenadeExplode(PlayerController playerController, Vector3 posObj)
	{
		Debug.Log("Explode");
		ObjectImpactPool objectImpactPool = ImpactSpawner.Instance.Get();
		objectImpactPool.transform.position = new Vector3(posObj.x, 0f, posObj.z);
		objectImpactPool.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		objectImpactPool.typeImpact = _impactType;
		objectImpactPool.initType();
		if (MathFunc.Distance(objectImpactPool.transform.position, playerController.transform.position) < 11f)
		{
			CameraGame.Instance.CameraShake(_weaponData.DurShake, _weaponData.DurShake);
		}
		GameManager.Instance.CheckModifierExplosionCallHorde();
		if (playerController.network.isLocalPlayer)
		{
			if (playerController.data.idThrowable != -1)
			{
				playerController.canGrenade = true;
			}
			CheckExplosionDamage(playerController, posObj);
		}
		else
		{
			playerController.canGrenade = true;
		}
		AudioManager.PlaySFXTransform("grenade", objectImpactPool.transform, isLocalPlayerTrigger: false);
		UIGameManager.Instance.cursorGrenade.gameObject.SetActive(value: false);
		playerController.weaponController.CheckEnemyAggro(objectImpactPool.transform);
	}

	protected void CheckExplosionDamage(PlayerController playerController, Vector3 objPosition)
	{
		foreach (EnemyController item in GameManager.Instance.arrEnemyController)
		{
			if (!item.animatorState.HasParam("IsEscapeDanger") && MathFunc.Distance(item.transform.position, objPosition) < _weaponData.ImpactAoESize)
			{
				playerController.weaponController.CheckDamageToEnemy(item, isRange: true, _weaponData.Damage, objPosition);
			}
		}
		if (GameModes.Instance.isGrenadeFriendlyFire)
		{
			foreach (PlayerController item2 in NetworkGameManager.Instance.arrPlayerController)
			{
				if (MathFunc.Distance(item2.transform.position, objPosition) < _weaponData.ImpactAoESize / 2f)
				{
					playerController.weaponController.CheckDamageToOtherPlayer(item2, isRange: true, _weaponData.Damage * item2.PlayerMultiplyStatsData.GetMultiplyDamageExplosion(), objPosition);
				}
			}
		}
		foreach (DestructibleObject item3 in GameManager.Instance.arrDestructibleObject)
		{
			if ((bool)item3 && MathFunc.Distance(item3.transform.position, objPosition) < _weaponData.ImpactAoESize)
			{
				playerController.weaponController.CheckDamageToBreakableObject(item3.GetComponent<ObjectCollisionBullet>(), isRange: true, default, _weaponData.Damage);
			}
		}
	}
}
