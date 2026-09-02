#define DEBUG
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Fusion;

[NetworkBehaviourWeaved(1)]
[DisallowMultipleComponent]
[AddComponentMenu("Fusion/Lag Compensation/Hitbox Root")]
public class HitboxRoot : NetworkBehaviour
{
	[Flags]
	public enum ConfigFlags
	{
		ReinitializeHitboxesBeforeRegistration = 1,
		IncludeInactiveHitboxes = 2,
		Legacy = ReinitializeHitboxesBeforeRegistration,
		Default = ReinitializeHitboxesBeforeRegistration | IncludeInactiveHitboxes
	}

	internal class HitboxComparerX : IComparer<HitboxRoot>
	{
		public int Compare(HitboxRoot a, HitboxRoot b)
		{
			return a.transform.position.x.CompareTo(b.transform.position.x);
		}
	}

	internal class HitboxComparerY : IComparer<HitboxRoot>
	{
		public int Compare(HitboxRoot a, HitboxRoot b)
		{
			return a.transform.position.y.CompareTo(b.transform.position.y);
		}
	}

	internal class HitboxComparerZ : IComparer<HitboxRoot>
	{
		public int Compare(HitboxRoot a, HitboxRoot b)
		{
			return a.transform.position.z.CompareTo(b.transform.position.z);
		}
	}

	private const int WORD_COUNT = 1;

	public const int MAX_HITBOXES = 31;

	[InlineHelp]
	public ConfigFlags Config = ConfigFlags.ReinitializeHitboxesBeforeRegistration;

	[InlineHelp]
	[Unit(Units.Units)]
	[MultiPropertyDrawersFix]
	public float BroadRadius;

	[InlineHelp]
	public Vector3 Offset;

	[InlineHelp]
	public Color GizmosColor = Color.gray;

	[InlineHelp]
	[Space(4f)]
	public Hitbox[] Hitboxes;

