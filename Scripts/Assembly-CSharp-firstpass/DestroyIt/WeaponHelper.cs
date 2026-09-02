using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DestroyIt;

public static class WeaponHelper
{
	public static void Launch(GameObject weaponPrefab, Transform weaponLauncher, float startDistance, float initialVelocity, bool randomRotation)
	{
		Quaternion rotation = (randomRotation ? UnityEngine.Random.rotation : weaponLauncher.rotation);
		Vector3 position = weaponLauncher.TransformPoint(Vector3.forward * startDistance);
		Rigidbody component = ObjectPool.Instance.Spawn(weaponPrefab, position, rotation).GetComponent<Rigidbody>();
		if (component != null)
		{
			component.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
			component.velocity = Vector3.zero;
			if (initialVelocity > 0f)
			{
				component.AddForce(weaponLauncher.forward * initialVelocity, ForceMode.Impulse);
			}
		}
	}

	public static void Launch(GameObject weaponPrefab, Transform weaponLauncher, float startDistance, bool randomRotation)
	{
		Quaternion rotation = (randomRotation ? UnityEngine.Random.rotation : weaponLauncher.rotation);
		Vector3 position = weaponLauncher.TransformPoint(Vector3.forward * startDistance);
		ObjectPool.Instance.Spawn(weaponPrefab, position, rotation);
	}

	public static WeaponType GetNext(WeaponType currentWeaponType)
	{
		List<WeaponType> list = Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>().ToList();
		int num = (int)currentWeaponType;
		num = ((num != list.Count - 1) ? (num + 1) : 0);
		return list[num];
	}

	public static WeaponType GetPrevious(WeaponType currentWeaponType)
	{
		List<WeaponType> list = Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>().ToList();
		int num = (int)currentWeaponType;
		num = ((num != 0) ? (num - 1) : (list.Count - 1));
		return list[num];
	}
}
