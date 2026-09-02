using System.Collections.Generic;
using Toked;
using UnityEngine;

public class GLauncherBulletControl : MonoBehaviour
{
	[SerializeField]
	private PlayerController playerController;

	[SerializeField]
	private List<ObjectThrowable> ListBullet = new List<ObjectThrowable>();

	[SerializeField]
	private LayerMask layerColliderMask;

	private const float MIN_VELOCITY_BULLET = 5f;

	private const float MAX_VELOCITY_BULLET = 20f;

	private void Start()
	{
		InvokeRepeating("CheckBullet", 0f, 0.1f);
	}

	private void CheckBullet()
	{
		if (ListBullet.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < ListBullet.Count; i++)
		{
			if ((bool)ListBullet[i] && Physics.OverlapSphere(ListBullet[i].transform.position, 0.4f, layerColliderMask).Length != 0)
			{
				GLauncherExplode(ListBullet[i]);
				i--;
			}
		}
	}

	public void ExecuteGrenadeLauncher(Vector3 direction)
	{
		ObjectThrowable objectThrowable = ThrowableSpawner.Instance.Get(ObjectThrowable.ThrowableType.GLauncher);
		ListBullet.Add(objectThrowable);
		objectThrowable.transform.position = new Vector3(playerController.weaponPosSprite.position.x, playerController.weaponPos.position.y - 0.2f, playerController.weaponPosSprite.position.z);
		objectThrowable.transform.rotation = Quaternion.LookRotation(direction);
		float num = MathFunc.Distance(playerController.angleInput, playerController.weaponPos.position) * 2.5f;
		if (num < 5f)
		{
			num = 5f;
		}
		else if (num > 20f)
		{
			num = 20f;
		}
		objectThrowable.rigidBody.velocity = new Vector3(direction.x, 0f, direction.z).normalized * num;
	}

	protected void GLauncherExplode(ObjectThrowable objThrow)
	{
		ObjectImpactPool objectImpactPool = ImpactSpawner.Instance.Get();
		objectImpactPool.transform.position = new Vector3(objThrow.transform.position.x, objThrow.transform.position.y, objThrow.transform.position.z);
		objectImpactPool.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
		objectImpactPool.typeImpact = ObjectImpactPool.ImpactType.GLauncher;
		objectImpactPool.initType();
		if (MathFunc.Distance(objectImpactPool.transform.position, playerController.transform.position) < 10f)
		{
			CameraGame.Instance.CameraShake(0.7f, 0.7f);
		}
		if (playerController.network.isLocalPlayer)
		{
			if (playerController.data.idThrowable != -1)
			{
				playerController.canGrenade = true;
			}
			CheckGLauncherExplosionDamage(objThrow.transform.position);
		}
		else
		{
			playerController.canGrenade = true;
		}
		GameManager.Instance.CheckModifierExplosionCallHorde();
		AudioManager.PlaySFXTransform("grenade", objectImpactPool.transform, isLocalPlayerTrigger: false);
		UIGameManager.Instance.cursorGrenade.gameObject.SetActive(value: false);
		playerController.weaponController.CheckEnemyAggro(objectImpactPool.transform);
		ListBullet.Remove(objThrow);
		ThrowableSpawner.Instance.Release(objThrow);
	}

	protected void CheckGLauncherExplosionDamage(Vector3 objPosition)
	{
		foreach (EnemyController item in GameManager.Instance.arrEnemyController)
		{
			if (MathFunc.Distance(item.transform.position, objPosition) < BGDatabase_Weapon.GetEntityByKeyid(26).ImpactAoESize)
			{
				playerController.weaponController.CheckDamageToEnemy(item, isRange: true, BGDatabase_Weapon.GetEntityByKeyid(26).Damage, new Vector3(objPosition.x, item.transform.position.y, objPosition.z));
			}
		}
		if (GameModes.Instance.friendlyFire)
		{
			foreach (PlayerController item2 in NetworkGameManager.Instance.arrPlayerController)
			{
				if (MathFunc.Distance(item2.transform.position, objPosition) < 2f)
				{
					playerController.weaponController.CheckDamageToOtherPlayer(item2, isRange: true, 20f * item2.PlayerMultiplyStatsData.GetMultiplyDamageExplosion(), objPosition);
				}
			}
		}
		foreach (DestructibleObject item3 in GameManager.Instance.arrDestructibleObject)
		{
			if ((bool)item3 && MathFunc.Distance(item3.transform.position, objPosition) < 2f)
			{
				playerController.weaponController.CheckDamageToBreakableObject(item3.GetComponent<ObjectCollisionBullet>(), isRange: true, default, 120f);
			}
		}
	}
}
