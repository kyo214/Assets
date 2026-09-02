using UnityEngine;

namespace DestroyIt;

public static class ExplosionHelper
{
	public static bool IsExposedToBlast(this GameObject gameObj, ExplosiveDamage explosion)
	{
		if (Physics.Raycast(explosion.Position, gameObj.transform.position - explosion.Position, out var hitInfo, explosion.Radius))
		{
			Collider[] componentsInChildren = gameObj.GetComponentsInChildren<Collider>();
			foreach (Collider collider in componentsInChildren)
			{
				if (hitInfo.collider == collider)
				{
					return true;
				}
			}
		}
		return false;
	}

	public static void ApplyForcesToDebris<T>(GameObject destroyedObj, float velocityReduction, T damageInfo)
	{
		if (destroyedObj == null)
		{
			return;
		}
		Rigidbody[] componentsInChildren = destroyedObj.GetComponentsInChildren<Rigidbody>();
		if (damageInfo.GetType() == typeof(ExplosiveDamage))
		{
			ExplosiveDamage explosiveDamage = damageInfo as ExplosiveDamage;
			Rigidbody[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AddExplosionForce(explosiveDamage.BlastForce, explosiveDamage.Position, explosiveDamage.Radius, explosiveDamage.UpwardModifier);
			}
		}
		if (damageInfo.GetType() == typeof(ImpactDamage) && damageInfo is ImpactDamage { AdditionalForce: >0f } impactDamage)
		{
			Rigidbody[] array = componentsInChildren;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].AddExplosionForce(impactDamage.AdditionalForce, impactDamage.AdditionalForcePosition, impactDamage.AdditionalForceRadius, 0f);
			}
		}
	}
}
