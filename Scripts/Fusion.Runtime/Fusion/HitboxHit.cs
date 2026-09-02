using UnityEngine;

namespace Fusion;

internal struct HitboxHit
{
	public Vector3 Point;

	public Vector3 Normal;

	public float Distance;

	public Hitbox Hitbox;

	public Vector3 DebugPosition;

	public int DebugTick;

	public float Alpha;
}
