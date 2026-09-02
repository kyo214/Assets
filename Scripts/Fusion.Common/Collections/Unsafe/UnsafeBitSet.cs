#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;

namespace Collections.Unsafe;

public struct UnsafeBitSet
{
	public unsafe struct Iterator(UnsafeBitSet* set) : IUnsafeIterator<(int bit, bool set)>, IEnumerator<(int bit, bool set)>, IEnumerator, IDisposable, IEnumerable<(int bit, bool set)>, IEnumerable
	{
		private unsafe UnsafeBitSet* _set = set;

		private int _current = -1;

		public unsafe (int bit, bool set) Current
		{
			get
			{
				if ((uint)_current >= (uint)_set->_sizeBits)
				{
					throw new InvalidOperationException();
				}
				return (bit: _current, set: IsSet(_set, _current));
			}
		}

		object IEnumerator.Current => Current;

		public unsafe bool MoveNext()
		{
			return ++_current < _set->_sizeBits;
		}

		public void Reset()
		{
			_current = -1;
		}

		public void Dispose()
		{
		}

		public IEnumerator<(int, bool)> GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	private const int WORD_SIZE = 8;

	private const int WORD_SIZE_BITS = 64;

	private const ulong WORD_ONE = 1uL;

	private const ulong WORD_ZERO = 0uL;

	private const string SET_DIFFERENT_SIZE = "Sets have different size";

	private const string SET_SIZE_LESS_THAN_ONE = "Set size can't be less than 1";

	private const string SET_ARRAY_LESS_CAPACITY = "Array is not long enough to hold all bits";

	private unsafe ulong* _bits;

	private int _sizeBits;

	private int _sizeBuckets;

	public unsafe static UnsafeBitSet* Alloc(int size)
	{
		if (size < 1)
		{
			throw new InvalidOperationException("Set size can't be less than 1");
		}
		size = Native.RoundToAlignment(size, 64);
		int num = Native.RoundToAlignment(sizeof(UnsafeBitSet), 8);
		int num2 = size / 8;
		void* ptr = Native.MallocAndClear(num + num2);
		UnsafeBitSet* ptr2 = (UnsafeBitSet*)ptr;
		ptr2->_sizeBits = size;
		ptr2->_sizeBuckets = size / 64;
		ptr2->_bits = (ulong*)((byte*)ptr + num);
		return ptr2;
	}

	public unsafe static void Free(UnsafeBitSet* set)
	{
		*set = default;
		Native.Free(set);
	}

	public unsafe static int Size(UnsafeBitSet* set)
	{
		return set->_sizeBits;
	}

	public unsafe static void Clear(UnsafeBitSet* set)
	{
		Native.MemClear(set->_bits, set->_sizeBuckets * 8);
	}

	public unsafe static void Set(UnsafeBitSet* set, int bit)
	{
		if ((uint)bit >= (uint)set->_sizeBits)
		{
			throw new IndexOutOfRangeException();
		}
		set->_bits[bit / 64] |= (ulong)(1L << bit % 64);
	}

	public unsafe static void Clear(UnsafeBitSet* set, int bit)
	{
		if ((uint)bit >= (uint)set->_sizeBits)
		{
			throw new IndexOutOfRangeException();
		}
		set->_bits[bit / 64] &= (ulong)(~(1L << bit % 64));
	}

	public unsafe static bool IsSet(UnsafeBitSet* set, int bit)
	{
		if ((uint)bit >= (uint)set->_sizeBits)
		{
			throw new IndexOutOfRangeException();
		}
		return (set->_bits[bit / 64] & (ulong)(1L << bit % 64)) != 0;
	}

	public unsafe static void Or(UnsafeBitSet* set, UnsafeBitSet* other)
	{
		if (set->_sizeBits != other->_sizeBits)
		{
			throw new InvalidOperationException("Sets have different size");
		}
		for (int num = set->_sizeBuckets - 1; num >= 0; num--)
		{
			set->_bits[num] |= other->_bits[num];
		}
	}

	public unsafe static void And(UnsafeBitSet* set, UnsafeBitSet* other)
	{
		if (set->_sizeBits != other->_sizeBits)
		{
			throw new InvalidOperationException("Sets have different size");
		}
		for (int num = set->_sizeBuckets - 1; num >= 0; num--)
		{
			set->_bits[num] &= other->_bits[num];
		}
	}

	public unsafe static void Xor(UnsafeBitSet* set, UnsafeBitSet* other)
	{
		if (set->_sizeBits != other->_sizeBits)
		{
			throw new InvalidOperationException("Sets have different size");
		}
		for (int num = set->_sizeBuckets - 1; num >= 0; num--)
		{
			set->_bits[num] ^= other->_bits[num];
		}
	}

	public unsafe static bool AnySet(UnsafeBitSet* set)
	{
		for (int num = set->_sizeBuckets - 1; num >= 0; num--)
		{
			if (set->_bits[num] != 0)
			{
				return true;
			}
		}
		return false;
	}

	public unsafe static Iterator GetIterator(UnsafeBitSet* set)
	{
		return new Iterator(set);
	}

	public unsafe static int GetSetBits(UnsafeBitSet* set, UnsafeArray* array)
	{
		Assert.Check(UnsafeArray.GetStride(array) == 4);
		if (UnsafeArray.Length(array) < set->_sizeBits)
		{
			throw new InvalidOperationException("Array is not long enough to hold all bits");
		}
		int result = 0;
		int num = 0;
		int* buffer = (int*)UnsafeArray.GetBuffer(array);
		for (int i = 0; i < set->_sizeBuckets; i++)
		{
			ulong num2 = set->_bits[i];
			if (num2 == 0)
			{
				num += 64;
				continue;
			}
			int num3 = 0;
			while (true)
			{
				uint num4 = ((uint*)(&num2))[num3];
				if (num4 != 0)
				{
					int num5 = 0;
					while (true)
					{
						ushort num6 = ((ushort*)(&num4))[num5];
						if (num6 != 0)
						{
							int num7 = 0;
							while (true)
							{
								byte b = ((byte*)(&num6))[num7];
								if (b != 0)
								{
									if ((b & 1) == 1)
									{
										buffer[result++] = num;
									}
									if ((b & 2) == 2)
									{
										buffer[result++] = num + 1;
									}
									if ((b & 4) == 4)
									{
										buffer[result++] = num + 2;
									}
									if ((b & 8) == 8)
									{
										buffer[result++] = num + 3;
									}
									if ((b & 0x10) == 16)
									{
										buffer[result++] = num + 4;
									}
									if ((b & 0x20) == 32)
									{
										buffer[result++] = num + 5;
									}
									if ((b & 0x40) == 64)
									{
										buffer[result++] = num + 6;
									}
									if ((b & 0x80) == 128)
									{
										buffer[result++] = num + 7;
									}
								}
								num += 8;
								if (num7 == 0)
								{
									num7++;
									continue;
								}
								break;
							}
						}
						else
						{
							num += 16;
						}
						if (num5 == 0)
						{
							num5++;
							continue;
						}
						break;
					}
				}
				else
				{
					num += 32;
				}
				if (num3 != 0)
				{
					break;
				}
				num3++;
			}
		}
		return result;
	}
}
