using UnityEngine;

namespace DestroyIt;

public class ExplosiveDamage : Damage
{
	public float BlastForce { get; set; }

	public Vector3 Position { get; set; }

	public float Radius { get; set; }

	public float UpwardModifier { get; set; }

	public float DamageAmount { get; set; }
}
