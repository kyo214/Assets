using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Fusion.Sockets;

namespace Fusion;

[Serializable]
[StructLayout(LayoutKind.Explicit)]
[NetworkStructWeaved(1)]
public struct NetworkId : INetworkStruct, IEquatable<NetworkId>, IComparable, IComparable<NetworkId>
{
	public sealed class EqualityComparer : IEqualityComparer<NetworkId>
	{
		public bool Equals(NetworkId a, NetworkId b)
		{
			return a.Raw == b.Raw;
		}

		public int GetHashCode(NetworkId id)
		{
			return (int)id.Raw;
		}
	}

	public const int PREDICTED_BIT = int.MinValue;

	public const int BLOCK_SIZE = 8;

	public const int SIZE = 4;

	public const int ALIGNMENT = 4;

	[FieldOffset(0)]
	public uint Raw;

	public bool IsValid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return Raw != 0;
		}
	}

	internal static NetworkId InternalState => new NetworkId(1u);

	internal NetworkId(uint raw)
	{
		Raw = raw;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(NetworkId other)
	{
		return Raw == other.Raw;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int CompareTo(NetworkId other)
	{
		return (int)(Raw - other.Raw);
	}

	public override bool Equals(object obj)
	{
		return obj is NetworkId networkId && Raw == networkId.Raw;
	}

	int IComparable.CompareTo(object obj)
	{
		return (obj is NetworkId other) ? CompareTo(other) : 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(NetworkId a, NetworkId b)
	{
		return a.Raw == b.Raw;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(NetworkId a, NetworkId b)
	{
		return a.Raw != b.Raw;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator bool(NetworkId id)
	{
		return id.Raw != 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void Write(NetBitBuffer* buffer, NetworkId id)
	{
		buffer->WriteUInt32VarLength(id.Raw, 8);
	}

	public unsafe static NetworkId Read(NetBitBuffer* buffer, uint mask = 0u)
	{
		NetworkId result = default;
		result.Raw = buffer->ReadUInt32VarLength(8) | mask;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void Write(NetBitBuffer* buffer)
	{
		Write(buffer, this);
	}

	public override int GetHashCode()
	{
		return (int)Raw;
	}

	public override string ToString()
	{
		return IsValid ? $"[Id:{Raw}]" : "[Id:None]";
	}

	public string ToNamePrefixString()
	{
		return IsValid ? $"[{Raw}] " : "[Invalid] ";
	}
}
