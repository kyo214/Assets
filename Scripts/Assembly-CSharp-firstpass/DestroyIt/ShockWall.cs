using UnityEngine;

namespace DestroyIt;

public class ShockWall : MonoBehaviour
{
	public float blastForce = 200f;

	public float damageAmount = 200f;

	public Vector3 origin;

	private void OnTriggerEnter(Collider col)
	{
		Rigidbody attachedRigidbody = col.attachedRigidbody;
		if (attachedRigidbody != null && !attachedRigidbody.isKinematic)
		{
			attachedRigidbody.AddExplosionForce(blastForce, origin, 0f, 0.5f);
		}
		ChipAwayDebris component = col.gameObject.GetComponent<ChipAwayDebris>();
		if (component != null)
		{
			if (Random.Range(1, 100) > 50)
			{
				component.BreakOff(blastForce, 0f, 0.5f);
			}
			return;
		}
		Destructible[] componentsInParent = col.gameObject.GetComponentsInParent<Destructible>(includeInactive: false);
		foreach (Destructible destructible in componentsInParent)
		{
			if (destructible.isActiveAndEnabled || destructible.isTerrainTree)
			{
				ExplosiveDamage damage = new ExplosiveDamage
				{
					DamageAmount = damageAmount,
					BlastForce = blastForce,
					Position = origin,
					Radius = 0f,
					UpwardModifier = 0.5f
				};
				destructible.ApplyDamage(damage);
				break;
			}
		}
	}
}
