#define DEBUG
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Fusion.Sockets;

namespace Fusion;

[Serializable]
[StructLayout(LayoutKind.Explicit)]
[NetworkStructWeaved(1)]
public struct PlayerRef : INetworkStruct
{
	public const int SIZE = 4;

	[FieldOffset(0)]
	private int _index;

	public static PlayerRef None => default;

	public bool IsValid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _index > 0;
		}
	}

	public bool IsNone
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return _index == 0;
		}
	}

	public int RawEncoded => _index;

	public int PlayerId => this;

	public override bool Equals(object obj)
	{
		if (obj is PlayerRef playerRef)
		{
			return _index == playerRef._index;
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode()
	{
		return _index;
	}

	public override string ToString()
	{
		return (_index > 0) ? $"[Player:{_index - 1}]" : "[Player:None]";
	}

	public static implicit operator PlayerRef(int value)
	{
		PlayerRef result = default;
		result._index = value + 1;
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete("implicit cast of PlayerRef to bool will be removed in next stable release, please use PlayerRef.IsValid instead")]
	public static implicit operator bool(PlayerRef value)
	{
		return value._index > 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator int(PlayerRef value)
	{
		return value._index - 1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(PlayerRef a, PlayerRef b)
	{
		return a._index == b._index;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(PlayerRef a, PlayerRef b)
	{
		return a._index != b._index;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void Write(NetBitBuffer* buffer, PlayerRef playerRef)
	{
		if (buffer->WriteBoolean(playerRef.IsValid))
		{
			buffer->WriteInt32VarLength(playerRef);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static void Write<T>(T* buffer, PlayerRef playerRef) where T : unmanaged, INetBitWriteStream
	{
		if (buffer->WriteBoolean(playerRef.IsValid))
		{
			buffer->WriteInt32VarLength(playerRef);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe static PlayerRef Read(NetBitBuffer* buffer)
	{
		if (buffer->ReadBoolean())
		{
			PlayerRef result = buffer->ReadInt32VarLength();
			Assert.Check(result.IsValid);
			return result;
		}
		return default;
	}
}