	public unsafe bool HitboxRootActive
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return (*Ptr & 1) == 0;
		}
		set
		{
			if (value)
			{
				*Ptr &= -2;
			}
			else
			{
				*Ptr |= 1;
			}
		}
	}

	internal bool Registered => BehaviourUtils.IsAlive(Manager);

	public HitboxManager Manager { get; internal set; }

	public void OnDrawGizmos()
	{
		Matrix4x4 localToWorldMatrix = Matrix4x4.TRS(base.transform.position, base.transform.rotation, Vector3.one);
		DrawGizmos(GizmosColor, ref localToWorldMatrix);
	}

	protected virtual void DrawGizmos(Color color, ref Matrix4x4 localToWorldMatrix)
	{
		Gizmos.matrix = localToWorldMatrix;
		Gizmos.color = color;
		Gizmos.DrawWireSphere(Offset, BroadRadius);
		Gizmos.color = Color.white;
		Gizmos.matrix = Matrix4x4.identity;
	}

	[BehaviourButtonAction("Find Hitboxes", null, null, ConditionFlags = (BehaviourActionAttribute.ActionFlags.ShowAtNotRuntime | BehaviourActionAttribute.ActionFlags.DirtyAfterButton))]
	public void InitHitboxes()
	{
		bool includeInactive = (Config & ConfigFlags.IncludeInactiveHitboxes) == ConfigFlags.IncludeInactiveHitboxes;
		Hitboxes = base.transform.GetNestedComponentsInChildren<Hitbox, NetworkObject>(null, includeInactive).ToArray();
		if (Hitboxes.Length > 31)
		{
			Debug.LogWarning($"Hitbox count above limit per root, clamped to max {31}");
			Array.Resize(ref Hitboxes, 31);
		}
		for (int i = 0; i < Hitboxes.Length; i++)
		{
			Hitbox hitbox = Hitboxes[i];
			hitbox._hitboxIndex = i;
			hitbox.Root = this;
		}
		if (BroadRadius == 0f)
		{
			SetMinBoundingRadius();
		}
	}

	[BehaviourButtonAction("Quick Set BroadRadius", null, null, ConditionFlags = (BehaviourActionAttribute.ActionFlags.AlwaysShow | BehaviourActionAttribute.ActionFlags.DirtyAfterButton))]
	public void SetMinBoundingRadius()
	{
		Vector3 vector = base.transform.position + base.transform.rotation * Offset;
		float num = 0f;
		Hitbox[] hitboxes = Hitboxes;
		foreach (Hitbox hitbox in hitboxes)
		{
			if (hitbox.Type != HitboxTypes.None)
			{
				Vector3 vector2 = hitbox.transform.position + hitbox.transform.rotation * hitbox.Offset;
				float magnitude = (vector2 - vector).magnitude;
				magnitude += ((hitbox.Type == HitboxTypes.Sphere) ? hitbox.AbsSphereRadius : hitbox.BoxExtents.magnitude);
				if (magnitude > num)
				{
					num = magnitude;
				}
			}
		}
		BroadRadius = num;
	}

	public void SetHitboxActive(Hitbox hitbox, bool setActive)
	{
		if ((uint)hitbox.HitboxIndex >= 31u)
		{
			throw new ArgumentOutOfRangeException($"Hitbox index {hitbox.HitboxIndex} is outside the valid range: [0, {31})");
		}
		if (!BehaviourUtils.IsSame(this, hitbox.Root))
		{
			Assert.Fail(string.Format("Hitbox '{0}' is part of a different HitboxRoot '{1}' than this '{2}'. Are you missing a call to {3} to update the root reference?", hitbox, hitbox.Root, this, "InitHitboxes"));
		}
		else if (hitbox.HitboxIndex >= Hitboxes.Length || !BehaviourUtils.IsSame(Hitboxes[hitbox.HitboxIndex], hitbox))
		{
			Assert.Fail(string.Format("Hitbox '{0}' (index {1}) does not match Root's Hitbox of same index: '{2}'. Are you missing a call to {3} to update the root reference?", hitbox, hitbox.HitboxIndex, Hitboxes[hitbox.HitboxIndex], "InitHitboxes"));
		}
		else
		{
			SetHitboxActiveFastUnchecked(hitbox, setActive);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe void SetHitboxActiveFastUnchecked(Hitbox hitbox, bool setActive)
	{
		if (setActive)
		{
			*Ptr &= (int)(~hitbox.HitboxMask);
		}
		else
		{
			*Ptr |= (int)hitbox.HitboxMask;
		}
	}

	public bool IsHitboxActive(Hitbox hitbox)
	{
		if ((uint)hitbox.HitboxIndex >= 31u)
		{
			throw new ArgumentOutOfRangeException($"Hitbox index {hitbox.HitboxIndex} is outside the valid range: [0, {31})");
		}
		if (!BehaviourUtils.IsSame(this, hitbox.Root))
		{
			Assert.Fail(string.Format("Hitbox '{0}' is part of a different HitboxRoot '{1}' than this '{2}'. Are you missing a call to {3} to update the root reference?", hitbox, hitbox.Root, this, "InitHitboxes"));
			return false;
		}
		if (hitbox.HitboxIndex >= Hitboxes.Length || !BehaviourUtils.IsSame(Hitboxes[hitbox.HitboxIndex], hitbox))
		{
			Assert.Fail(string.Format("Hitbox '{0}' (index {1}) does not match Root's Hitbox of same index: '{2}'. Are you missing a call to {3} to update the root reference?", hitbox, hitbox.HitboxIndex, Hitboxes[hitbox.HitboxIndex], "InitHitboxes"));
			return false;
		}
		return IsHitboxActiveFastUnchecked(hitbox);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal unsafe bool IsHitboxActiveFastUnchecked(Hitbox hitbox)
	{
		return ((uint)(*Ptr) & hitbox.HitboxMask) == 0;
	}

	public override void Despawned(NetworkRunner runner, bool hasState)
	{
		DeRegister();
	}

	public void DeRegister()
	{
		if (Registered && BehaviourUtils.IsAlive(Manager))
		{
			HitboxManager manager = Manager;
			Manager = null;
			manager.Remove(this);
		}
	}

	internal void RegisterColliders(IHitboxColliderContainer container, int tick)
	{
		if ((Config & ConfigFlags.ReinitializeHitboxesBeforeRegistration) == ConfigFlags.ReinitializeHitboxesBeforeRegistration)
		{
			InitHitboxes();
		}
		if (Hitboxes != null)
		{
			Hitbox[] hitboxes = Hitboxes;
			foreach (Hitbox hitbox in hitboxes)
			{
				hitbox.SetColliderData(ref container.GetNextCollider(out var index), tick);
				hitbox.ColliderIndex = index;
			}
		}
	}

	internal void DeregisterColliders(IHitboxColliderContainer container)
	{
		if (Hitboxes == null)
		{
			return;
		}
		Hitbox[] hitboxes = Hitboxes;
		foreach (Hitbox hitbox in hitboxes)
		{
			if (hitbox.ColliderIndex > 0)
			{
				container.ReleaseCollider(hitbox.ColliderIndex);
			}
		}
	}

	internal Bounds GetBounds()
	{
		Vector3 vector = base.transform.TransformPoint(Offset);
		return new Bounds
		{
			min = new Vector3(vector.x - BroadRadius, vector.y - BroadRadius, vector.z - BroadRadius),
			max = new Vector3(vector.x + BroadRadius, vector.y + BroadRadius, vector.z + BroadRadius)
		};
	}
}
