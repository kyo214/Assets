using System;

namespace BansheeGz.BGDatabase;

internal class BGKeyStorageKeyN : BGKeyStorageKeyI, IEquatable<BGKeyStorageKeyI>
{
	public static readonly BGObjectPool<BGKeyStorageKeyN> Pool = new BGObjectPool<BGKeyStorageKeyN>(() => new BGKeyStorageKeyN(Array.Empty<object>()), (BGKeyStorageKeyN k) =>
	{
		k.Values = Array.Empty<object>();
	});

	internal object[] Values;

	public BGKeyStorageKeyN(object[] values)
	{
		Values = values;
	}

	public bool IsValueEquals(object value, int index)
	{
		return object.Equals(Values[index], value);
	}

	public BGKeyStorageKeyI Clone()
	{
		object[] array = new object[Values.Length];
		for (int i = 0; i < Values.Length; i++)
		{
			array[i] = Values[i];
		}
		return new BGKeyStorageKeyN(array);
	}

	public bool Equals(BGKeyStorageKeyI key)
	{
		if (key == null)
		{
			return false;
		}
		for (int i = 0; i < Values.Length; i++)
		{
			object value = Values[i];
			if (!key.IsValueEquals(value, i))
			{
				return false;
			}
		}
		return true;
	}

	public override int GetHashCode()
	{
		int num = Values[0]?.GetHashCode() ?? 0;
		for (int i = 1; i < Values.Length; i++)
		{
			num = (num * 397) ^ (Values[i]?.GetHashCode() ?? 0);
		}
		return num;
	}
}
