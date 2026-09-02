using System.Collections.Generic;
using UnityEngine;

namespace DestroyIt;

public class MeleeArea : MonoBehaviour
{
	public int damageAmount = 30;

	public int repairAmount = 20;

	public float meleeRadius = 1.3f;

	public float additionalForceAmount = 150f;

	public float additionalForceRadius = 2f;

	public ParticleSystem repairEffect;

	public void OnMeleeDamage()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, meleeRadius);
		List<Destructible> list = new List<Destructible>();
		bool flag = false;
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (collider is TerrainCollider || collider.isTrigger || (collider is CharacterController && collider.tag == "Player"))
			{
				continue;
			}
			if (!flag)
			{
				HitEffects componentInParent = collider.gameObject.GetComponentInParent<HitEffects>();
				if (componentInParent != null && componentInParent.effects.Count > 0)
				{
					componentInParent.PlayEffect(HitBy.Axe, base.transform.position, base.transform.forward * -1f);
				}
				flag = true;
			}
			Rigidbody attachedRigidbody = collider.attachedRigidbody;
			if (attachedRigidbody != null)
			{
				attachedRigidbody.AddForceAtPosition(base.transform.forward * 3f, base.transform.position, ForceMode.Impulse);
			}
			Destructible[] componentsInParent = collider.gameObject.GetComponentsInParent<Destructible>(includeInactive: false);
			foreach (Destructible destructible in componentsInParent)
			{
				if (!list.Contains(destructible) && (destructible.isActiveAndEnabled || destructible.isTerrainTree))
				{
					list.Add(destructible);
					ImpactDamage damage = new ImpactDamage
					{
						DamageAmount = damageAmount,
						AdditionalForce = additionalForceAmount,
						AdditionalForcePosition = base.transform.position,
						AdditionalForceRadius = additionalForceRadius
					};
					destructible.ApplyDamage(damage);
				}
			}
		}
	}

	private void OnMeleeRepair()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, meleeRadius);
		List<Destructible> list = new List<Destructible>();
		bool flag = false;
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (collider is TerrainCollider || collider.isTrigger || (collider is CharacterController && collider.tag == "Player"))
			{
				continue;
			}
			Destructible componentInParent = collider.gameObject.GetComponentInParent<Destructible>();
			if (componentInParent != null && !list.Contains(componentInParent) && componentInParent.currentHitPoints < componentInParent.totalHitPoints && componentInParent.canBeRepaired)
			{
				list.Add(componentInParent);
				componentInParent.RepairDamage(repairAmount);
				if (repairEffect != null && !flag)
				{
					repairEffect.GetComponent<ParticleSystem>().Clear(withChildren: true);
					repairEffect.Play(withChildren: true);
					flag = true;
				}
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(base.transform.position, meleeRadius);
	}
}
