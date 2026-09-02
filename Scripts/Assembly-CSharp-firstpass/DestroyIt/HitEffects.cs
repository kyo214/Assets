using System.Collections.Generic;
using UnityEngine;

namespace DestroyIt;

[DisallowMultipleComponent]
public class HitEffects : MonoBehaviour
{
	public List<HitEffect> effects;

	public void PlayEffect(HitBy weaponType, Vector3 hitPoint, Vector3 hitNormal)
	{
		GameObject gameObject = null;
		foreach (HitEffect effect in effects)
		{
			if ((effect.hitBy & weaponType) > (HitBy)0)
			{
				gameObject = effect.effect;
				break;
			}
		}
		if (gameObject != null)
		{
			ObjectPool.Instance.Spawn(gameObject, hitPoint, Quaternion.LookRotation(hitNormal));
		}
	}
}
