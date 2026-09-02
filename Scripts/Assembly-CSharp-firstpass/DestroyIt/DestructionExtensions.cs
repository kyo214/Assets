using System.Collections.Generic;
using UnityEngine;

namespace DestroyIt;

public static class DestructionExtensions
{
	public static void Update(this List<float> models, int withinSeconds)
	{
		bool flag = false;
		if (models.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < models.Count; i++)
		{
			if (Time.time > models[i] + (float)withinSeconds)
			{
				models.Remove(models[i]);
				flag = true;
			}
		}
		if (flag)
		{
			DestructionManager.Instance.FireDestroyedPrefabCounterChangedEvent();
		}
	}

	public static void ReleaseClingingDebris(this Destructible destroyedObj)
	{
		List<Transform> list = new List<Transform>();
		TagIt[] componentsInChildren = destroyedObj.GetComponentsInChildren<TagIt>();
		if (componentsInChildren == null)
		{
			return;
		}
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			for (int j = 0; j < componentsInChildren[i].tags.Count; j++)
			{
				if (componentsInChildren[i].tags[j] == Tag.ClingingDebris)
				{
					list.Add(componentsInChildren[i].transform);
				}
			}
		}
		for (int k = 0; k < list.Count; k++)
		{
			list[k].gameObject.AddComponent<Rigidbody>();
		}
	}

	public static void MakeDebrisCling(this GameObject destroyedObj)
	{
		ClingPoint[] componentsInChildren = destroyedObj.GetComponentsInChildren<ClingPoint>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Rigidbody component = componentsInChildren[i].transform.parent.GetComponent<Rigidbody>();
			if (!(component == null) && (componentsInChildren[i].chanceToCling >= 100 || Random.Range(1, 100) <= componentsInChildren[i].chanceToCling) && Physics.Raycast(new Ray(componentsInChildren[i].transform.position - componentsInChildren[i].transform.forward * 0.025f, componentsInChildren[i].transform.forward), out var hitInfo, 0.075f) && !hitInfo.collider.isTrigger)
			{
				component.transform.parent = hitInfo.collider.gameObject.transform;
				if ((bool)component.gameObject.GetComponent<Destructible>() && !hitInfo.collider.gameObject.GetComponent<DestructibleParent>())
				{
					hitInfo.collider.gameObject.AddComponent<DestructibleParent>();
				}
				Destructible componentInParent = hitInfo.collider.gameObject.GetComponentInParent<Destructible>();
				if (componentInParent != null)
				{
					componentInParent.unparentOnDestroy.Add(component.gameObject);
					DelayedRigidbody delayedRigidbody = component.gameObject.AddComponent<DelayedRigidbody>();
					delayedRigidbody.mass = component.mass;
					delayedRigidbody.drag = component.drag;
					delayedRigidbody.angularDrag = component.angularDrag;
				}
				ClingPoint[] componentsInChildren2 = component.gameObject.GetComponentsInChildren<ClingPoint>();
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					Object.Destroy(componentsInChildren2[j].gameObject);
				}
				component.gameObject.RemoveAllFromChildren<Rigidbody>();
			}
		}
	}

	public static void ProcessDestructibleCollision(this Destructible destructibleObj, Collision collision, Rigidbody collidingRigidbody)
	{
		if (!(collidingRigidbody == null) && !destructibleObj.IsDestroyed && !(collision.relativeVelocity.magnitude < destructibleObj.ignoreCollisionsUnder) && collision.contacts.Length != 0)
		{
			Rigidbody attachedRigidbody = collision.contacts[0].otherCollider.attachedRigidbody;
			float f;
			if (attachedRigidbody != null)
			{
				float num = (attachedRigidbody.mass + collidingRigidbody.mass) / 2f;
				f = Vector3.Dot(collision.contacts[0].normal, collision.relativeVelocity) * num;
			}
			else
			{
				f = Vector3.Dot(collision.contacts[0].normal, collision.relativeVelocity) * collidingRigidbody.mass;
			}
			f = Mathf.Abs(f);
			if (f > 1f)
			{
				ImpactDamage damage = new ImpactDamage
				{
					ImpactObject = attachedRigidbody,
					DamageAmount = (int)f,
					ImpactObjectVelocityFrom = collision.relativeVelocity * -1f
				};
				destructibleObj.ApplyDamage(damage);
			}
		}
	}

	public static void CalculateDamageLevels(this List<DamageLevel> damageLevels, float maxHitPoints)
	{
		if (maxHitPoints <= 0f || damageLevels == null || damageLevels.Count == 0)
		{
			return;
		}
		int num = -1;
		for (int i = 0; i < damageLevels.Count; i++)
		{
			if (damageLevels[i] == null)
			{
				continue;
			}
			if (damageLevels[i].healthPercent <= 0)
			{
				damageLevels[i].hasError = true;
				continue;
			}
			if (num > -1 && damageLevels[i].healthPercent >= num)
			{
				damageLevels[i].hasError = true;
				num = damageLevels[i].healthPercent;
				continue;
			}
			damageLevels[i].hasError = false;
			if (i == 0)
			{
				damageLevels[i].maxHitPoints = maxHitPoints;
			}
			else
			{
				damageLevels[i].maxHitPoints = Mathf.RoundToInt(maxHitPoints * (0.01f * (float)damageLevels[i].healthPercent));
				damageLevels[i - 1].minHitPoints = Mathf.RoundToInt(maxHitPoints * (0.01f * (float)damageLevels[i].healthPercent)) + 1;
			}
			if (i == damageLevels.Count - 1)
			{
				damageLevels[i].minHitPoints = 0f;
			}
			num = damageLevels[i].healthPercent;
		}
	}

	public static DamageLevel GetDamageLevel(this List<DamageLevel> damageLevels, float hitPoints)
	{
		if (damageLevels == null || damageLevels.Count == 0)
		{
			return null;
		}
		if (hitPoints <= 0f)
		{
			return damageLevels[damageLevels.Count - 1];
		}
		for (int i = 0; i < damageLevels.Count; i++)
		{
			if (damageLevels[i].maxHitPoints >= hitPoints && damageLevels[i].minHitPoints <= hitPoints)
			{
				return damageLevels[i];
			}
		}
		return null;
	}

	public static void ReparentChildren(this Destructible destObj, GameObject newObj)
	{
		if (destObj.childrenToReParentByName == null || destObj.childrenToReParentByName.Count <= 0)
		{
			return;
		}
		foreach (string item in destObj.childrenToReParentByName)
		{
			Transform transform = destObj.transform.Find(item);
			if (transform != null)
			{
				transform.SetParent(newObj.transform);
			}
		}
	}

	public static void SetActiveOrInactive(this Destructible destObj, DestructionManager destructionManager)
	{
		if (!destObj.isTerrainTree)
		{
			if (destructionManager.autoDeactivateDestructibles)
			{
				destObj.enabled = false;
			}
		}
		else if (destructionManager.autoDeactivateDestructibleTerrainObjects)
		{
			destObj.enabled = false;
		}
	}
}
