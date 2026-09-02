#define DEBUG
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Fusion.LagCompensation;
using UnityEngine;

namespace Fusion;

internal class HitboxBuffer
{
	internal class HitboxSnapshot : IHitboxColliderContainer
	{
		internal readonly int CollidersCapacity;

		private HitboxCollider[] _colliders;

		private int _collidersCount = 1;

		private int _collidersTempCount = 0;

		private int _collidersFreeHead = 0;

		internal ILagCompensationBroadphase _broadphase;

		internal int Tick;

		internal int DataTick;

		private readonly RayNodeTraversalTest _rayTraversalTest = new RayNodeTraversalTest(default, default, 0f);

		private readonly RadialNodeTraversalTest _radialTraversalTest = new RadialNodeTraversalTest(default, 0f);

		private readonly AabbNodeTraversalTest _aabbTraversalTest = new AabbNodeTraversalTest(default, default);

		internal int CollidersCount => _collidersCount - 1;

		internal HitboxSnapshot(Mapper mapper, List<HitboxRoot> initialObjects, int hitboxCapacity, float expansionFactor)
		{
			CollidersCapacity = Math.Max(16, hitboxCapacity);
			_colliders = new HitboxCollider[CollidersCapacity];
			if (initialObjects != null)
			{
				foreach (HitboxRoot initialObject in initialObjects)
				{
					initialObject.RegisterColliders(this, 0);
				}
			}
			_broadphase = new BVH(mapper, CollidersCapacity * 2, initialObjects, expansionFactor);
		}

		internal void CopyFrom(int tick, int dataTick, HitboxSnapshot from)
		{
			ReleaseTempColliders();
			_broadphase.CopyFrom(from._broadphase);
			Tick = tick;
			DataTick = dataTick;
			Assert.Check(_colliders.Length >= from._collidersCount);
			Array.Copy(from._colliders, 0, _colliders, 0, from._collidersCount);
			Array.Clear(_colliders, from._collidersCount, _colliders.Length - from._collidersCount);
			_collidersCount = from._collidersCount;
			_collidersFreeHead = from._collidersFreeHead;
		}

		public ref HitboxCollider GetNextCollider(out int index)
		{
			Assert.Check(_collidersTempCount == 0, "Temp Colliders were not released.", _collidersTempCount, _collidersCount, CollidersCapacity);
			if (_collidersFreeHead == 0)
			{
				Assert.Check(_collidersCount < CollidersCapacity, "All hitbox colliders are already being used. Consider increasing the hitbox capacity in the Lag-Compensation settings.");
				index = _collidersCount++;
			}
			else
			{
				index = _collidersFreeHead;
				_collidersFreeHead = _colliders[_collidersFreeHead].Next;
			}
			Assert.Check(!_colliders[index].Used, index);
			_colliders[index] = default;
			_colliders[index].Used = true;
			return ref _colliders[index];
		}

		public ref HitboxCollider GetNextTempCollider(out int tmpIndex)
		{
			Assert.Check(_collidersCount + _collidersTempCount < CollidersCapacity, "All hitbox colliders are already being used. Consider increasing the hitbox capacity in the Lag-Compensation settings.");
			tmpIndex = _collidersCount + _collidersTempCount++;
			Assert.Check(!_colliders[tmpIndex].Used, tmpIndex);
			_colliders[tmpIndex] = default;
			return ref _colliders[tmpIndex];
		}

		public void ReleaseTempColliders()
		{
			if (_collidersTempCount > 0)
			{
				Array.Clear(_colliders, _collidersCount, _collidersTempCount);
			}
			_collidersTempCount = 0;
		}

