#define DEBUG
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Fusion;

internal struct BVHNode
{
	internal struct CachedBounds
	{
		public Vector3 Center;

		public Vector3 Extents;

		public Vector3 Min;

		public Vector3 Max;

		public CachedBounds(Bounds bounds)
		{
			Center = bounds.center;
			Extents = bounds.extents;
			Min = bounds.min;
			Max = bounds.max;
		}

		public CachedBounds(Vector3 center, Vector3 extents)
		{
			Center = center;
			Extents = extents;
			Min = center - extents;
			Max = center + extents;
		}
	}

	internal enum Rot
	{
		NONE = 0,
		L_RL = 1,
		L_RR = 2,
		R_LL = 3,
		R_LR = 4,
		LL_RR = 5,
		LL_RL = 6
	}

	internal const int MaxEntriesPerNode = 1;

	internal const int RootNodeIndex = 1;

	private static readonly HitboxRoot.HitboxComparerX ComparerX = new HitboxRoot.HitboxComparerX();

	private static readonly HitboxRoot.HitboxComparerY ComparerY = new HitboxRoot.HitboxComparerY();

	private static readonly HitboxRoot.HitboxComparerZ ComparerZ = new HitboxRoot.HitboxComparerZ();

	public Bounds Box;

	internal CachedBounds _cachedBounds;

	private int _nodeIndex;

	private int _parentIndex;

	private int _leftIndex;

	private int _rightIndex;

	internal bool Active;

	internal int Depth;

	internal bool Used;

	internal int Next;

	internal HitboxRoot _root;

	internal NetworkBehaviourId _rootId;

	internal bool _isLeaf;

	internal int Index => _nodeIndex;

