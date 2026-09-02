using System;

namespace Unity.Collections.LowLevel.Unsafe;

public static class HashSetExtensions
{
	public static void ExceptWith<T>(this NativeHashSet<T> container, UnsafeHashSet<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Remove(item);
		}
	}

	public static void IntersectWith<T>(this NativeHashSet<T> container, UnsafeHashSet<T> other) where T : unmanaged, IEquatable<T>
	{
		UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp);
		foreach (T item in other)
		{
			T value = item;
			if (container.Contains(value))
			{
				other2.Add(in value);
			}
		}
		container.Clear();
		container.UnionWith(other2);
		other2.Dispose();
	}

	public static void UnionWith<T>(this NativeHashSet<T> container, UnsafeHashSet<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Add(item);
		}
	}

	public static void ExceptWith<T>(this NativeHashSet<T> container, UnsafeList<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Remove(item);
		}
	}

	public static void IntersectWith<T>(this NativeHashSet<T> container, UnsafeList<T> other) where T : unmanaged, IEquatable<T>
	{
		UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp);
		foreach (T item in other)
		{
			T value = item;
			if (container.Contains(value))
			{
				other2.Add(in value);
			}
		}
		container.Clear();
		container.UnionWith(other2);
		other2.Dispose();
	}

	public static void UnionWith<T>(this NativeHashSet<T> container, UnsafeList<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Add(item);
		}
	}

	public static void ExceptWith<T>(this UnsafeHashSet<T> container, FixedList128Bytes<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Remove(item);
		}
	}

	public static void IntersectWith<T>(this UnsafeHashSet<T> container, FixedList128Bytes<T> other) where T : unmanaged, IEquatable<T>
	{
		UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp);
		foreach (T item in other)
		{
			T value = item;
			if (container.Contains(value))
			{
				other2.Add(in value);
			}
		}
		container.Clear();
		container.UnionWith(other2);
		other2.Dispose();
	}

	public static void UnionWith<T>(this UnsafeHashSet<T> container, FixedList128Bytes<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Add(item);
		}
	}

	public static void ExceptWith<T>(this UnsafeHashSet<T> container, FixedList32Bytes<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Remove(item);
		}
	}

	public static void IntersectWith<T>(this UnsafeHashSet<T> container, FixedList32Bytes<T> other) where T : unmanaged, IEquatable<T>
	{
		UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp);
		foreach (T item in other)
		{
			T value = item;
			if (container.Contains(value))
			{
				other2.Add(in value);
			}
		}
		container.Clear();
		container.UnionWith(other2);
		other2.Dispose();
	}

	public static void UnionWith<T>(this UnsafeHashSet<T> container, FixedList32Bytes<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Add(item);
		}
	}

	public static void ExceptWith<T>(this UnsafeHashSet<T> container, FixedList4096Bytes<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Remove(item);
		}
	}

	public static void IntersectWith<T>(this UnsafeHashSet<T> container, FixedList4096Bytes<T> other) where T : unmanaged, IEquatable<T>
	{
		UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp);
		foreach (T item in other)
		{
			T value = item;
			if (container.Contains(value))
			{
				other2.Add(in value);
			}
		}
		container.Clear();
		container.UnionWith(other2);
		other2.Dispose();
	}

	public static void UnionWith<T>(this UnsafeHashSet<T> container, FixedList4096Bytes<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Add(item);
		}
	}

	public static void ExceptWith<T>(this UnsafeHashSet<T> container, FixedList512Bytes<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Remove(item);
		}
	}

	public static void IntersectWith<T>(this UnsafeHashSet<T> container, FixedList512Bytes<T> other) where T : unmanaged, IEquatable<T>
	{
		UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp);
		foreach (T item in other)
		{
			T value = item;
			if (container.Contains(value))
			{
				other2.Add(in value);
			}
		}
		container.Clear();
		container.UnionWith(other2);
		other2.Dispose();
	}

	public static void UnionWith<T>(this UnsafeHashSet<T> container, FixedList512Bytes<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Add(item);
		}
	}

	public static void ExceptWith<T>(this UnsafeHashSet<T> container, FixedList64Bytes<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Remove(item);
		}
	}

	public static void IntersectWith<T>(this UnsafeHashSet<T> container, FixedList64Bytes<T> other) where T : unmanaged, IEquatable<T>
	{
		UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp);
		foreach (T item in other)
		{
			T value = item;
			if (container.Contains(value))
			{
				other2.Add(in value);
			}
		}
		container.Clear();
		container.UnionWith(other2);
		other2.Dispose();
	}

	public static void UnionWith<T>(this UnsafeHashSet<T> container, FixedList64Bytes<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Add(item);
		}
	}

	public static void ExceptWith<T>(this UnsafeHashSet<T> container, NativeArray<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Remove(item);
		}
	}

	public static void IntersectWith<T>(this UnsafeHashSet<T> container, NativeArray<T> other) where T : unmanaged, IEquatable<T>
	{
		UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp);
		foreach (T item in other)
		{
			T value = item;
			if (container.Contains(value))
			{
				other2.Add(in value);
			}
		}
		container.Clear();
		container.UnionWith(other2);
		other2.Dispose();
	}

	public static void UnionWith<T>(this UnsafeHashSet<T> container, NativeArray<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Add(item);
		}
	}

	public static void ExceptWith<T>(this UnsafeHashSet<T> container, NativeHashSet<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Remove(item);
		}
	}

	public static void IntersectWith<T>(this UnsafeHashSet<T> container, NativeHashSet<T> other) where T : unmanaged, IEquatable<T>
	{
		UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp);
		foreach (T item in other)
		{
			T value = item;
			if (container.Contains(value))
			{
				other2.Add(in value);
			}
		}
		container.Clear();
		container.UnionWith(other2);
		other2.Dispose();
	}

	public static void UnionWith<T>(this UnsafeHashSet<T> container, NativeHashSet<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Add(item);
		}
	}

	public static void ExceptWith<T>(this UnsafeHashSet<T> container, NativeList<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Remove(item);
		}
	}

	public static void IntersectWith<T>(this UnsafeHashSet<T> container, NativeList<T> other) where T : unmanaged, IEquatable<T>
	{
		UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp);
		foreach (T item in other)
		{
			T value = item;
			if (container.Contains(value))
			{
				other2.Add(in value);
			}
		}
		container.Clear();
		container.UnionWith(other2);
		other2.Dispose();
	}

	public static void UnionWith<T>(this UnsafeHashSet<T> container, NativeList<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Add(item);
		}
	}

	public static void ExceptWith<T>(this UnsafeHashSet<T> container, UnsafeHashSet<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Remove(item);
		}
	}

	public static void IntersectWith<T>(this UnsafeHashSet<T> container, UnsafeHashSet<T> other) where T : unmanaged, IEquatable<T>
	{
		UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp);
		foreach (T item in other)
		{
			T value = item;
			if (container.Contains(value))
			{
				other2.Add(in value);
			}
		}
		container.Clear();
		container.UnionWith(other2);
		other2.Dispose();
	}

	public static void UnionWith<T>(this UnsafeHashSet<T> container, UnsafeHashSet<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Add(item);
		}
	}

	public static void ExceptWith<T>(this UnsafeHashSet<T> container, UnsafeList<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Remove(item);
		}
	}

	public static void IntersectWith<T>(this UnsafeHashSet<T> container, UnsafeList<T> other) where T : unmanaged, IEquatable<T>
	{
		UnsafeList<T> other2 = new UnsafeList<T>(container.Count(), Allocator.Temp);
		foreach (T item in other)
		{
			T value = item;
			if (container.Contains(value))
			{
				other2.Add(in value);
			}
		}
		container.Clear();
		container.UnionWith(other2);
		other2.Dispose();
	}

	public static void UnionWith<T>(this UnsafeHashSet<T> container, UnsafeList<T> other) where T : unmanaged, IEquatable<T>
	{
		foreach (T item in other)
		{
			container.Add(item);
		}
	}
}
