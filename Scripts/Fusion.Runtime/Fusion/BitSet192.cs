#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fusion;

[Serializable]
[StructLayout(LayoutKind.Explicit)]
[NetworkStructWeaved(6)]
public struct BitSet192 : INetworkStruct, IEquatable<BitSet192>, IEnumerable<int>, IEnumerable
{
	public unsafe struct Enumerator(ulong* bits) : IEnumerator<int>, IEnumerator, IDisposable
	{
		private unsafe ulong* _bits = bits;

		private int _bit = -1;

		public int Current => _bit;

		object IEnumerator.Current => Current;

		public void Reset()
		{
			_bit = -1;
		}

		public unsafe bool MoveNext()
		{
			while (++_bit < 192)
			{
				if ((_bits[_bit / 64] & (ulong)(1L << _bit % 64)) != 0)
				{
					return true;
				}
			}
			return false;
		}

		public unsafe void Dispose()
		{
			_bits = null;
			_bit = -1;
		}
	}

	public const int SIZE = 24;

	[FieldOffset(0)]
	public unsafe fixed ulong Bits[3];

	public int Length => 192;

	public unsafe static BitSet192 FromArray(ulong[] values)
	{
		if (values == null)
		{
			throw new ArgumentNullException("values");
		}
		if (3 != values.Length)
		{
			throw new ArgumentException("Array needs to be of length 3", "values");
		}
		BitSet192 result = default;
		for (int i = 0; i < 3; i++)
		{
			result.Bits[i] = values[i];
		}
		return result;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void Set(int bit)
	{
		Assert.Check(bit >= 0 && bit < 192);
		fixed (ulong* bits = Bits)
		{
			bits[bit / 64] |= (ulong)(1L << bit % 64);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void Clear(int bit)
	{
		Assert.Check(bit >= 0 && bit < 192);
		fixed (ulong* bits = Bits)
		{
			bits[bit / 64] &= (ulong)(~(1L << bit % 64));
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe void ClearAll()
	{
		fixed (ulong* bits = Bits)
		{
			Native.ArrayClear(bits, 3);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe bool IsSet(int bit)
	{
		fixed (ulong* bits = Bits)
		{
			return (bits[bit / 64] & (ulong)(1L << bit % 64)) != 0;
		}
	}

	public unsafe int GetSetCount()
	{
		int num = 0;
		fixed (ulong* bits = Bits)
		{
			num += Maths.CountSetBits(*bits);
			num += Maths.CountSetBits(bits[1]);
			return num + Maths.CountSetBits(bits[2]);
		}
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public unsafe bool Any()
	{
		int num = 0;
		fixed (ulong* bits = Bits)
		{
			num += ((*bits != 0L) ? 1 : 0);
			num += ((bits[1] != 0L) ? 1 : 0);
			num += ((bits[2] != 0L) ? 1 : 0);
		}
		return num > 0;
	}

	public unsafe override int GetHashCode()
	{
		fixed (ulong* bits = Bits)
		{
			return HashCodeUtilities.GetArrayHashCode(bits, 3, 43);
		}
	}

	public override bool Equals(object obj)
	{
		return obj is BitSet192 && Equals((BitSet192)obj);
	}

	public unsafe bool Equals(BitSet192 other)
	{
		fixed (ulong* bits = Bits)
		{
			return Native.ArrayCompare(bits, other.Bits, 3) == 0;
		}
	}

	public unsafe Enumerator GetEnumerator()
	{
		fixed (ulong* bits = Bits)
		{
			return new Enumerator(bits);
		}
	}

	IEnumerator<int> IEnumerable<int>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
