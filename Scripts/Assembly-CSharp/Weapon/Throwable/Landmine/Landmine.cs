using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Toked.Weapon.Throwable;
using UnityEngine;

namespace Weapon.Throwable.Landmine;

[CreateAssetMenu(fileName = "Landmine", menuName = "WMO/ScriptableObjects/Weapons/Landmine", order = 1)]
public class Landmine : ThrowableWeapon
{
	[SerializeField]
	protected float _explodeDelayTime = 1f;

	[SerializeField]
	protected LayerMask _obstacleMask;

	[SerializeField]
	protected float _offsetDropPosition = 1.2f;

	public LayerMask ObstacleMask => _obstacleMask;

	protected override void OnThrow(PlayerController playerController, ObjectThrowable objectThrow, Vector3 targetPosition)
	{
		Vector3 position = playerController.weaponPos.position + GetOffset(playerController);
		objectThrow.transform.position = playerController.weaponPos.position;
		objectThrow.transform.DOMove(position, 0.3f);
		UniTaskUtil.DelayedCall(_explodeDelayTime, () =>
		{
			OnThrew(playerController, objectThrow, position);
		}).Forget();
	}

	protected override void OnThrew(PlayerController playerController, ObjectThrowable objectThrow, Vector3 targetPosition)
	{
		LandmineSpawner.Instance.GetItem(playerController, objectThrow.transform.position);
		ThrowableSpawner.Instance.Release(objectThrow);
		SetPlayerState(playerController);
	}

	private void SetPlayerState(PlayerController playerController)
	{
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
		UIGameManager.Instance.cursorGrenade.gameObject.SetActive(value: false);
	}

	private Vector3 GetOffset(PlayerController playerController)
	{
		float num = playerController.network.GetAngleInputNetwork();
		return new Vector3(Mathf.Sin(MathF.PI / 180f * num), 0f, Mathf.Cos(MathF.PI / 180f * num)).normalized * _offsetDropPosition;
	}
}
