using UnityEngine;

namespace DestroyIt;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyProjectile : MonoBehaviour
{
	public HitBy weaponType = HitBy.Cannonball;

	[Tooltip("Impact velocity must be at least this amount to be detected as a hit.")]
	public float minHitVelocity = 10f;

	private Rigidbody rbody;

	private Vector3 lastVelocity;

	public void OnEnable()
	{
		rbody = GetComponent<Rigidbody>();
	}

	public void FixedUpdate()
	{
		lastVelocity = rbody.velocity;
	}

	public void OnCollisionEnter(Collision collision)
	{
		if (collision.relativeVelocity.magnitude < minHitVelocity || collision.contacts.Length == 0)
		{
			return;
		}
		Collider otherCollider = collision.contacts[0].otherCollider;
		HitEffects componentInParent = otherCollider.gameObject.GetComponentInParent<HitEffects>();
		if (componentInParent != null && componentInParent.effects.Count > 0)
		{
			componentInParent.PlayEffect(weaponType, collision.contacts[0].point, collision.contacts[0].normal);
		}
		Destructible[] componentsInParent = otherCollider.gameObject.GetComponentsInParent<Destructible>(includeInactive: false);
		foreach (Destructible destructible in componentsInParent)
		{
			if ((destructible.isActiveAndEnabled || destructible.isTerrainTree) && !(destructible.GetComponentInParent<DestructibleParent>() != null) && (otherCollider.attachedRigidbody == null || otherCollider.attachedRigidbody.GetComponent<Destructible>() == null) && collision.relativeVelocity.magnitude >= destructible.ignoreCollisionsUnder)
			{
				destructible.ProcessDestructibleCollision(collision, base.gameObject.GetComponent<Rigidbody>());
				rbody.velocity = lastVelocity;
				break;
			}
		}
		ChipAwayDebris component = collision.contacts[0].otherCollider.gameObject.GetComponent<ChipAwayDebris>();
		if (component != null)
		{
			component.BreakOff(collision.relativeVelocity * -1f, collision.contacts[0].point);
		}
	}
}