	internal bool IsValid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _nodeIndex > 0;
		}
	}

	internal bool IsRootNode
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _nodeIndex == 1;
		}
	}

	internal bool HasParent
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _parentIndex > 0;
		}
	}

	internal bool HasLeft
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _leftIndex > 0;
		}
	}

	internal bool HasRight
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _rightIndex > 0;
		}
	}

	internal bool IsLeaf
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _isLeaf;
		}
	}

	internal bool HasValidRoot
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _root.Id == _rootId && BehaviourUtils.IsAlive(_root) && BehaviourUtils.IsAlive(_root.Object);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ref BVHNode GetParent(BVH bvh)
	{
		return ref bvh._nodes[_parentIndex];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ref BVHNode GetRight(BVH bvh)
	{
		return ref bvh._nodes[_rightIndex];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ref BVHNode GetLeft(BVH bvh)
	{
		return ref bvh._nodes[_leftIndex];
	}

	public override string ToString()
	{
		return $"BVHNode: {_nodeIndex}";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void RefitObjectChanged(BVH bvh)
	{
		Assert.Check(BehaviourUtils.IsAlive(_root));
		Active = _root.HitboxRootActive;
		if (Active && RefitVolume(bvh) && HasParent)
		{
			bvh.refitNodes.Add(_parentIndex);
		}
	}

	private void ExpandVolume(BVH bvh, Vector3 objectpos, float radius, ref Bounds bounds, bool expandParent)
	{
		bool flag = false;
		if (objectpos.x - radius < bounds.min.x)
		{
			bounds.min = new Vector3(objectpos.x - radius, bounds.min.y, bounds.min.z);
			flag = true;
		}
		if (objectpos.x + radius > bounds.max.x)
		{
			bounds.max = new Vector3(objectpos.x + radius, bounds.max.y, bounds.max.z);
			flag = true;
		}
		if (objectpos.y - radius < bounds.min.y)
		{
			bounds.min = new Vector3(bounds.min.x, objectpos.y - radius, bounds.min.z);
			flag = true;
		}
		if (objectpos.y + radius > bounds.max.y)
		{
			bounds.max = new Vector3(bounds.max.x, objectpos.y + radius, bounds.max.z);
			flag = true;
		}
		if (objectpos.z - radius < bounds.min.z)
		{
			bounds.min = new Vector3(bounds.min.x, bounds.min.y, objectpos.z - radius);
			flag = true;
		}
		if (objectpos.z + radius > bounds.max.z)
		{
			bounds.max = new Vector3(bounds.max.x, bounds.max.y, objectpos.z + radius);
			flag = true;
		}
		if ((expandParent & flag) && HasParent)
		{
			GetParent(bvh).ChildExpanded(bvh, ref this);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void AssignVolume(Vector3 pos, float radius, ref Bounds bounds)
	{
		bounds.min = new Vector3(pos.x - radius, pos.y - radius, pos.z - radius);
		bounds.max = new Vector3(pos.x + radius, pos.y + radius, pos.z + radius);
	}

	private void ComputeVolume(BVH bvh)
	{
		if (BehaviourUtils.IsAlive(_root))
		{
			AssignVolume(_root.transform.TransformPoint(_root.Offset), _root.BroadRadius, ref Box);
		}
		else
		{
			ChildRefit(bvh, Index, propagate: false);
		}
	}

	private Bounds ComputeMinVolume(BVH bvh)
	{
		if (BehaviourUtils.IsAlive(_root))
		{
			Bounds bounds = default;
			AssignVolume(_root.transform.TransformPoint(_root.Offset), _root.BroadRadius, ref bounds);
			return bounds;
		}
		Assert.Fail();
		ChildRefit(bvh, Index, propagate: false);
		return default;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private bool RefitVolume(BVH bvh)
	{
		Assert.Check(BehaviourUtils.IsAlive(_root));
		Bounds box = Box;
		Bounds bounds = ComputeMinVolume(bvh);
		if (!Box.ContainBounds(bounds))
		{
			Box = bounds;
			UpdateBoundsCache();
			if (HasParent)
			{
				GetParent(bvh).ChildRefit(bvh);
			}
			return true;
		}
		return false;
	}

	internal static float SA(Bounds box)
	{
		float num = box.max.x - box.min.x;
		float num2 = box.max.y - box.min.y;
		float num3 = box.max.z - box.min.z;
		return 2f * (num * num2 + num * num3 + num2 * num3);
	}

	internal static float SA(ref Bounds box)
	{
		float num = box.max.x - box.min.x;
		float num2 = box.max.y - box.min.y;
		float num3 = box.max.z - box.min.z;
		return 2f * (num * num2 + num * num3 + num2 * num3);
	}

	internal static float SA(ref BVHNode node)
	{
		float num = node.Box.max.x - node.Box.min.x;
		float num2 = node.Box.max.y - node.Box.min.y;
		float num3 = node.Box.max.z - node.Box.min.z;
		return 2f * (num * num2 + num * num3 + num2 * num3);
	}

	internal static Bounds AABBofPair(ref BVHNode nodea, ref BVHNode nodeb)
	{
		Bounds box = nodea.Box;
		box.Encapsulate(nodeb.Box);
		return box;
	}

	private static Bounds GetEntryBounds(HitboxRoot entry)
	{
		float broadRadius = entry.BroadRadius;
		return new Bounds
		{
			min = new Vector3(0f - broadRadius, 0f - broadRadius, 0f - broadRadius),
			max = new Vector3(broadRadius, broadRadius, broadRadius)
		};
	}

	private static float SAofList(List<HitboxRoot> entries)
	{
		Assert.Check(entries.Count > 0);
		Bounds box = GetEntryBounds(entries[0]);
		foreach (HitboxRoot entry in entries)
		{
			Bounds entryBounds = GetEntryBounds(entry);
			box.Encapsulate(entryBounds);
		}
		return SA(ref box);
	}

	internal void TryRotate(BVH bvh)
	{
		ref BVHNode left = ref GetLeft(bvh);
		ref BVHNode right = ref GetRight(bvh);
		if (left.IsLeaf && right.IsLeaf && HasParent)
		{
			bvh.refitNodes.Add(_parentIndex);
			return;
		}
		float num = SA(ref left) + SA(ref right);
		Rot rot = Rot.NONE;
		float num2 = num;
		ref BVHNode left2 = ref right.GetLeft(bvh);
		ref BVHNode right2 = ref right.GetRight(bvh);
		ref BVHNode left3 = ref left.GetLeft(bvh);
		ref BVHNode right3 = ref left.GetRight(bvh);
		if (!right.IsLeaf)
		{
			float num3 = SA(ref left2) + SA(AABBofPair(ref left, ref right2));
			float num4 = SA(ref right2) + SA(AABBofPair(ref left, ref left2));
			if (num3 < num2)
			{
				num2 = num3;
				rot = Rot.L_RL;
			}
			if (num4 < num2)
			{
				num2 = num4;
				rot = Rot.L_RR;
			}
		}
		if (!left.IsLeaf)
		{
			float num5 = SA(AABBofPair(ref right, ref right3)) + SA(ref left3);
			float num6 = SA(AABBofPair(ref right, ref left3)) + SA(ref right3);
			if (num5 < num2)
			{
				num2 = num5;
				rot = Rot.R_LL;
			}
			if (num6 < num2)
			{
				num2 = num6;
				rot = Rot.R_LR;
			}
		}
		if (!right.IsLeaf && !left.IsLeaf)
		{
			float num7 = SA(AABBofPair(ref right2, ref right3)) + SA(AABBofPair(ref left2, ref left3));
			float num8 = SA(AABBofPair(ref left2, ref right3)) + SA(AABBofPair(ref left3, ref right2));
			if (num7 < num2)
			{
				num2 = num7;
				rot = Rot.LL_RR;
			}
			if (num8 < num2)
			{
				num2 = num8;
				rot = Rot.LL_RL;
			}
		}
		if (rot == Rot.NONE)
		{
			if (HasParent && DateTime.Now.Ticks % 100 < 2)
			{
				bvh.refitNodes.Add(_parentIndex);
			}
			return;
		}
		if (HasParent)
		{
			bvh.refitNodes.Add(_parentIndex);
		}
		if (!((num - num2) / num < 0.3f))
		{
			switch (rot)
			{
			case Rot.L_RL:
			{
				int leftIndex4 = _leftIndex;
				_leftIndex = right._leftIndex;
				GetLeft(bvh)._parentIndex = _nodeIndex;
				right._leftIndex = leftIndex4;
				bvh._nodes[leftIndex4]._parentIndex = _rightIndex;
				right.ChildRefit(bvh, propagate: false);
				break;
			}
			case Rot.L_RR:
			{
				int leftIndex3 = _leftIndex;
				_leftIndex = right._rightIndex;
				GetLeft(bvh)._parentIndex = _nodeIndex;
				right._rightIndex = leftIndex3;
				bvh._nodes[leftIndex3]._parentIndex = _rightIndex;
				right.ChildRefit(bvh, propagate: false);
				break;
			}
			case Rot.R_LL:
			{
				int rightIndex2 = _rightIndex;
				_rightIndex = left._leftIndex;
				GetRight(bvh)._parentIndex = _nodeIndex;
				left._leftIndex = rightIndex2;
				bvh._nodes[rightIndex2]._parentIndex = _leftIndex;
				left.ChildRefit(bvh, propagate: false);
				break;
			}
			case Rot.R_LR:
			{
				int rightIndex = _rightIndex;
				_rightIndex = left._rightIndex;
				GetRight(bvh)._parentIndex = _nodeIndex;
				left._rightIndex = rightIndex;
				bvh._nodes[rightIndex]._parentIndex = _leftIndex;
				left.ChildRefit(bvh, propagate: false);
				break;
			}
			case Rot.LL_RR:
			{
				int leftIndex2 = left._leftIndex;
				left._leftIndex = right._rightIndex;
				right._rightIndex = leftIndex2;
				left3 = ref left.GetLeft(bvh);
				right2 = ref right.GetRight(bvh);
				left3._parentIndex = _leftIndex;
				bvh._nodes[leftIndex2]._parentIndex = _rightIndex;
				left.ChildRefit(bvh, propagate: false);
				right.ChildRefit(bvh, propagate: false);
				break;
			}
			case Rot.LL_RL:
			{
				int leftIndex = left._leftIndex;
				left._leftIndex = right._leftIndex;
				right._leftIndex = leftIndex;
				left3 = ref left.GetLeft(bvh);
				left2 = ref right.GetLeft(bvh);
				left3._parentIndex = _leftIndex;
				bvh._nodes[leftIndex]._parentIndex = _rightIndex;
				left.ChildRefit(bvh, propagate: false);
				right.ChildRefit(bvh, propagate: false);
				break;
			}
			default:
				throw new NotImplementedException("missing implementation for BVH Rotation .. " + rot);
			}
			Rot rot2 = rot;
			Rot rot3 = rot2;
			if ((uint)(rot3 - 1) <= 3u)
			{
				SetDepth(bvh, Depth);
			}
		}
	}

	internal void SplitNode(BVH bvh, List<HitboxRoot> entries)
	{
		Assert.Check(entries != null);
		Assert.Check(entries.Count > 1);
		int num = entries.Count / 2;
		entries.Sort(ComparerX);
		List<HitboxRoot> range = entries.GetRange(0, num);
		List<HitboxRoot> range2 = entries.GetRange(num, entries.Count - num);
		float num2 = SAofList(range) * (float)range.Count + SAofList(range2) * (float)range2.Count;
		float num3 = num2;
		List<HitboxRoot> entries2 = range;
		List<HitboxRoot> entries3 = range2;
		entries.Sort(ComparerY);
		range = entries.GetRange(0, num);
		range2 = entries.GetRange(num, entries.Count - num);
		num2 = SAofList(range) * (float)range.Count + SAofList(range2) * (float)range2.Count;
		if (num2 < num3 || (num2 == num3 && UnityEngine.Random.value > 0.67f))
		{
			num3 = num2;
			entries2 = range;
			entries3 = range2;
		}
		entries.Sort(ComparerZ);
		range = entries.GetRange(0, num);
		range2 = entries.GetRange(num, entries.Count - num);
		num2 = SAofList(range) * (float)range.Count + SAofList(range2) * (float)range2.Count;
		if (num2 < num3 || (num2 == num3 && UnityEngine.Random.value > 0.67f))
		{
			num3 = num2;
			entries2 = range;
			entries3 = range2;
		}
		ref BVHNode nextNode = ref bvh.GetNextNode(out _leftIndex);
		ref BVHNode nextNode2 = ref bvh.GetNextNode(out _rightIndex);
		InitNode(ref nextNode, bvh, _leftIndex, _nodeIndex, Depth + 1, entries2);
		InitNode(ref nextNode2, bvh, _rightIndex, _nodeIndex, Depth + 1, entries3);
		_isLeaf = false;
		Active = true;
	}

	private static void AddObjectPushdown(BVH bvh, ref BVHNode curNode, HitboxRoot entry)
	{
		ref BVHNode parent = ref curNode.GetParent(bvh);
		bool flag = parent._rightIndex == curNode.Index;
		bvh.ReusableList.Clear();
		bvh.ReusableList.Add(entry);
		ref BVHNode nextNode = ref bvh.GetNextNode(out var index);
		InitNode(ref nextNode, bvh, index, parent.Index, curNode.Depth);
		ref BVHNode nextNode2 = ref bvh.GetNextNode(out var index2);
		InitNode(ref nextNode2, bvh, index2, index, nextNode.Depth + 1, bvh.ReusableList);
		if (flag)
		{
			parent._rightIndex = index;
		}
		else
		{
			parent._leftIndex = index;
		}
		curNode._parentIndex = index;
		curNode.Depth = nextNode.Depth + 1;
		nextNode._leftIndex = curNode.Index;
		nextNode._rightIndex = nextNode2.Index;
		ChildRefit(bvh, index);
	}

	internal static void Add(BVH bvh, ref BVHNode startNode, HitboxRoot entry, ref Bounds newObBox, float newObSah)
	{
		Assert.Check(BehaviourUtils.IsAlive(entry));
		if ((!startNode.HasLeft || !startNode.HasRight) && startNode.IsRootNode)
		{
			Assert.Check(!startNode.HasLeft && !startNode.HasRight);
			bvh.ReusableList.Clear();
			bvh.ReusableList.Add(entry);
			Assert.Check(startNode._isLeaf == BehaviourUtils.IsAlive(startNode._root));
			if (startNode._isLeaf)
			{
				bvh.ReusableList.Add(startNode._root);
			}
			InitNode(ref startNode, bvh, startNode.Index, 0, 0, bvh.ReusableList);
			return;
		}
		ref BVHNode reference = ref startNode;
		while (!reference.IsLeaf)
		{
			if (!reference.HasLeft || !reference.HasRight)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(string.Format("Trying to add '{0}' to a {1}({2}) that does not have a Left({3}) and/or Right({4}) children and is not a Leaf. Interrupting operation to avoid an infinite loop.", entry, "BVHNode", reference.Index, reference._leftIndex, reference._rightIndex));
				bvh.BuildNodesLog(stringBuilder);
				throw new InvalidOperationException(stringBuilder.ToString());
			}
			ref BVHNode left = ref reference.GetLeft(bvh);
			ref BVHNode right = ref reference.GetRight(bvh);
			float num = SA(ref left);
			float num2 = SA(ref right);
			Bounds box = new Bounds
			{
				min = left.Box.min,
				max = left.Box.max
			};
			Bounds box2 = new Bounds
			{
				min = right.Box.min,
				max = right.Box.max
			};
			box.Encapsulate(newObBox);
			box2.Encapsulate(newObBox);
			float num3 = num2 + SA(box);
			float num4 = num + SA(box2);
			float num5 = SA(AABBofPair(ref left, ref right)) + newObSah;
			reference = ref !(num3 < num4) ? ref right : ref left;
		}
		Assert.Check(!reference.IsRootNode);
		AddObjectPushdown(bvh, ref reference, entry);
	}

	internal int NodesCount(BVH bvh)
	{
		if (BehaviourUtils.IsAlive(_root))
		{
			return 1;
		}
		return GetLeft(bvh).NodesCount(bvh) + GetRight(bvh).NodesCount(bvh);
	}

	internal void Remove(BVH bvh, HitboxRoot entry)
	{
		if (BehaviourUtils.IsNotAlive(_root))
		{
			throw new Exception("removeObject() called on nonLeaf!");
		}
		Assert.Check(BehaviourUtils.IsSame(_root, entry));
		bvh.Mapper.DeRegister(entry);
		if (IsRootNode)
		{
			InitNode(ref bvh.GetNode(_nodeIndex), bvh, _nodeIndex, 0, 0);
			return;
		}
		Assert.Check(HasParent);
		GetParent(bvh).RemoveLeaf(bvh, Index);
	}

	private void SetDepth(BVH bvh, int newdepth)
	{
		Depth = newdepth;
		if (newdepth > bvh.maxDepth)
		{
			bvh.maxDepth = newdepth;
		}
		if (HasLeft)
		{
			GetLeft(bvh).SetDepth(bvh, newdepth + 1);
		}
		if (HasRight)
		{
			GetRight(bvh).SetDepth(bvh, newdepth + 1);
		}
	}

	private void RemoveLeaf(BVH bvh, int removeIndex)
	{
		Assert.Check(!IsLeaf);
		if (removeIndex != _leftIndex && removeIndex != _rightIndex)
		{
			throw new Exception("removeLeaf doesn't match any leaf!");
		}
		ref BVHNode reference = ref this;
		ref BVHNode reference2 = ref this;
		if (removeIndex == _leftIndex)
		{
			reference = ref GetLeft(bvh);
			reference2 = ref GetRight(bvh);
			_leftIndex = 0;
		}
		else if (removeIndex == _rightIndex)
		{
			reference = ref GetRight(bvh);
			reference2 = ref GetLeft(bvh);
			_rightIndex = 0;
		}
		bvh.DisposeNode(removeIndex);
		Assert.Check(reference.IsLeaf);
		if (IsRootNode)
		{
			this = reference2;
			_nodeIndex = 1;
			_parentIndex = 0;
			Depth = 0;
			if (HasLeft)
			{
				ref BVHNode left = ref GetLeft(bvh);
				left._parentIndex = _nodeIndex;
				left.SetDepth(bvh, Depth + 1);
			}
			if (HasRight)
			{
				ref BVHNode right = ref GetRight(bvh);
				right._parentIndex = _nodeIndex;
				right.SetDepth(bvh, Depth + 1);
			}
			if (_isLeaf)
			{
				bvh.Mapper.DeRegister(_root);
				bvh.Mapper.RegisterMapping(_root, _nodeIndex);
			}
			bvh.DisposeNode(reference2.Index);
		}
		else
		{
			Assert.Check(HasParent);
			ref BVHNode parent = ref GetParent(bvh);
			Assert.Check(Index == parent._leftIndex || Index == parent._rightIndex);
			if (Index == parent._leftIndex)
			{
				parent._leftIndex = reference2.Index;
			}
			else
			{
				parent._rightIndex = reference2.Index;
			}
			Assert.Check(reference2.IsValid);
			reference2._parentIndex = _parentIndex;
			reference2.SetDepth(bvh, reference2.Depth - 1);
			Assert.Check(reference2.HasParent);
			reference2.GetParent(bvh).ChildRefit(bvh);
			bvh.DisposeNode(Index);
		}
	}

	internal void FindOverlappingLeaves(BVH bvh, Vector3 origin, float radius, List<BVHNode> overlapList)
	{
		if (BoundsIntersectsSphere(ToBounds(), origin, radius))
		{
			if (BehaviourUtils.IsAlive(_root))
			{
				overlapList.Add(this);
				return;
			}
			GetLeft(bvh).FindOverlappingLeaves(bvh, origin, radius, overlapList);
			GetRight(bvh).FindOverlappingLeaves(bvh, origin, radius, overlapList);
		}
	}

	private bool BoundsIntersectsSphere(Bounds bounds, Vector3 origin, float radius)
	{
		if (origin.x + radius < bounds.min.x || origin.y + radius < bounds.min.y || origin.z + radius < bounds.min.z || origin.x - radius > bounds.max.x || origin.y - radius > bounds.max.y || origin.z - radius > bounds.max.z)
		{
			return false;
		}
		return true;
	}

	internal void FindOverlappingLeaves(BVH bvh, Bounds aabb, List<BVHNode> overlapList)
	{
		if (ToBounds().Intersects(aabb))
		{
			if (BehaviourUtils.IsAlive(_root))
			{
				overlapList.Add(this);
				return;
			}
			GetLeft(bvh).FindOverlappingLeaves(bvh, aabb, overlapList);
			GetRight(bvh).FindOverlappingLeaves(bvh, aabb, overlapList);
		}
	}

	internal Bounds ToBounds()
	{
		return new Bounds
		{
			min = new Vector3(Box.min.x, Box.min.y, Box.min.z),
			max = new Vector3(Box.max.x, Box.max.y, Box.max.z)
		};
	}

	internal void ChildExpanded(BVH bvh, ref BVHNode child)
	{
		bool flag = false;
		if (child.Box.min.x < Box.min.x)
		{
			Box.min = new Vector3(child.Box.min.x, Box.min.y, Box.min.z);
			flag = true;
		}
		if (child.Box.max.x > Box.max.x)
		{
			Box.max = new Vector3(child.Box.max.x, Box.max.y, Box.max.z);
			flag = true;
		}
		if (child.Box.min.y < Box.min.y)
		{
			Box.min = new Vector3(Box.min.x, child.Box.min.y, Box.min.z);
			flag = true;
		}
		if (child.Box.max.y > Box.max.y)
		{
			Box.max = new Vector3(Box.max.x, child.Box.max.y, Box.max.z);
			flag = true;
		}
		if (child.Box.min.z < Box.min.z)
		{
			Box.min = new Vector3(Box.min.x, Box.min.y, child.Box.min.z);
			flag = true;
		}
		if (child.Box.max.z > Box.max.z)
		{
			Box.max = new Vector3(Box.max.x, Box.max.y, child.Box.max.z);
			flag = true;
		}
		if (flag)
		{
			UpdateBoundsCache();
		}
		if (flag && HasParent)
		{
			GetParent(bvh).ChildExpanded(bvh, ref this);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void UpdateBoundsCache()
	{
		_cachedBounds = new CachedBounds(Box);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal void ChildRefit(BVH bvh, bool propagate = true)
	{
		ChildRefit(bvh, Index, propagate);
	}

	internal static void ChildRefit(BVH bvh, int nodeIndex, bool propagate = true)
	{
		do
		{
			ref BVHNode node = ref bvh.GetNode(nodeIndex);
			ref BVHNode left = ref node.GetLeft(bvh);
			ref BVHNode right = ref node.GetRight(bvh);
			Bounds box = left.Box;
			if (right.Box.min.x < box.min.x)
			{
				box.min = new Vector3(right.Box.min.x, box.min.y, box.min.z);
			}
			if (right.Box.min.y < box.min.y)
			{
				box.min = new Vector3(box.min.x, right.Box.min.y, box.min.z);
			}
			if (right.Box.min.z < box.min.z)
			{
				box.min = new Vector3(box.min.x, box.min.y, right.Box.min.z);
			}
			if (right.Box.max.x > box.max.x)
			{
				box.max = new Vector3(right.Box.max.x, box.max.y, box.max.z);
			}
			if (right.Box.max.y > box.max.y)
			{
				box.max = new Vector3(box.max.x, right.Box.max.y, box.max.z);
			}
			if (right.Box.max.z > box.max.z)
			{
				box.max = new Vector3(box.max.x, box.max.y, right.Box.max.z);
			}
			if (!node.Box.ContainBounds(box))
			{
				node.Box = box;
				node.UpdateBoundsCache();
				nodeIndex = node._parentIndex;
			}
			else
			{
				nodeIndex = 0;
			}
		}
		while (propagate && nodeIndex != 0);
	}

	internal static void InitNode(ref BVHNode node, BVH bvh, int index, int parentIndex, int curDepth, List<HitboxRoot> entries = null)
	{
		node._nodeIndex = index;
		node._parentIndex = parentIndex;
		node._leftIndex = 0;
		node._rightIndex = 0;
		node._root = null;
		node._rootId = NetworkBehaviourId.None;
		node._isLeaf = false;
		node.Active = true;
		node.Depth = curDepth;
		if (bvh.maxDepth < node.Depth)
		{
			bvh.maxDepth = node.Depth;
		}
		node.Box = default;
		if (entries != null && entries.Count >= 1)
		{
			if (entries.Count <= 1)
			{
				Assert.Check(entries.Count == 1);
				node._root = entries[0];
				node._rootId = node._root.Id;
				node.Active = node._root.HitboxRootActive;
				node._isLeaf = true;
				bvh.Mapper.RegisterMapping(node._root, node.Index);
				node.Box = node.ComputeMinVolume(bvh);
				node.Box.Expand(node.Box.size * bvh.ExpansionFactor);
				node.UpdateBoundsCache();
			}
			else
			{
				node.SplitNode(bvh, entries);
				node.ChildRefit(bvh, propagate: false);
			}
		}
	}

	public void BuildLog(StringBuilder builder)
	{
		builder.Append($"Index: {_nodeIndex}");
		builder.Append($", Active: {Active}");
		builder.Append($", Used: {Used}");
		builder.Append($", Next: {Next}");
		builder.Append($", Depth: {Depth}");
		builder.Append(", Root: '" + (BehaviourUtils.IsAlive(_root) ? _root.name : "NULL") + "'");
		builder.Append($", Parent: {_parentIndex}");
		builder.Append($", IsLeaf: {_isLeaf}");
		builder.Append($", Left: {_leftIndex}");
		builder.Append($", Right {_rightIndex}");
	}
}
