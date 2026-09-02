using UnityEngine;

namespace DestroyIt;

public class ImpactDamage : Damage
{
	public float DamageAmount { get; set; }

	public Rigidbody ImpactObject { get; set; }

	public Vector3 ImpactObjectVelocityFrom { get; set; }

	public Vector3 ImpactObjectVelocityTo => ImpactObjectVelocityFrom * -1f;

	public float AdditionalForce { get; set; }

	public Vector3 AdditionalForcePosition { get; set; }

	public float AdditionalForceRadius { get; set; }
}
