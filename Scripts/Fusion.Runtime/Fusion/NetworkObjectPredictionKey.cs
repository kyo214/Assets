using System;
using System.Runtime.InteropServices;

namespace Fusion;

[StructLayout(LayoutKind.Explicit)]
public struct NetworkObjectPredictionKey : INetworkStruct, IEquatable<NetworkObjectPredictionKey>
{
	[FieldOffset(0)]
	public byte Byte0;

	[FieldOffset(1)]
	public byte Byte1;

	[FieldOffset(2)]
	public byte Byte2;

	[FieldOffset(3)]
	public byte Byte3;

	[FieldOffset(0)]
	public int AsInt;

	[FieldOffset(0)]
	public float AsFloat;

	public bool Equals(NetworkObjectPredictionKey other)
	{
		return AsInt == other.AsInt;
	}

	public override string ToString()
	{
		return $"[PredictionKey Byte0={Byte0} Byte1={Byte1} Byte2={Byte2} Byte3={Byte3}]";
	}

	public override bool Equals(object obj)
	{
		return obj is NetworkObjectPredictionKey other && Equals(other);
	}

	public override int GetHashCode()
	{
		return AsInt.GetHashCode();
	}

	public static bool operator ==(NetworkObjectPredictionKey a, NetworkObjectPredictionKey b)
	{
		return a.AsInt == b.AsInt;
	}

	public static bool operator !=(NetworkObjectPredictionKey a, NetworkObjectPredictionKey b)
	{
		return a.AsInt != b.AsInt;
	}

	public static implicit operator NetworkObjectPredictionKey(PlayerRef player)
	{
		return new NetworkObjectPredictionKey
		{
			AsInt = player.RawEncoded
		};
	}

	public static implicit operator bool(NetworkObjectPredictionKey key)
	{
		return key.AsInt != 0;
	}
}
