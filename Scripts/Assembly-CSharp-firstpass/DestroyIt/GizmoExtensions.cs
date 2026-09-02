using System.Collections.Generic;
using UnityEngine;

namespace DestroyIt;

public static class GizmoExtensions
{
	public static void DrawGizmos(this List<DamageEffect> damageEffects, Transform transform)
	{
		if (damageEffects == null)
		{
			return;
		}
		foreach (DamageEffect damageEffect in damageEffects)
		{
			if (damageEffect != null)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawWireCube(transform.TransformPoint(damageEffect.Offset), new Vector3(0.1f, 0.1f, 0.1f));
				Quaternion quaternion = transform.rotation * Quaternion.Euler(damageEffect.Rotation);
				Gizmos.DrawRay(transform.TransformPoint(damageEffect.Offset), quaternion * Vector3.forward * 0.5f);
			}
		}
	}

	public static void DrawGizmos(this Vector3 centerPointOverride, Transform transform)
	{
		if (!(centerPointOverride == Vector3.zero))
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(transform.TransformPoint(centerPointOverride), 0.1f);
		}
	}
}
