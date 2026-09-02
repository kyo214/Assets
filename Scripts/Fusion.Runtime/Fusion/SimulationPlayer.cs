using System.Collections.Generic;
using Fusion.Sockets;
using UnityEngine;

namespace Fusion;

internal class SimulationPlayer
{
	public struct AOIQuery
	{
		public Vector3 Position;

		public float Radius;

		public int LayerMask;

		public unsafe static void Write(AOIQuery q, NetBitBuffer* b)
		{
			b->WriteInt32(q.LayerMask);
			b->WriteSingle(q.Radius);
			b->WriteSingle(q.Position.x);
			b->WriteSingle(q.Position.y);
			b->WriteSingle(q.Position.z);
		}

		public unsafe static AOIQuery Read(NetBitBuffer* b)
		{
			AOIQuery result = default;
			result.LayerMask = b->ReadInt32();
			result.Radius = b->ReadSingle();
			result.Position.x = b->ReadSingle();
			result.Position.y = b->ReadSingle();
			result.Position.z = b->ReadSingle();
			return result;
		}
	}

	public HashSet<NetworkId> AlwaysInterested = new HashSet<NetworkId>();

	public List<AOIQuery> AOIQueries = new List<AOIQuery>();

	public HashSet<NetworkId> AOIResult = new HashSet<NetworkId>();

	public void Reset()
	{
		AlwaysInterested.Clear();
		AOIQueries.Clear();
		AOIResult.Clear();
	}
}
