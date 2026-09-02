#define DEBUG
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion;

[AddComponentMenu("Fusion/Lag Compensation/Hitbox")]
public class Hitbox : Behaviour
{
	[InlineHelp]
	public HitboxTypes Type;

	[InlineHelp]
	[DrawIf("Type", 2.0, Hide = true)]
	[Unit(Units.Units)]
	[MultiPropertyDrawersFix]
	public float SphereRadius;

	[InlineHelp]
	[DrawIf("Type", 1.0, Hide = true)]
	[MultiPropertyDrawersFix]
	public Vector3 BoxExtents;

	[DrawIf("Type", Hide = true)]
	public Vector3 Offset;

	[HideInInspector]
	public HitboxRoot Root;

	internal int _hitboxIndex;

	[InlineHelp]
	public Color GizmosColor = Color.yellow;

	internal float AbsSphereRadius => Mathf.Abs(SphereRadius);

	internal Vector3 AbsBoxExtents
	{
		get
		{
			Vector3 result = default;
			result.x = Mathf.Abs(BoxExtents.x);
			result.y = Mathf.Abs(BoxExtents.y);
			result.z = Mathf.Abs(BoxExtents.z);
			return result;
		}
	}

	public int HitboxIndex
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _hitboxIndex;
		}
	}

	internal uint HitboxMask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			Assert.Check((uint)_hitboxIndex < 31u);
			return (uint)(1 << _hitboxIndex + 1);
		}
	}

	public bool HitboxActive
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return BehaviourUtils.IsAlive(Root) && Root.IsHitboxActive(this);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (BehaviourUtils.IsAlive(Root))
			{
				Root.SetHitboxActive(this, value);
			}
		}
	}

	public int ColliderIndex { get; internal set; }

	public Vector3 Position => base.transform.position + base.transform.rotation * Offset;

	internal void SetColliderData(ref HitboxCollider c, int tick)
	{
		Assert.Check(BehaviourUtils.IsAlive(Root));
		c.Type = Type;
		c.Offset = Offset;
		c.LocalToWorld = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
		c.BoxExtents = AbsBoxExtents;
		c.SphereRadius = AbsSphereRadius;
		c.Hitbox = this;
		c.DebugTick = tick;
		c.layerMask = 1 << base.gameObject.layer;
		c.Active = Root.IsHitboxActiveFastUnchecked(this);
		c.IsBoxNarrowDataInitialized = false;
	}

	public unsafe void OnDrawGizmos()
	{
		Color gizmosColor = GizmosColor;
		if (BehaviourUtils.IsAlive(Root) && Root.Ptr != null && (!Root.HitboxRootActive || !Root.IsHitboxActiveFastUnchecked(this)))
		{
			gizmosColor.a *= 0.1f;
		}
		Matrix4x4 localToWorldMatrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
		DrawGizmos(gizmosColor, ref localToWorldMatrix);
	}

	protected virtual void DrawGizmos(Color color, ref Matrix4x4 localToWorldMatrix)
	{
		Gizmos.matrix = localToWorldMatrix;
		Gizmos.color = color;
		switch (Type)
		{
		case HitboxTypes.Box:
			Gizmos.DrawWireCube(Offset, AbsBoxExtents * 2f);
			break;
		case HitboxTypes.Sphere:
			Gizmos.DrawWireSphere(Offset, AbsSphereRadius);
			break;
		}
		Gizmos.color = Color.white;
		Gizmos.matrix = Matrix4x4.identity;
	}
}
