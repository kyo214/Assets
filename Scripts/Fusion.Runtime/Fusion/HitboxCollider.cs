using UnityEngine;

namespace Fusion;

internal struct HitboxCollider
{
	internal HitboxTypes Type;

	internal Matrix4x4 LocalToWorld;

	internal Vector3 Offset;

	internal Vector3 BoxExtents;

	internal float SphereRadius;

	internal bool Active;

	internal Hitbox Hitbox;

	internal int layerMask;

	internal int DebugTick;

	internal bool Used;

	internal int Next;

	internal LagCompensationUtils.BoxNarrowData BoxNarrowData;

	internal bool IsBoxNarrowDataInitialized { get; set; }

	internal Vector3 Position => LocalToWorld.MultiplyPoint(Offset);

	internal static void Lerp(ref HitboxCollider from, ref HitboxCollider to, float alpha, ref HitboxCollider result)
	{
		result = from;
		result.Offset = Vector3.Lerp(from.Offset, to.Offset, alpha);
		result.SphereRadius = Mathf.Lerp(from.SphereRadius, to.SphereRadius, alpha);
		result.BoxExtents = Vector3.Lerp(from.BoxExtents, to.BoxExtents, alpha);
		result.LocalToWorld = Lerp(ref from.LocalToWorld, ref to.LocalToWorld, alpha);
		result.layerMask = ((alpha > 0.5f) ? to.layerMask : from.layerMask);
		result.IsBoxNarrowDataInitialized = false;
	}

	internal void InitNarrowData()
	{
		if (Type == HitboxTypes.Box && !IsBoxNarrowDataInitialized)
		{
			BoxNarrowData = new LagCompensationUtils.BoxNarrowData(Position, LocalToWorld.rotation, BoxExtents);
			IsBoxNarrowDataInitialized = true;
		}
	}

	private static Matrix4x4 Lerp(ref Matrix4x4 from, ref Matrix4x4 to, float alpha)
	{
		Vector3 a = from.GetColumn(3);
		Quaternion a2 = Quaternion.LookRotation(from.GetColumn(2), from.GetColumn(1));
		Vector3 a3 = new Vector3(from.GetColumn(0).magnitude, from.GetColumn(1).magnitude, from.GetColumn(2).magnitude);
		Vector3 b = to.GetColumn(3);
		Quaternion b2 = Quaternion.LookRotation(to.GetColumn(2), to.GetColumn(1));
		Vector3 b3 = new Vector3(to.GetColumn(0).magnitude, to.GetColumn(1).magnitude, to.GetColumn(2).magnitude);
		Vector3 pos = Vector3.Lerp(a, b, alpha);
		Vector3 s = Vector3.Lerp(a3, b3, alpha);
		Quaternion q = Quaternion.Slerp(a2, b2, alpha);
		return Matrix4x4.TRS(pos, q, s);
	}
}
