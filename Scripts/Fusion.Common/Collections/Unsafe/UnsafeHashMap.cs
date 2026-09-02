#define DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Fusion;

namespace Collections.Unsafe;

public struct UnsafeHashMap
{
	public unsafe struct Iterator<K, V>(UnsafeHashMap* map) : IUnsafeIterator<(K key, V value)>, IEnumerator<(K key, V value)>, IEnumerator, IDisposable, IEnumerable<(K key, V value)>, IEnumerable where K : unmanaged where V : unmanaged
	{
		private unsafe UnsafeHashCollection.Iterator _iterator = new UnsafeHashCollection.Iterator(&map->_collection);

		private unsafe int _keyOffset = map->_collection.KeyOffset;

		private unsafe int _valueOffset = map->_valueOffset;

		public unsafe K CurrentKey
		{
			get
			{
				if (_iterator.Current == null)
				{
					throw new InvalidOperationException();
				}
				return *(K*)((byte*)_iterator.Current + _keyOffset);
			}
		}

		public unsafe V CurrentValue
		{
			get
			{
				if (_iterator.Current == null)
				{
					throw new InvalidOperationException();
				}
				return *(V*)((byte*)_iterator.Current + _valueOffset);
			}
		}

		public (K key, V value) Current => (key: CurrentKey, value: CurrentValue);

		object IEnumerator.Current => Current;

		public bool MoveNext()
		{
			return _iterator.Next();
		}

		public void Reset()
		{
			_iterator.Reset();
		}

		public void Dispose()
		{
		}

		public IEnumerator<(K key, V value)> GetEnumerator()
		{
			return this;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}

	private UnsafeHashCollection _collection;

	private int _valueOffset;

	public unsafe static int Capacity(UnsafeHashMap* map)
	{
		return map->_collection.Entries.Length;
	}

	public unsafe static int Count(UnsafeHashMap* map)
	{
		return map->_collection.UsedCount - map->_collection.FreeCount;
	}

	public unsafe static void Clear(UnsafeHashMap* set)
	{
		UnsafeHashCollection.Clear(&set->_collection);
	}

	public unsafe static UnsafeHashMap* Allocate<K, V>(int capacity, bool fixedSize = false) where K : unmanaged, IEquatable<K> where V : unmanaged
	{
		return Allocate(capacity, sizeof(K), sizeof(V), fixedSize);
	}

	public unsafe static UnsafeHashMap* Allocate(int capacity, int keyStride, int valStride, bool fixedSize = false)
	{
		int num = sizeof(UnsafeHashCollection.Entry);
		capacity = UnsafeHashCollection.GetNextPrime(capacity);
		Assert.Check(num == 16);
		int alignment = Native.GetAlignment(keyStride);
		int alignment2 = Native.GetAlignment(valStride);
		int alignment3 = Math.Max(8, Math.Max(alignment, alignment2));
		keyStride = Native.RoundToAlignment(keyStride, alignment3);
		valStride = Native.RoundToAlignment(valStride, alignment3);
		num = Native.RoundToAlignment(sizeof(UnsafeHashCollection.Entry), alignment3);
		UnsafeHashMap* ptr2;
		if (fixedSize)
		{
			int num2 = Native.RoundToAlignment(sizeof(UnsafeHashMap), alignment3);
			int num3 = Native.RoundToAlignment(sizeof(UnsafeHashCollection.Entry**) * capacity, alignment3);
			int num4 = (num + keyStride + valStride) * capacity;
			void* ptr = Native.MallocAndClear(num2 + num3 + num4);
			ptr2 = (UnsafeHashMap*)ptr;
			ptr2->_collection.Buckets = (UnsafeHashCollection.Entry**)((byte*)ptr + num2);
			UnsafeBuffer.InitFixed(&ptr2->_collection.Entries, (byte*)ptr + (num2 + num3), capacity, num + keyStride + valStride);
		}
		else
		{
			ptr2 = Native.MallocAndClear<UnsafeHashMap>();
			ptr2->_collection.Buckets = (UnsafeHashCollection.Entry**)Native.MallocAndClear(sizeof(UnsafeHashCollection.Entry**) * capacity);
			UnsafeBuffer.InitDynamic(&ptr2->_collection.Entries, capacity, num + keyStride + valStride);
		}
		ptr2->_collection.FreeCount = 0;
		ptr2->_collection.UsedCount = 0;
		ptr2->_collection.KeyOffset = num;
		ptr2->_valueOffset = num + keyStride;
		return ptr2;
	}

	public unsafe static void Free(UnsafeHashMap* set)
	{
		if (set->_collection.Entries.Dynamic == 1)
		{
			UnsafeHashCollection.Free(&set->_collection);
		}
		Native.Free(set);
	}

	public unsafe static Iterator<K, V> GetIterator<K, V>(UnsafeHashMap* map) where K : unmanaged where V : unmanaged
	{
		return new Iterator<K, V>(map);
	}

