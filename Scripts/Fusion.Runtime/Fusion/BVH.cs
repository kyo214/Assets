#define DEBUG
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Fusion;

internal class BVH : ILagCompensationBroadphase
{
	internal BVHNode[] _nodes;

	internal Mapper Mapper;

	internal int maxDepth = 0;

	internal HashSet<int> refitNodes = new HashSet<int>();

	internal readonly List<HitboxRoot> ReusableList = new List<HitboxRoot>(2);

	private int _nodesCount = 1;

	private int _usedNodesCount = 0;

	private int _freeNodesHead = 0;

	private const float DEFAULT_EXPANSION_FACTOR = 0.1f;

	internal float ExpansionFactor;

	private List<int> _sweepNodes = new List<int>();

	internal ref BVHNode rootBVH => ref _nodes[1];

	internal int UsedNodesCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _usedNodesCount;
		}
	}

	public void CopyFrom(ILagCompensationBroadphase other)
	{
		BVH bVH = (BVH)other;
		Array.Clear(_nodes, 0, _nodes.Length);
		Array.Copy(bVH._nodes, 0, _nodes, 0, bVH._nodesCount);
		maxDepth = bVH.maxDepth;
		_nodesCount = bVH._nodesCount;
		_usedNodesCount = bVH._usedNodesCount;
		_freeNodesHead = bVH._freeNodesHead;
	}

	internal ref BVHNode GetNextNode(out int index)
	{
		if (_freeNodesHead == 0)
		{
			index = _nodesCount++;
		}
		else
		{
			index = _freeNodesHead;
			_freeNodesHead = _nodes[_freeNodesHead].Next;
		}
		ref BVHNode reference = ref _nodes[index];
		Assert.Check(!reference.Used, "Retrieving a node that is already marked as used.", index);
		reference = default;
		reference.Used = true;
		_usedNodesCount++;
		return ref reference;
	}

	internal void DisposeNode(int index)
	{
		Assert.Check(_nodes[index].Used, "Disposing a node that is not marked as Used.", index);
		ref BVHNode reference = ref _nodes[index];
		reference.Used = false;
		reference.Next = _freeNodesHead;
		_freeNodesHead = index;
		_usedNodesCount--;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal ref BVHNode GetNode(int index)
	{
		return ref _nodes[index];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Update(HitboxRoot changed, int tick)
	{
		if (Mapper.TryGetLeafIndex(changed, out var index))
		{
			ref BVHNode node = ref GetNode(index);
			Assert.Check(BehaviourUtils.IsSame(node._root, changed));
			node.RefitObjectChanged(this);
		}
	}

	public void Traverse(IBoundsTraversalTest hitTest, List<HitboxRoot> candidateRoots, int layerMask)
	{
		TraverseInternal(ref rootBVH, hitTest, candidateRoots, layerMask);
	}

	private void TraverseInternal(ref BVHNode curNode, IBoundsTraversalTest hitTest, List<HitboxRoot> candidateRoots, int layermask)
	{
		if (curNode.IsValid && hitTest.Check(ref curNode._cachedBounds))
		{
			if (!curNode.IsLeaf)
			{
				TraverseInternal(ref curNode.GetLeft(this), hitTest, candidateRoots, layermask);
				TraverseInternal(ref curNode.GetRight(this), hitTest, candidateRoots, layermask);
			}
			else if (curNode.HasValidRoot && curNode.Active)
			{
				candidateRoots.Add(curNode._root);
			}
		}
	}

	public void Optimize()
	{
		bool flag = false;
		while (refitNodes.Count > 0)
		{
			int num = -1;
			foreach (int refitNode in refitNodes)
			{
				BVHNode node = GetNode(refitNode);
				if (node.Depth > num)
				{
					num = node.Depth;
				}
			}
			_sweepNodes.Clear();
			foreach (int refitNode2 in refitNodes)
			{
				BVHNode node2 = GetNode(refitNode2);
				if (node2.Depth == num)
				{
					_sweepNodes.Add(refitNode2);
				}
			}
			foreach (int sweepNode in _sweepNodes)
			{
				refitNodes.Remove(sweepNode);
				GetNode(sweepNode).TryRotate(this);
			}
		}
	}

	public void Add(HitboxRoot root)
	{
		Bounds box = root.GetBounds();
		float newObSah = BVHNode.SA(ref box);
		BVHNode.Add(this, ref rootBVH, root, ref box, newObSah);
	}

	internal static Bounds BoundsFromSphere(Vector3 pos, float radius)
	{
		return new Bounds
		{
			min = new Vector3(pos.x - radius, pos.y - radius, pos.z - radius),
			max = new Vector3(pos.x + radius, pos.y + radius, pos.z + radius)
		};
	}

	public bool Remove(HitboxRoot root)
	{
		if (Mapper.TryGetLeafIndex(root, out var index))
		{
			GetNode(index).Remove(this, root);
			return true;
		}
		return false;
	}

	internal BVH(Mapper mapper, int nodesCapacity, List<HitboxRoot> initialEntries = null, float expansionFactor = 0.1f)
	{
		_nodes = new BVHNode[Mathf.Max(32, nodesCapacity)];
		Mapper = mapper;
		ExpansionFactor = expansionFactor;
		ref BVHNode nextNode = ref GetNextNode(out var index);
		Assert.Check(index == 1);
		BVHNode.InitNode(ref nextNode, this, index, 0, 0, initialEntries);
		Assert.Check(nextNode.IsRootNode);
	}

	public void RenderGizmos(Color color)
	{
		RenderGizmos(ref rootBVH, color, 0f);
		Gizmos.color = Color.white;
	}

	private void RenderGizmos(ref BVHNode n, Color color, float depth)
	{
		if (n.Active)
		{
			Gizmos.color = color + Color.red * (depth / 4f);
			Gizmos.DrawWireCube(n.Box.center, n.Box.size);
		}
		if (n.HasRight)
		{
			RenderGizmos(ref n.GetRight(this), color, depth + 1f);
		}
		if (n.HasLeft)
		{
			RenderGizmos(ref n.GetLeft(this), color, depth + 1f);
		}
	}

	internal void BuildNodesLog(StringBuilder builder)
	{
		builder.AppendLine($"Nodes count: {_nodesCount}, Used nodes: {UsedNodesCount}");
		for (int i = 0; i < _nodesCount; i++)
		{
			builder.Append($"[{i}]: ");
			_nodes[i].BuildLog(builder);
			builder.AppendLine();
		}
	}
}
