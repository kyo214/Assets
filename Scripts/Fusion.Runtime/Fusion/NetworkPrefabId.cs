using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion;

[Serializable]
[StructLayout(LayoutKind.Explicit)]
[InlineHelp]
[NetworkStructWeaved(1)]
public struct NetworkPrefabId : INetworkStruct, IEquatable<NetworkPrefabId>
{
	public sealed class EqualityComparer : IEqualityComparer<NetworkPrefabId>
	{
		public bool Equals(NetworkPrefabId x, NetworkPrefabId y)
		{
			return x.Value == y.Value;
		}

		public int GetHashCode(NetworkPrefabId obj)
		{
			return (int)obj.Value;
		}
	}

	public const int SIZE = 4;

	public const int ALIGNMENT = 4;

	[FieldOffset(0)]
	public uint Value;

	public bool IsNone => Value == 0;

	public bool IsValid => Value != 0;

	public NetworkPrefabId(uint value)
	{
		Value = value;
	}

	public bool Equals(NetworkPrefabId other)
	{
		return Value == other.Value;
	}

	public override bool Equals(object obj)
	{
		return obj is NetworkPrefabId other && Equals(other);
	}

	public override int GetHashCode()
	{
		return (int)Value;
	}

	public override string ToString()
	{
		return $"[TypeId:{Value:X8}]";
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(NetworkPrefabId a, NetworkPrefabId b)
	{
		return a.Value == b.Value;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(NetworkPrefabId a, NetworkPrefabId b)
	{
		return a.Value != b.Value;
	}
}