	public unsafe static bool ContainsKey<K>(UnsafeHashMap* map, K key) where K : unmanaged, IEquatable<K>
	{
		return UnsafeHashCollection.Find(&map->_collection, key, key.GetHashCode()) != null;
	}

	public unsafe static void AddOrGet<K, V>(UnsafeHashMap* map, K key, ref V value) where K : unmanaged, IEquatable<K> where V : unmanaged
	{
		int hashCode = key.GetHashCode();
		UnsafeHashCollection.Entry* ptr = UnsafeHashCollection.Find(&map->_collection, key, hashCode);
		if (ptr == null)
		{
			ptr = UnsafeHashCollection.Insert(&map->_collection, key, hashCode);
			*(V*)GetValue(map, ptr) = value;
		}
		else
		{
			value = *(V*)GetValue(map, ptr);
		}
	}

	public unsafe static void Add<K, V>(UnsafeHashMap* map, K key, V value) where K : unmanaged, IEquatable<K> where V : unmanaged
	{
		int hashCode = key.GetHashCode();
		UnsafeHashCollection.Entry* ptr = UnsafeHashCollection.Find(&map->_collection, key, hashCode);
		if (ptr == null)
		{
			ptr = UnsafeHashCollection.Insert(&map->_collection, key, hashCode);
			*(V*)GetValue(map, ptr) = value;
			return;
		}
		throw new InvalidOperationException();
	}

	public unsafe static void Set<K, V>(UnsafeHashMap* map, K key, V value) where K : unmanaged, IEquatable<K> where V : unmanaged
	{
		int hashCode = key.GetHashCode();
		UnsafeHashCollection.Entry* ptr = UnsafeHashCollection.Find(&map->_collection, key, hashCode);
		if (ptr == null)
		{
			ptr = UnsafeHashCollection.Insert(&map->_collection, key, hashCode);
		}
		*(V*)GetValue(map, ptr) = value;
	}

	public unsafe static V Get<K, V>(UnsafeHashMap* map, K key) where K : unmanaged, IEquatable<K> where V : unmanaged
	{
		UnsafeHashCollection.Entry* ptr = UnsafeHashCollection.Find(&map->_collection, key, key.GetHashCode());
		if (ptr == null)
		{
			throw new KeyNotFoundException(key.ToString());
		}
		return *(V*)GetValue(map, ptr);
	}

	public unsafe static V* GetPtr<K, V>(UnsafeHashMap* map, K key) where K : unmanaged, IEquatable<K> where V : unmanaged
	{
		UnsafeHashCollection.Entry* ptr = UnsafeHashCollection.Find(&map->_collection, key, key.GetHashCode());
		if (ptr == null)
		{
			throw new KeyNotFoundException(key.ToString());
		}
		return (V*)GetValue(map, ptr);
	}

	public unsafe static V* GetOrAddPtr<K, V>(UnsafeHashMap* map, K key, V value) where K : unmanaged, IEquatable<K> where V : unmanaged
	{
		int hashCode = key.GetHashCode();
		UnsafeHashCollection.Entry* ptr = UnsafeHashCollection.Find(&map->_collection, key, hashCode);
		V* value2;
		if (ptr == null)
		{
			ptr = UnsafeHashCollection.Insert(&map->_collection, key, hashCode);
			value2 = (V*)GetValue(map, ptr);
			*value2 = value;
		}
		else
		{
			value2 = (V*)GetValue(map, ptr);
		}
		return value2;
	}

	public unsafe static bool TryGetValue<K, V>(UnsafeHashMap* map, K key, out V val) where K : unmanaged, IEquatable<K> where V : unmanaged
	{
		UnsafeHashCollection.Entry* ptr = UnsafeHashCollection.Find(&map->_collection, key, key.GetHashCode());
		if (ptr != null)
		{
			val = *(V*)GetValue(map, ptr);
			return true;
		}
		val = default;
		return false;
	}

	public unsafe static bool TryGetValuePtr<K, V>(UnsafeHashMap* map, K key, out V* val) where K : unmanaged, IEquatable<K> where V : unmanaged
	{
		UnsafeHashCollection.Entry* ptr = UnsafeHashCollection.Find(&map->_collection, key, key.GetHashCode());
		if (ptr != null)
		{
			val = (V*)GetValue(map, ptr);
			return true;
		}
		val = null;
		return false;
	}

	public unsafe static bool Remove<K>(UnsafeHashMap* map, K key) where K : unmanaged, IEquatable<K>
	{
		return UnsafeHashCollection.Remove(&map->_collection, key, key.GetHashCode());
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private unsafe static void* GetValue(UnsafeHashMap* map, UnsafeHashCollection.Entry* entry)
	{
		return (byte*)entry + map->_valueOffset;
	}
}
