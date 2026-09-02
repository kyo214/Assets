using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldCachedListA<T> : BGFieldCachedClassA<List<T>>, BGListI
{
	protected virtual char[] StringValueSeparator => BGField<List<T>>.AA;

	protected BGFieldCachedListA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGFieldCachedListA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	public int CountValues(int entityIndex)
	{
		return this[entityIndex]?.Count ?? 0;
	}

	public override bool AreStoredValuesEqual(BGField field, int myEntityIndex, int otherEntityIndex)
	{
		if (!(field is BGFieldCachedListA<T> bGFieldCachedListA))
		{
			return false;
		}
		List<T> list = this[myEntityIndex];
		List<T> list2 = bGFieldCachedListA[otherEntityIndex];
		bool flag = BGUtil.IsEmpty(list);
		bool flag2 = BGUtil.IsEmpty(list2);
		if (flag & flag2)
		{
			return true;
		}
		if (flag | flag2)
		{
			return false;
		}
		if (list.Count != list2.Count)
		{
			return false;
		}
		for (int i = 0; i < list.Count; i++)
		{
			T myValue = list[i];
			T myValue2 = list2[i];
			if (!AreEqual(myValue, myValue2))
			{
				return false;
			}
		}
		return true;
	}

	protected abstract bool AreEqual(T myValue, T myValue2);

	public static List<T> EnsureValue(BGFieldCachedListA<T> field, int entityIndex)
	{
		List<T> list = field[entityIndex];
		if (list != null)
		{
			return list;
		}
		return field[entityIndex] = new List<T>();
	}

	public static List<T> EnsureValueCleared(BGFieldCachedListA<T> field, int entityIndex, int capacity = 0)
	{
		List<T> list = EnsureValue(field, entityIndex);
		list.Clear();
		if (capacity != 0)
		{
			list.Capacity = capacity;
		}
		return list;
	}
}