		public void ReleaseCollider(int index)
		{
			if (index <= 0 || index >= CollidersCapacity)
			{
				throw new IndexOutOfRangeException($"Index {index} is out of valid range: (0, {CollidersCapacity})");
			}
			Assert.Check(_colliders[index].Used, index);
			_colliders[index].Used = false;
			_colliders[index].Next = _collidersFreeHead;
			_collidersFreeHead = index;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref HitboxCollider GetCollider(int index)
		{
			if (index <= 0 || index >= CollidersCapacity)
			{
				throw new IndexOutOfRangeException($"Index {index} is out of valid range: (0, {CollidersCapacity})");
			}
			return ref _colliders[index];
		}

		internal void Add(HitboxRoot h)
		{
			h.RegisterColliders(this, DataTick);
			_broadphase.Add(h);
		}

		internal bool Remove(HitboxRoot hr)
		{
			hr.DeregisterColliders(this);
			return _broadphase.Remove(hr);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Update(HitboxRoot h)
		{
			bool hitboxRootActive = h.HitboxRootActive;
			Hitbox[] hitboxes = h.Hitboxes;
			foreach (Hitbox hitbox in hitboxes)
			{
				ref HitboxCollider collider = ref GetCollider(hitbox.ColliderIndex);
				if (hitboxRootActive)
				{
					hitbox.SetColliderData(ref collider, DataTick);
				}
				else
				{
					collider.Active = false;
				}
			}
			_broadphase.Update(h, DataTick);
		}

		internal void Optimize()
		{
			_broadphase.Optimize();
		}

		internal static bool NarrowPhaseRay(ref HitboxCollider c, Vector3 origin, Vector3 direction, float length, out Vector3 point, out Vector3 normal, out float distance)
		{
			switch (c.Type)
			{
			case HitboxTypes.Box:
			{
				Matrix4x4 inverse = c.LocalToWorld.inverse;
				Vector3 origin2 = inverse.MultiplyPoint(origin) - c.Offset;
				Vector3 dir = inverse.MultiplyVector(direction);
				Vector3 minB = -c.BoxExtents;
				Vector3 maxB = c.BoxExtents;
				if (LagCompensationUtils.RayAABB(ref minB, ref maxB, ref origin2, ref dir, length * length, out point, out normal, out distance))
				{
					point = c.LocalToWorld.MultiplyPoint(point + c.Offset);
					normal = c.LocalToWorld.MultiplyVector(normal);
					return true;
				}
				break;
			}
			case HitboxTypes.Sphere:
			{
				Vector3 center = c.LocalToWorld.MultiplyPoint(c.Offset);
				if (LagCompensationUtils.RaySphereIntersection(origin, direction, length, center, c.SphereRadius, out point, out normal, out distance))
				{
					return true;
				}
				break;
			}
			}
			point = default;
			normal = default;
			distance = 0f;
			return false;
		}

		internal static bool NarrowPhaseSphere(ref HitboxCollider c, Vector3 origin, float radius, out Vector3 point, out Vector3 normal)
		{
			switch (c.Type)
			{
			case HitboxTypes.Box:
			{
				Vector3 sphereCenter = c.LocalToWorld.inverse.MultiplyPoint(origin) - c.Offset;
				if (LagCompensationUtils.LocalAABBSphereContact(c.BoxExtents, sphereCenter, radius, out var contact))
				{
					point = c.LocalToWorld.MultiplyPoint(contact.Point);
					normal = c.LocalToWorld.MultiplyVector(contact.Normal);
					return true;
				}
				break;
			}
			case HitboxTypes.Sphere:
			{
				Vector3 centerB = c.LocalToWorld.MultiplyPoint(c.Offset);
				if (LagCompensationUtils.SphereSphere(origin, radius, centerB, c.SphereRadius, out point, out normal))
				{
					return true;
				}
				break;
			}
			}
			point = default;
			normal = default;
			return false;
		}

		internal static bool NarrowPhaseBox(ref LagCompensationUtils.BoxNarrowData boxQueryNarrowData, ref HitboxCollider c, bool computeDetailedInfo, out Vector3 hitPoint, out Vector3 hitNormal)
		{
			switch (c.Type)
			{
			case HitboxTypes.Box:
				Assert.Check(c.IsBoxNarrowDataInitialized);
				return LagCompensationUtils.NarrowBoxBox(ref boxQueryNarrowData, ref c.BoxNarrowData, computeDetailedInfo, out hitPoint, out hitNormal);
			case HitboxTypes.Sphere:
			{
				Vector3 sphereCenter = boxQueryNarrowData.WorldToLocalPoint(c.Position);
				if (LagCompensationUtils.LocalAABBSphereContact(boxQueryNarrowData.Extents, sphereCenter, c.SphereRadius, out var contact))
				{
					hitPoint = boxQueryNarrowData.LocalToWorldPoint(contact.Point);
					hitNormal = boxQueryNarrowData.LocalToWorldVector(-contact.Normal);
					return true;
				}
				break;
			}
			}
			hitPoint = default;
			hitNormal = default;
			return false;
		}

		public void RaycastBroadphase(ref Query query, List<HitboxRoot> broadphaseCandidates)
		{
			Assert.Check(query.Type == QueryType.Raycast);
			_rayTraversalTest.SetTestSettings(query.Raycast.Origin, query.Raycast.Direction, query.Raycast.Length);
			broadphaseCandidates.Clear();
			_broadphase.Traverse(_rayTraversalTest, broadphaseCandidates, query.LayerMask);
		}

		public void OverlapSphereBroadphase(ref Query query, List<HitboxRoot> broadphaseCandidates)
		{
			Assert.Check(query.Type == QueryType.SphereOverlap);
			_radialTraversalTest.SetTestSettings(query.SphereOverlap.Center, query.SphereOverlap.Radius);
			broadphaseCandidates.Clear();
			_broadphase.Traverse(_radialTraversalTest, broadphaseCandidates, query.LayerMask);
		}

		public void OverlapBoxBroadphase(ref Query query, List<HitboxRoot> broadphaseCandidates)
		{
			Assert.Check(query.Type == QueryType.BoxOverlap);
			_aabbTraversalTest.SetTestSettings(query.BoxOverlap.Center, query.BoxOverlap.AabbExtents);
			broadphaseCandidates.Clear();
			_broadphase.Traverse(_aabbTraversalTest, broadphaseCandidates, query.LayerMask);
		}

		public void ProcessBroadphaseRootCandidates(ref Query query, bool interpolateColliders, List<HitboxRoot> rootCandidates, IHitboxColliderContainer refContainer, HashSet<int> processedColliderIndices)
		{
			bool flag = (query.Options & HitOptions.IgnoreInputAuthority) == HitOptions.IgnoreInputAuthority && query.Player.IsValid;
			Assert.Check(!interpolateColliders || query.Alpha.HasValue);
			float alpha = (interpolateColliders ? query.Alpha.Value : 0f);
			for (int i = 0; i < rootCandidates.Count; i++)
			{
				HitboxRoot hitboxRoot = rootCandidates[i];
				if (flag && hitboxRoot.Object.InputAuthority == query.Player)
				{
					continue;
				}
				for (int j = 0; j < hitboxRoot.Hitboxes.Length; j++)
				{
					int colliderIndex = hitboxRoot.Hitboxes[j].ColliderIndex;
					ref HitboxCollider collider = ref GetCollider(colliderIndex);
					if (!processedColliderIndices.Contains(colliderIndex))
					{
						if (!interpolateColliders && collider.Active && (query.LayerMask & collider.layerMask) != 0)
						{
							processedColliderIndices.Add(colliderIndex);
						}
					}
					else if (interpolateColliders)
					{
						processedColliderIndices.Remove(colliderIndex);
						HitboxCollider.Lerp(ref refContainer.GetCollider(colliderIndex), ref collider, alpha, ref refContainer.GetNextTempCollider(out var tmpIndex));
						processedColliderIndices.Add(tmpIndex);
					}
				}
			}
		}

		internal void DebugDraw(Color color, bool renderBroadphase, bool renderColliders)
		{
			Gizmos.color = color;
			if (renderBroadphase)
			{
				_broadphase.RenderGizmos(color);
			}
			if (renderColliders)
			{
				DrawColliders(color);
			}
		}

		private void DrawColliders(Color color)
		{
			Gizmos.color = color;
			for (int i = 0; i < _collidersCount; i++)
			{
				HitboxCollider hitboxCollider = _colliders[i];
				if (hitboxCollider.Active)
				{
					Gizmos.matrix = hitboxCollider.LocalToWorld;
					switch (hitboxCollider.Type)
					{
					case HitboxTypes.Box:
						Gizmos.DrawWireCube(hitboxCollider.Offset, hitboxCollider.BoxExtents * 2f);
						break;
					case HitboxTypes.Sphere:
						Gizmos.DrawWireSphere(hitboxCollider.Offset, hitboxCollider.SphereRadius);
						break;
					}
					Gizmos.matrix = Matrix4x4.identity;
				}
			}
		}
	}

	internal HitboxSnapshot[] _buffer;

	private Mapper _mapper;

	private int _head = 0;

	private int _advanced = 0;

	internal int Tick;

	private readonly List<HitboxRoot> _broadphaseCandidates = new List<HitboxRoot>();

	private readonly HashSet<int> _colliderCandidates = new HashSet<int>();

	internal int Length => _buffer.Length;

	internal BVH BVH => _buffer[_head]._broadphase as BVH;

	internal HitboxSnapshot Current => _buffer[_head];

	internal HitboxBuffer(List<HitboxRoot> initialObjects, int bufferSize, int hitboxCapacity, float expansionFactor)
	{
		if (bufferSize <= 0)
		{
			Log.DebugWarn(string.Format("Trying to initialize {0} with {1} length. Initiatizing with 1 instead.", "HitboxBuffer", bufferSize));
			bufferSize = 1;
		}
		_buffer = new HitboxSnapshot[bufferSize];
		_mapper = new Mapper();
		_head = 0;
		_advanced = 0;
		Assert.Check(Length > 0);
		_buffer[0] = new HitboxSnapshot(_mapper, initialObjects, hitboxCapacity, expansionFactor);
		for (int i = 1; i < Length; i++)
		{
			_buffer[i] = new HitboxSnapshot(_mapper, null, hitboxCapacity, expansionFactor);
		}
	}

	internal void Advance(int tick, int dataTick)
	{
		int num;
		if (tick == Tick)
		{
			num = (_head + _buffer.Length - 1) % _buffer.Length;
		}
		else
		{
			num = _head;
			_advanced++;
		}
		_head = (num + 1) % _buffer.Length;
		_buffer[_head].CopyFrom(tick, dataTick, _buffer[num]);
		Tick = tick;
	}

	internal void Optimize()
	{
		_buffer[_head].Optimize();
	}

	internal void Add(HitboxRoot root)
	{
		_buffer[_head].Add(root);
	}

	internal bool Remove(HitboxRoot root)
	{
		return _buffer[_head].Remove(root);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void Update(HitboxRoot root)
	{
		_buffer[_head].Update(root);
	}

	internal void DebugDraw(Color color, Color historyColor, bool debugBroadphase, bool debugHistory)
	{
		if (debugBroadphase)
		{
			_buffer[_head].DebugDraw(color, renderBroadphase: true, renderColliders: false);
		}
		if (debugHistory)
		{
			HitboxSnapshot[] buffer = _buffer;
			foreach (HitboxSnapshot hitboxSnapshot in buffer)
			{
				hitboxSnapshot.DebugDraw(historyColor, renderBroadphase: false, renderColliders: true);
			}
		}
	}

	internal bool Raycast(ref Query query, out HitboxHit hit)
	{
		Assert.Check(query.Type == QueryType.Raycast);
		Assert.Check(!query.Raycast.HitAll);
		_colliderCandidates.Clear();
		RaycastBroadphase(ref query, _colliderCandidates, out var container);
		if (_colliderCandidates.Count <= 0)
		{
			hit = default;
			return false;
		}
		InitColliderCandidatesForNarrowPhase(container, _colliderCandidates);
		bool result = RaycastNarrowPhase(ref query.Raycast, container, _colliderCandidates, out hit);
		container.ReleaseTempColliders();
		return result;
	}

	internal bool RaycastAll(ref Query query, List<HitboxHit> hits)
	{
		Assert.Check(query.Type == QueryType.Raycast);
		Assert.Check(query.Raycast.HitAll);
		_colliderCandidates.Clear();
		RaycastBroadphase(ref query, _colliderCandidates, out var container);
		if (_colliderCandidates.Count <= 0)
		{
			return false;
		}
		InitColliderCandidatesForNarrowPhase(container, _colliderCandidates);
		bool result = RaycastAllNarrowPhase(ref query.Raycast, container, _colliderCandidates, hits);
		container.ReleaseTempColliders();
		return result;
	}

	internal bool OverlapSphere(ref Query query, List<HitboxHit> hits)
	{
		Assert.Check(query.Type == QueryType.SphereOverlap);
		_colliderCandidates.Clear();
		OverlapSphereBroadphase(ref query, _colliderCandidates, out var container);
		if (_colliderCandidates.Count <= 0)
		{
			return false;
		}
		InitColliderCandidatesForNarrowPhase(container, _colliderCandidates);
		bool result = OverlapSphereNarrowPhase(ref query.SphereOverlap, container, _colliderCandidates, hits);
		container.ReleaseTempColliders();
		return result;
	}

	internal bool OverlapBox(ref Query query, bool computeDetailedInfo, List<HitboxHit> hits)
	{
		Assert.Check(query.Type == QueryType.BoxOverlap);
		_colliderCandidates.Clear();
		bool flag;
		LagCompensationUtils.BoxNarrowData boxNarrowData;
		if (query.BoxOverlap.Rotation == Quaternion.identity)
		{
			flag = false;
			boxNarrowData = default;
			query.BoxOverlap.AabbExtents = query.BoxOverlap.Extents;
		}
		else
		{
			flag = true;
			boxNarrowData = new LagCompensationUtils.BoxNarrowData(query.BoxOverlap.Center, query.BoxOverlap.Rotation, query.BoxOverlap.Extents);
			Vector3 start = boxNarrowData.BoxEdgesRotated.E00.Start;
			Vector3 start2 = boxNarrowData.BoxEdgesRotated.E01.Start;
			Vector3 start3 = boxNarrowData.BoxEdgesRotated.E02.Start;
			Vector3 start4 = boxNarrowData.BoxEdgesRotated.E03.Start;
			start.x = Mathf.Abs(start.x);
			start.y = Mathf.Abs(start.y);
			start.z = Mathf.Abs(start.z);
			start2.x = Mathf.Abs(start2.x);
			start2.y = Mathf.Abs(start2.y);
			start2.z = Mathf.Abs(start2.z);
			start3.x = Mathf.Abs(start3.x);
			start3.y = Mathf.Abs(start3.y);
			start3.z = Mathf.Abs(start3.z);
			start4.x = Mathf.Abs(start4.x);
			start4.y = Mathf.Abs(start4.y);
			start4.z = Mathf.Abs(start4.z);
			Vector3 aabbExtents = default;
			aabbExtents.x = Mathf.Max(start.x, start2.x);
			aabbExtents.y = Mathf.Max(start.y, start2.y);
			aabbExtents.z = Mathf.Max(start.z, start2.z);
			aabbExtents.x = Mathf.Max(aabbExtents.x, start3.x);
			aabbExtents.y = Mathf.Max(aabbExtents.y, start3.y);
			aabbExtents.z = Mathf.Max(aabbExtents.z, start3.z);
			aabbExtents.x = Mathf.Max(aabbExtents.x, start4.x);
			aabbExtents.y = Mathf.Max(aabbExtents.y, start4.y);
			aabbExtents.z = Mathf.Max(aabbExtents.z, start4.z);
			query.BoxOverlap.AabbExtents = aabbExtents;
		}
		OverlapBoxBroadphase(ref query, _colliderCandidates, out var container);
		if (_colliderCandidates.Count <= 0)
		{
			return false;
		}
		InitColliderCandidatesForNarrowPhase(container, _colliderCandidates);
		if (!flag)
		{
			boxNarrowData = new LagCompensationUtils.BoxNarrowData(query.BoxOverlap.Center, query.BoxOverlap.Rotation, query.BoxOverlap.Extents);
		}
		bool result = OverlapBoxNarrowPhase(ref boxNarrowData, computeDetailedInfo, container, _colliderCandidates, hits);
		container.ReleaseTempColliders();
		return result;
	}

	private void InitColliderCandidatesForNarrowPhase(IHitboxColliderContainer container, HashSet<int> candidates)
	{
		foreach (int candidate in candidates)
		{
			container.GetCollider(candidate).InitNarrowData();
		}
	}

	private bool RaycastNarrowPhase(ref RaycastQuery raycast, IHitboxColliderContainer container, HashSet<int> candidates, out HitboxHit hit)
	{
		float num = float.MaxValue;
		bool result = false;
		Vector3 point = default;
		Vector3 normal = default;
		HitboxCollider hitboxCollider = default;
		foreach (int candidate in candidates)
		{
			ref HitboxCollider collider = ref container.GetCollider(candidate);
			if (HitboxSnapshot.NarrowPhaseRay(ref collider, raycast.Origin, raycast.Direction, raycast.Length, out var point2, out var normal2, out var distance))
			{
				if (distance < num)
				{
					num = distance;
					hitboxCollider = collider;
					point = point2;
					normal = normal2;
				}
				result = true;
			}
		}
		hit.Point = point;
		hit.Distance = num;
		hit.Normal = normal;
		hit.Hitbox = hitboxCollider.Hitbox;
		hit.DebugTick = hitboxCollider.DebugTick;
		hit.DebugPosition = hitboxCollider.Position;
		hit.Alpha = 0f;
		return result;
	}

	private bool RaycastAllNarrowPhase(ref RaycastQuery raycast, IHitboxColliderContainer container, HashSet<int> candidates, List<HitboxHit> hits)
	{
		bool result = false;
		foreach (int candidate in candidates)
		{
			ref HitboxCollider collider = ref container.GetCollider(candidate);
			if (HitboxSnapshot.NarrowPhaseRay(ref collider, raycast.Origin, raycast.Direction, raycast.Length, out var point, out var normal, out var distance))
			{
				hits.Add(new HitboxHit
				{
					Point = point,
					Distance = distance,
					Normal = normal,
					Hitbox = collider.Hitbox,
					DebugTick = collider.DebugTick,
					DebugPosition = collider.Position,
					Alpha = 0f
				});
				result = true;
			}
		}
		return result;
	}

	private bool OverlapSphereNarrowPhase(ref SphereOverlapQuery sphereOverlap, IHitboxColliderContainer container, HashSet<int> candidates, List<HitboxHit> hits)
	{
		bool result = false;
		HitboxHit item = default;
		foreach (int candidate in candidates)
		{
			ref HitboxCollider collider = ref container.GetCollider(candidate);
			if (HitboxSnapshot.NarrowPhaseSphere(ref collider, sphereOverlap.Center, sphereOverlap.Radius, out var point, out var normal))
			{
				result = true;
				item.Point = point;
				item.Normal = normal;
				item.Distance = 0f;
				item.Hitbox = collider.Hitbox;
				item.DebugTick = collider.DebugTick;
				item.DebugPosition = collider.Position;
				item.Alpha = 0f;
				hits.Add(item);
			}
		}
		return result;
	}

	private bool OverlapBoxNarrowPhase(ref LagCompensationUtils.BoxNarrowData boxNarrowData, bool computeDetailedInfo, IHitboxColliderContainer container, HashSet<int> candidates, List<HitboxHit> hits)
	{
		bool result = false;
		foreach (int candidate in candidates)
		{
			ref HitboxCollider collider = ref container.GetCollider(candidate);
			if (HitboxSnapshot.NarrowPhaseBox(ref boxNarrowData, ref collider, computeDetailedInfo, out var hitPoint, out var hitNormal))
			{
				result = true;
				hits.Add(new HitboxHit
				{
					Point = hitPoint,
					Normal = hitNormal,
					Hitbox = collider.Hitbox,
					DebugTick = collider.DebugTick,
					DebugPosition = collider.Position,
					Alpha = 0f
				});
			}
		}
		return result;
	}

	internal void PositionQueryInternal(ref Query query, out Vector3 position, out Quaternion rotation)
	{
		Assert.Check(query.Type == QueryType.PositionRotation);
		GetClosestSnapshotForTick(query.Tick, out var snapshot);
		int colliderIndex = query.PositionRotation.Hitbox.ColliderIndex;
		HitboxCollider from = snapshot.GetCollider(colliderIndex);
		if ((query.Options & HitOptions.SubtickAccuracy) == HitOptions.SubtickAccuracy && query.TickTo.HasValue && query.Alpha.HasValue)
		{
			GetClosestSnapshotForTick(query.TickTo.Value, out var snapshot2);
			HitboxCollider.Lerp(ref from, ref snapshot2.GetCollider(colliderIndex), query.Alpha.Value, ref from);
		}
		position = from.Position;
		rotation = QuaternionFromMatrix(from.LocalToWorld);
	}

	internal static Quaternion QuaternionFromMatrix(Matrix4x4 m)
	{
		return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
	}

	private void GetClosestSnapshotForTick(int tick, out HitboxSnapshot snapshot)
	{
		int num = tick - Tick;
		if (num > 0)
		{
			snapshot = _buffer[_head];
			Log.DebugWarn($"Tick {tick} is not in the Hitbox history, using closest instead: {snapshot.Tick}. Buffer length: {Length}, Buffer current tick: {Tick}");
		}
		else if (num < 1 - Length)
		{
			int num2 = ((_advanced < Length) ? 1 : ((_head + 1) % Length));
			snapshot = _buffer[num2];
			Log.DebugWarn($"Tick {tick} is not in the Hitbox history, using closest instead: {snapshot.Tick}. Buffer length: {Length}, Buffer current tick: {Tick}");
		}
		else
		{
			snapshot = _buffer[(_head + num + Length) % Length];
			Assert.Check(snapshot.Tick == tick);
		}
	}

	private void RaycastBroadphase(ref Query query, HashSet<int> processedColliderIndices, out IHitboxColliderContainer container)
	{
		Assert.Check(query.Type == QueryType.Raycast);
		GetClosestSnapshotForTick(query.Tick, out var snapshot);
		_broadphaseCandidates.Clear();
		snapshot.RaycastBroadphase(ref query, _broadphaseCandidates);
		query.PreProcessingDelegate?.Invoke(ref query, _broadphaseCandidates, processedColliderIndices);
		snapshot.ProcessBroadphaseRootCandidates(ref query, interpolateColliders: false, _broadphaseCandidates, snapshot, processedColliderIndices);
		if ((query.Options & HitOptions.SubtickAccuracy) == HitOptions.SubtickAccuracy && query.TickTo.HasValue && query.Alpha.HasValue)
		{
			GetClosestSnapshotForTick(query.TickTo.Value, out var snapshot2);
			_broadphaseCandidates.Clear();
			snapshot2.RaycastBroadphase(ref query, _broadphaseCandidates);
			snapshot2.ProcessBroadphaseRootCandidates(ref query, interpolateColliders: true, _broadphaseCandidates, snapshot, processedColliderIndices);
		}
		container = snapshot;
	}

	private void OverlapSphereBroadphase(ref Query query, HashSet<int> processedColliderIndices, out IHitboxColliderContainer container)
	{
		Assert.Check(query.Type == QueryType.SphereOverlap);
		GetClosestSnapshotForTick(query.Tick, out var snapshot);
		_broadphaseCandidates.Clear();
		snapshot.OverlapSphereBroadphase(ref query, _broadphaseCandidates);
		query.PreProcessingDelegate?.Invoke(ref query, _broadphaseCandidates, processedColliderIndices);
		snapshot.ProcessBroadphaseRootCandidates(ref query, interpolateColliders: false, _broadphaseCandidates, snapshot, processedColliderIndices);
		if ((query.Options & HitOptions.SubtickAccuracy) == HitOptions.SubtickAccuracy && query.TickTo.HasValue && query.Alpha.HasValue)
		{
			GetClosestSnapshotForTick(query.TickTo.Value, out var snapshot2);
			_broadphaseCandidates.Clear();
			snapshot2.OverlapSphereBroadphase(ref query, _broadphaseCandidates);
			snapshot2.ProcessBroadphaseRootCandidates(ref query, interpolateColliders: true, _broadphaseCandidates, snapshot, processedColliderIndices);
		}
		container = snapshot;
	}

	private void OverlapBoxBroadphase(ref Query query, HashSet<int> processedColliderIndices, out IHitboxColliderContainer container)
	{
		Assert.Check(query.Type == QueryType.BoxOverlap);
		GetClosestSnapshotForTick(query.Tick, out var snapshot);
		_broadphaseCandidates.Clear();
		snapshot.OverlapBoxBroadphase(ref query, _broadphaseCandidates);
		query.PreProcessingDelegate?.Invoke(ref query, _broadphaseCandidates, processedColliderIndices);
		snapshot.ProcessBroadphaseRootCandidates(ref query, interpolateColliders: false, _broadphaseCandidates, snapshot, processedColliderIndices);
		if ((query.Options & HitOptions.SubtickAccuracy) == HitOptions.SubtickAccuracy && query.TickTo.HasValue && query.Alpha.HasValue)
		{
			GetClosestSnapshotForTick(query.TickTo.Value, out var snapshot2);
			_broadphaseCandidates.Clear();
			snapshot2.OverlapBoxBroadphase(ref query, _broadphaseCandidates);
			snapshot2.ProcessBroadphaseRootCandidates(ref query, interpolateColliders: true, _broadphaseCandidates, snapshot, processedColliderIndices);
		}
		container = snapshot;
	}
}
