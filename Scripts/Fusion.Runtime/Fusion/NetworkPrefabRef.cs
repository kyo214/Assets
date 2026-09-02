using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Fusion;

[Serializable]
[StructLayout(LayoutKind.Explicit)]
[NetworkStructWeaved(4)]
public struct NetworkPrefabRef : INetworkStruct, IEquatable<NetworkPrefabRef>, IComparable<NetworkPrefabRef>
{
	public sealed class EqualityComparer : IEqualityComparer<NetworkPrefabRef>
	{
		public bool Equals(NetworkPrefabRef x, NetworkPrefabRef y)
		{
			return x.Equals(y);
		}

		public int GetHashCode(NetworkPrefabRef obj)
		{
			return obj.GetHashCode();
		}
	}

	public const int SIZE = 16;

	public const int ALIGNMENT = 4;

	[FieldOffset(0)]
	public unsafe fixed long RawGuidValue[2];

	[FieldOffset(0)]
	[NonSerialized]
	private long _data0;

	[FieldOffset(8)]
	[NonSerialized]
	private long _data1;

	public static NetworkPrefabRef Empty => default;

	public bool IsValid => _data0 != 0L && _data1 != 0;

	public NetworkPrefabRef(string guid)
		: base(guid)
	{
	}

	public NetworkPrefabRef(byte[] guid)
	{
		_data0 = BitConverter.ToInt64(guid, 0);
		_data1 = BitConverter.ToInt64(guid, 8);
	}

	public unsafe NetworkPrefabRef(byte* guid)
	{
		_data0 = *(long*)guid;
		_data1 = ((long*)guid)[1];
	}

	public unsafe static implicit operator NetworkPrefabRef(Guid guid)
	{
		NetworkPrefabRef result = default;
		NetworkObjectGuidUtils.CopyAndMangleGuid((byte*)(&guid), (byte*)(&result));
		return result;
	}

	public unsafe static implicit operator Guid(NetworkPrefabRef guid)
	{
		Guid result = default;
		NetworkObjectGuidUtils.CopyAndMangleGuid((byte*)(&guid), (byte*)(&result));
		return result;
	}

	public static bool TryParse(string str, out NetworkPrefabRef guid)
	{
		if (Guid.TryParse(str, out var result))
		{
			guid = result;
			return true;
		}
		guid = default;
		return false;
	}

	public static NetworkPrefabRef Parse(string str)
	{
		return Guid.Parse(str);
	}

	public static bool operator ==(NetworkPrefabRef a, NetworkPrefabRef b)
	{
		return a.Equals(b);
	}

	public static bool operator !=(NetworkPrefabRef a, NetworkPrefabRef b)
	{
		return !a.Equals(b);
	}

	public bool Equals(NetworkPrefabRef other)
	{
		return _data0 == other._data0 && _data1 == other._data1;
	}

	public override bool Equals(object obj)
	{
		return obj is NetworkPrefabRef other && Equals(other);
	}

	public override int GetHashCode()
	{
		return ((Guid)this/*cast due to constrained. prefix*/).GetHashCode();
	}

	public override string ToString()
	{
		return ((Guid)this/*cast due to constrained. prefix*/).ToString();
	}

	public string ToUnityGuidString()
	{
		return ToString("N");
	}

	public string ToString(string format)
	{
		return ((Guid)this).ToString(format);
	}

	public int CompareTo(NetworkPrefabRef other)
	{
		long num = _data0 - other._data0;
		if (num == 0)
		{
			num = _data1 - other._data1;
			if (num == 0)
			{
				return 0;
			}
		}
		if (num < 0)
		{
			return -1;
		}
		return 1;
	}
}
