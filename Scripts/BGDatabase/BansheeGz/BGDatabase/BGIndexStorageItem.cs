using System;

namespace BansheeGz.BGDatabase;

public class BGIndexStorageItem<T> : IComparable<BGIndexStorageItem<T>> where T : IComparable<T>
{
	public static readonly BGObjectPool<BGIndexStorageItem<T>> Pool = new BGObjectPool<BGIndexStorageItem<T>>(() => new BGIndexStorageItem<T>(default, null), (BGIndexStorageItem<T> item) =>
	{
		item.key = default;
		item.entity = null;
	});

	public static readonly BGIndexStorageItem<T> Eternity = new BGIndexStorageItem<T>(default, null);

	public static readonly BGIndexStorageItem<T> EternityMinus = new BGIndexStorageItem<T>(default, null);

	private static readonly bool IsString = typeof(T) == typeof(string);

	public T key;

	public BGEntity entity;

	public BGIndexStorageItem(T key, BGEntity entity)
	{
		this.key = key;
		this.entity = entity;
	}

	public int CompareTo(BGIndexStorageItem<T> other)
	{
		if (this == Eternity)
		{
			return 1;
		}
		if (other == Eternity)
		{
			return -1;
		}
		if (this == EternityMinus)
		{
			return -1;
		}
		if (other == EternityMinus)
		{
			return 1;
		}
		if (IsString)
		{
			bool flag = key == null;
			bool flag2 = other.key == null;
			if (flag & flag2)
			{
				if (entity == null || other.entity == null)
				{
					return 0;
				}
				return entity.Index.CompareTo(other.entity.Index);
			}
			if (flag)
			{
				return -1;
			}
			if (flag2)
			{
				return 1;
			}
			int num = string.Compare((string)(object)key, (string)(object)other.key, BGIndex.DefaultStringComparison);
			if (num != 0)
			{
				return num;
			}
			if (entity == null || other.entity == null)
			{
				return 0;
			}
			return entity.Index.CompareTo(other.entity.Index);
		}
		ref T reference = ref key;
		T other2 = other.key;
		int num2 = reference.CompareTo(other2);
		if (num2 != 0)
		{
			return num2;
		}
		if (entity == null || other.entity == null)
		{
			return 0;
		}
		return entity.Index.CompareTo(other.entity.Index);
	}
}
