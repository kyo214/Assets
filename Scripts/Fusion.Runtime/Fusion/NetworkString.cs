using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace Fusion;

[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 4)]
[DebuggerDisplay("{Value}")]
[NetworkStructWeaved(1, true)]
public struct NetworkString<Size> : INetworkString, INetworkStruct, IEquatable<NetworkString<Size>>, IEnumerable<char>, IEnumerable where Size : unmanaged, IFixedStorage
{
	[SerializeField]
	internal int _length;

	[SerializeField]
	internal Size _data;

	public unsafe int Capacity => sizeof(Size) / 4;

	public string Value
	{
		get
		{
			string cache = null;
			Get(ref cache);
			return cache;
		}
		set
		{
			Set(value);
		}
	}

	public int Length => _length;

	public unsafe ref uint this[int index]
	{
		get
		{
			fixed (Size* data = &_data)
			{
				return ref *(uint*)((byte*)data + (nint)SafeIndex(index) * (nint)4);
			}
		}
	}

	private int SafeLength
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			if (_length < 0 || _length > Capacity)
			{
				throw new InvalidOperationException($"Invalid Length: {_length}");
			}
			return _length;
		}
	}

	public static implicit operator NetworkString<Size>(string str)
	{
		NetworkString<Size> result = default;
		result.Set(str);
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(NetworkString<Size> a, NetworkString<Size> b)
	{
		return !a.Equals(ref b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(string a, NetworkString<Size> b)
	{
		return !b.Equals(a);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(NetworkString<Size> a, string b)
	{
		return !a.Equals(b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(NetworkString<Size> a, NetworkString<Size> b)
	{
		return a.Equals(ref b);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(string a, NetworkString<Size> b)
	{
		return b.Equals(a);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(NetworkString<Size> a, string b)
	{
		return a.Equals(b);
	}

	public unsafe bool Get(ref string cache)
	{
		if (cache != null && Compare(cache) == 0)
		{
			return false;
		}
		int safeLength = SafeLength;
		if (safeLength == 0)
		{
			cache = string.Empty;
		}
		else
		{
			fixed (Size* data = &_data)
			{
				cache = new string((sbyte*)data, 0, safeLength * 4, Encoding.UTF32);
			}
		}
		return true;
	}

	public unsafe bool Set(string value)
	{
		value = value ?? string.Empty;
		fixed (char* ptr = value)
		{
			fixed (Size* data = &_data)
			{
				UTF32Tools.ConversionResult conversionResult = UTF32Tools.Convert(value, (uint*)data, Capacity);
				_length = conversionResult.CodePointCount;
				return conversionResult.CharacterCount == value.Length;
			}
		}
	}

	public int IndexOf(char c, int startIndex = 0)
	{
		return IndexOf((uint)c, startIndex, Length - startIndex);
	}

	public int IndexOf(char c, int startIndex, int count)
	{
		return IndexOf((uint)c, startIndex, count);
	}

	public int IndexOf(uint codePoint, int startIndex = 0)
	{
		return IndexOf(codePoint, startIndex, Length - startIndex);
	}

	public unsafe int IndexOf(uint codePoint, int startIndex, int count)
	{
		int safeLength = SafeLength;
		if (startIndex < 0 || startIndex > safeLength)
		{
			throw new ArgumentOutOfRangeException("startIndex");
		}
		if (count < 0 || startIndex + count > safeLength)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		fixed (Size* data = &_data)
		{
			uint* ptr = (uint*)data + startIndex;
			for (int i = 0; i < count; i++)
			{
				if (ptr[i] == codePoint)
				{
					return startIndex + i;
				}
			}
			return -1;
		}
	}

	public int IndexOf(string str, int startIndex = 0)
	{
		return IndexOf(str, startIndex, SafeLength - startIndex);
	}

	public unsafe int IndexOf(string str, int startIndex, int count)
	{
		if (str == null)
		{
			throw new ArgumentNullException("str");
		}
		if (startIndex < 0 || startIndex > SafeLength)
		{
			throw new ArgumentOutOfRangeException("startIndex");
		}
		if (count < 0 || startIndex + count > SafeLength)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		if (count < str.Length)
		{
		}
		fixed (Size* data = &_data)
		{
			int num = UTF32Tools.IndexOf((uint*)data + startIndex, count, str);
			if (num < 0)
			{
				return num;
			}
			return num + startIndex;
		}
	}

	public int IndexOf<OtherSize>(NetworkString<OtherSize> str, int startIndex = 0) where OtherSize : unmanaged, IFixedStorage
	{
		return IndexOf(ref str, startIndex, SafeLength - startIndex);
	}

	public int IndexOf<OtherSize>(NetworkString<OtherSize> str, int startIndex, int count) where OtherSize : unmanaged, IFixedStorage
	{
		return IndexOf(ref str, startIndex, count);
	}

	public int IndexOf<OtherSize>(ref NetworkString<OtherSize> str, int startIndex = 0) where OtherSize : unmanaged, IFixedStorage
	{
		return IndexOf(ref str, startIndex, SafeLength - startIndex);
	}

	public unsafe int IndexOf<OtherSize>(ref NetworkString<OtherSize> str, int startIndex, int count) where OtherSize : unmanaged, IFixedStorage
	{
		if (startIndex < 0 || startIndex > SafeLength)
		{
			throw new ArgumentOutOfRangeException("startIndex");
		}
		if (count < 0 || startIndex + count > SafeLength)
		{
			throw new ArgumentOutOfRangeException("count");
		}
		if (count < str.SafeLength)
		{
			return -1;
		}
		fixed (OtherSize* data = &str._data)
		{
			fixed (Size* data2 = &_data)
			{
				int num = UTF32Tools.IndexOf((uint*)data2 + startIndex, count, (uint*)data, str.SafeLength);
				if (num < 0)
				{
					return num;
				}
				return num + startIndex;
			}
		}
	}

	public bool Contains(char c)
	{
		return IndexOf(c) >= 0;
	}

	public bool Contains(uint codePoint)
	{
		return IndexOf(codePoint) >= 0;
	}

	public bool Contains(string str)
	{
		return IndexOf(str) >= 0;
	}

	public bool Contains<OtherSize>(NetworkString<OtherSize> str) where OtherSize : unmanaged, IFixedStorage
	{
		return IndexOf(ref str) >= 0;
	}

	public bool Contains<OtherSize>(ref NetworkString<OtherSize> str) where OtherSize : unmanaged, IFixedStorage
	{
		return IndexOf(ref str) >= 0;
	}

	public NetworkString<Size> Substring(int startIndex)
	{
		return Substring(startIndex, SafeLength - startIndex);
	}

	public unsafe NetworkString<Size> Substring(int startIndex, int length)
	{
		if (startIndex < 0 || startIndex >= SafeLength)
		{
			throw new ArgumentOutOfRangeException("startIndex");
		}
		if (length < 0 || startIndex + length > SafeLength)
		{
			throw new ArgumentOutOfRangeException("length");
		}
		NetworkString<Size> result = default;
		fixed (Size* data = &_data)
		{
			result._length = length;
			Native.MemCpy(&result._data, (byte*)data + (nint)startIndex * (nint)4, length * 4);
		}
		return result;
	}

	public unsafe NetworkString<Size> ToLower()
	{
		NetworkString<Size> result = default;
		fixed (Size* data = &_data)
		{
			UTF32Tools.ToLowerInvariant((uint*)data, (uint*)(&result._data), SafeLength);
			result._length = SafeLength;
		}
		return result;
	}

	public unsafe NetworkString<Size> ToUpper()
	{
		NetworkString<Size> result = default;
		fixed (Size* data = &_data)
		{
			UTF32Tools.ToUpperInvariant((uint*)data, (uint*)(&result._data), SafeLength);
			result._length = SafeLength;
		}
		return result;
	}

	public unsafe int GetCharCount()
	{
		fixed (Size* data = &_data)
		{
			return Encoding.UTF32.GetCharCount((byte*)data, Length * 4);
		}
	}

	public unsafe int Compare(string s)
	{
		if (s == null)
		{
			throw new ArgumentNullException("s");
		}
		fixed (Size* data = &_data)
		{
			return UTF32Tools.CompareOrdinal(s, (uint*)data, SafeLength);
		}
	}

	public unsafe int Compare(NetworkString<Size> s)
	{
		fixed (Size* data = &_data)
		{
			return UTF32Tools.CompareOrdinal((uint*)data, SafeLength, (uint*)(&s._data), s.SafeLength, ignoreCase: false);
		}
	}

	public unsafe int Compare(ref NetworkString<Size> s)
	{
		fixed (Size* data = &_data)
		{
			fixed (Size* data2 = &s._data)
			{
				return UTF32Tools.CompareOrdinal((uint*)data, SafeLength, (uint*)data2, s.SafeLength, ignoreCase: false);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int Compare<OtherSize>(NetworkString<OtherSize> other) where OtherSize : unmanaged, IFixedStorage
	{
		return Compare(ref other);
	}

	public unsafe int Compare<OtherSize>(ref NetworkString<OtherSize> other) where OtherSize : unmanaged, IFixedStorage
	{
		fixed (OtherSize* data = &other._data)
		{
			fixed (Size* data2 = &_data)
			{
				return UTF32Tools.CompareOrdinal((uint*)data2, SafeLength, (uint*)data, other.SafeLength, ignoreCase: false);
			}
		}
	}

	public bool Equals(string s)
	{
		return Compare(s) == 0;
	}

	public override bool Equals(object obj)
	{
		if (obj is INetworkString networkString)
		{
			return networkString.Equals(ref this);
		}
		return false;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(NetworkString<Size> other)
	{
		return Compare(ref other) == 0;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(ref NetworkString<Size> other)
	{
		return Compare(ref other) == 0;
	}

	public bool Equals<OtherSize>(NetworkString<OtherSize> other) where OtherSize : unmanaged, IFixedStorage
	{
		return Compare(ref other) == 0;
	}

	public bool Equals<OtherSize>(ref NetworkString<OtherSize> other) where OtherSize : unmanaged, IFixedStorage
	{
		return Compare(ref other) == 0;
	}

	public unsafe bool StartsWith(string s)
	{
		if (s == null)
		{
			throw new ArgumentNullException("s");
		}
		fixed (Size* data = &_data)
		{
			return UTF32Tools.StartsWithOrdinal((uint*)data, SafeLength, s);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe bool StartsWith<OtherSize>(ref NetworkString<OtherSize> other) where OtherSize : unmanaged, IFixedStorage
	{
		fixed (OtherSize* data = &other._data)
		{
			fixed (Size* data2 = &_data)
			{
				return UTF32Tools.StartsWithOrdinal((uint*)data2, SafeLength, (uint*)data, other.SafeLength);
			}
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe bool EndsWith<OtherSize>(ref NetworkString<OtherSize> other) where OtherSize : unmanaged, IFixedStorage
	{
		fixed (OtherSize* data = &other._data)
		{
			fixed (Size* data2 = &_data)
			{
				return UTF32Tools.EndsWithOrdinal((uint*)data2, SafeLength, (uint*)data, other.SafeLength);
			}
		}
	}

	public unsafe bool EndsWith(string s)
	{
		if (s == null)
		{
			throw new ArgumentNullException("s");
		}
		fixed (Size* data = &_data)
		{
			return UTF32Tools.EndsWithOrdinal((uint*)data, SafeLength, s);
		}
	}

	public unsafe override int GetHashCode()
	{
		fixed (Size* data = &_data)
		{
			return UTF32Tools.GetHashDeterministic((uint*)data, SafeLength);
		}
	}

	public override string ToString()
	{
		return Value;
	}

	public unsafe UTF32Tools.CharEnumerator GetEnumerator()
	{
		fixed (Size* data = &_data)
		{
			return new UTF32Tools.CharEnumerator((uint*)data, Length);
		}
	}

	IEnumerator<char> IEnumerable<char>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	private int SafeIndex(int index)
	{
		int safeLength = SafeLength;
		if (index < 0 || index >= safeLength)
		{
			throw new ArgumentOutOfRangeException("index");
		}
		return index;
	}
}
public static class NetworkString
{
	public unsafe static int GetCapacity<Size>() where Size : unmanaged, IFixedStorage
	{
		return sizeof(Size) / 4;
	}
}
