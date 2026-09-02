using System;
using System.Collections.Generic;
using Pathfinding.Serialization;
using UnityEngine;

namespace Pathfinding;

public abstract class GraphNode
{
	private int nodeIndex;

	protected uint flags;

	private const int NodeIndexMask = 268435455;

	private const int DestroyedNodeIndex = 268435454;

	private const int TemporaryFlag1Mask = 268435456;

	private const int TemporaryFlag2Mask = 536870912;

	public Int3 position;

	private const int FlagsWalkableOffset = 0;

	private const uint FlagsWalkableMask = 1u;

	private const int FlagsHierarchicalIndexOffset = 1;

	private const uint HierarchicalIndexMask = 262142u;

	private const int HierarchicalDirtyOffset = 18;

	private const uint HierarchicalDirtyMask = 262144u;

	private const int FlagsGraphOffset = 24;

	private const uint FlagsGraphMask = 4278190080u;

	public const uint MaxHierarchicalNodeIndex = 131071u;

	public const uint MaxGraphIndex = 255u;

	private const int FlagsTagOffset = 19;

	private const uint FlagsTagMask = 16252928u;

	public NavGraph Graph
	{
		get
		{
			if (!Destroyed)
			{
				return AstarData.GetGraph(this);
			}
			return null;
		}
	}

	public bool Destroyed => NodeIndex == 268435454;

	public int NodeIndex
	{
		get
		{
			return nodeIndex & 0xFFFFFFF;
		}
		private set
		{
			nodeIndex = (nodeIndex & -268435456) | value;
		}
	}

	internal bool TemporaryFlag1
	{
		get
		{
			return (nodeIndex & 0x10000000) != 0;
		}
		set
		{
			nodeIndex = (nodeIndex & -268435457) | (value ? 268435456 : 0);
		}
	}

	internal bool TemporaryFlag2
	{
		get
		{
			return (nodeIndex & 0x20000000) != 0;
		}
		set
		{
			nodeIndex = (nodeIndex & -536870913) | (value ? 536870912 : 0);
		}
	}

	public uint Flags
	{
		get
		{
			return flags;
		}
		set
		{
			flags = value;
		}
	}

	public uint Penalty
	{
		get
		{
			return 0u;
		}
		set
		{
		}
	}

	public bool Walkable
	{
		get
		{
			return (flags & 1) != 0;
		}
		set
		{
			flags = (flags & 0xFFFFFFFEu) | (uint)(value ? 1 : 0);
			AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
		}
	}

	internal int HierarchicalNodeIndex
	{
		get
		{
			return (int)((flags & 0x3FFFE) >> 1);
		}
		set
		{
			flags = (flags & 0xFFFC0001u) | (uint)(value << 1);
		}
	}

	internal bool IsHierarchicalNodeDirty
	{
		get
		{
			return (flags & 0x40000) != 0;
		}
		set
		{
			flags = (flags & 0xFFFBFFFFu) | (uint)((value ? 1 : 0) << 18);
		}
	}

	public uint Area => AstarPath.active.hierarchicalGraph.GetConnectedComponent(HierarchicalNodeIndex);

	public uint GraphIndex
	{
		get
		{
			return (flags & 0xFF000000u) >> 24;
		}
		set
		{
			flags = (flags & 0xFFFFFF) | (value << 24);
		}
	}

	public uint Tag
	{
		get
		{
			return (flags & 0xF80000) >> 19;
		}
		set
		{
			flags = (flags & 0xFF07FFFFu) | ((value << 19) & 0xF80000);
		}
	}

	protected GraphNode(AstarPath astar)
	{
		if ((object)astar != null)
		{
			nodeIndex = astar.GetNewNodeIndex();
			astar.InitializeNode(this);
			return;
		}
		throw new Exception("No active AstarPath object to bind to");
	}

	public void Destroy()
	{
		if (!Destroyed)
		{
			ClearConnections(alsoReverse: true);
			if (AstarPath.active != null)
			{
				AstarPath.active.DestroyNode(this);
			}
			NodeIndex = 268435454;
		}
	}

	public void SetConnectivityDirty()
	{
		AstarPath.active.hierarchicalGraph.AddDirtyNode(this);
	}

	[Obsolete("This method is deprecated because it never did anything, you can safely remove any calls to this method")]
	public void RecalculateConnectionCosts()
	{
	}

	public virtual void UpdateRecursiveG(Path path, PathNode pathNode, PathHandler handler)
	{
		pathNode.UpdateG(path);
		handler.heap.Add(pathNode);
		GetConnections((GraphNode other) =>
		{
			PathNode pathNode2 = handler.GetPathNode(other);
			if (pathNode2.parent == pathNode && pathNode2.pathID == handler.PathID)
			{
				other.UpdateRecursiveG(path, pathNode2, handler);
			}
		});
	}

	public abstract void GetConnections(Action<GraphNode> action);

	public abstract void AddConnection(GraphNode node, uint cost);

	public abstract void RemoveConnection(GraphNode node);

	public abstract void ClearConnections(bool alsoReverse);

	public virtual bool ContainsConnection(GraphNode node)
	{
		bool contains = false;
		GetConnections((GraphNode neighbour) =>
		{
			contains |= neighbour == node;
		});
		return contains;
	}

	public virtual bool GetPortal(GraphNode other, List<Vector3> left, List<Vector3> right, bool backwards)
	{
		return false;
	}

	public abstract void Open(Path path, PathNode pathNode, PathHandler handler);

	public virtual float SurfaceArea()
	{
		return 0f;
	}

	public virtual Vector3 RandomPointOnSurface()
	{
		return (Vector3)position;
	}

	public abstract Vector3 ClosestPointOnNode(Vector3 p);

	public virtual int GetGizmoHashCode()
	{
		return (int)((uint)position.GetHashCode() ^ (19 * Penalty) ^ (41 * (flags & 0xFFF80001u)));
	}

	public virtual void SerializeNode(GraphSerializationContext ctx)
	{
		ctx.writer.Write(Penalty);
		ctx.writer.Write(Flags & 0xFFF80001u);
	}

	public virtual void DeserializeNode(GraphSerializationContext ctx)
	{
		Penalty = ctx.reader.ReadUInt32();
		Flags = (ctx.reader.ReadUInt32() & 0xFFF80001u) | (Flags & 0x7FFFE);
		GraphIndex = ctx.graphIndex;
	}

	public virtual void SerializeReferences(GraphSerializationContext ctx)
	{
	}

	public virtual void DeserializeReferences(GraphSerializationContext ctx)
	{
	}
}
