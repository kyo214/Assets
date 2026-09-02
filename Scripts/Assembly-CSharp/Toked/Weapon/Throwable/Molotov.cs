using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Toked.Weapon.Throwable;

[CreateAssetMenu(fileName = "Molotov", menuName = "WMO/ScriptableObjects/Weapons/Molotov", order = 1)]
public class Molotov : ThrowableWeapon
{
	[SerializeField]
	protected float _timeFactor = 0.3f;

	[Tooltip("| k (multiplier) | Hasil                        |\n| -------------- | ---------------------------- |\n| 0.1            | sangat datar (kayak peluru)  |\n| 0.3 – 0.5      | natural (lemparan biasa) ✅   |\n| 0.7            | tinggi (arc jelas)           |\n| 1.0+           | sangat tinggi (kayak mortar) |\n")]
	[SerializeField]
	protected float _arcProjectory = 0.6f;

	[SerializeField]
	protected float _fireDuration = 2f;

	public float FireDuration
	{
		get
		{
			return _fireDuration;
		}
		set
		{
			_fireDuration = value;
		}
	}

	protected override void OnThrow(PlayerController playerController, ObjectThrowable objectThrow, Vector3 targetPosition)
	{
		Vector3 position = playerController.weaponPos.position;
		objectThrow.transform.position = position;
		objectThrow.rigidBody.DOMove(position, 0f);
		FireCannonAtPoint(playerController, targetPosition, objectThrow);
	}

	protected void FireCannonAtPoint(PlayerController playerController, Vector3 point, ObjectThrowable objectThrow)
	{
		Rigidbody rigidBody = objectThrow.rigidBody;
		rigidBody.rotation = Quaternion.identity;
		rigidBody.position = playerController.weaponPos.position;
		Vector3 velocity = CalculateItemVelocity(point, rigidBody.position);
		float num = CalculateExplodeTime(velocity, point, rigidBody.position);
		objectThrow.transform.DOLocalRotate(new Vector3(playerController.angleGround.x, 0f, 0f), num, RotateMode.FastBeyond360);
		rigidBody.velocity = velocity;
		UniTaskUtil.DelayedCall(num, () =>
		{
			OnThrew(playerController, objectThrow, point);
		}).Forget();
	}

	protected float CalculateExplodeTime(Vector3 velocity, Vector3 destination, Vector3 currentPosition)
	{
		float num = 0.5f * Physics.gravity.y;
		float y = velocity.y;
		float num2 = currentPosition.y - destination.y;
		float f = y * y - 4f * num * num2;
		float a = (0f - y + Mathf.Sqrt(f)) / (2f * num);
		float b = (0f - y - Mathf.Sqrt(f)) / (2f * num);
		return Mathf.Max(a, b);
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
		AreaImpactItem item = FireSpawner.Instance.GetItem(playerController, posObj, _fireDuration, _weaponData.Damage);
		if (MathFunc.Distance(item.transform.position, playerController.transform.position) < 10f)
		{
			CameraGame.Instance.CameraShake(_weaponData.DurShake, _weaponData.AmplitudeShake);
		}
		if (playerController.network.isLocalPlayer)
		{
			if (playerController.data.idThrowable != -1)
			{
				playerController.canGrenade = true;
			}
		}
		else
		{
			playerController.canGrenade = true;
		}
		AudioManager.PlaySFXTransform("impactBullet_Glass", item.transform, isLocalPlayerTrigger: false);
		UIGameManager.Instance.cursorGrenade.gameObject.SetActive(value: false);
		playerController.weaponController.CheckEnemyAggro(item.transform);
	}
}
