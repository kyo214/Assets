namespace BansheeGz.BGDatabase;

public abstract class BGFieldCachedArrayA<T> : BGFieldCachedClassA<T[]>, BGArrayI
{
	protected BGFieldCachedArrayA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGFieldCachedArrayA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	public int CountValues(int entityIndex)
	{
		T[] array = this[entityIndex];
		if (array == null)
		{
			return 0;
		}
		return array.Length;
	}

	public override bool AreStoredValuesEqual(BGField field, int myEntityIndex, int otherEntityIndex)
	{
		if (!(field is BGFieldCachedArrayA<T> bGFieldCachedArrayA))
		{
			return false;
		}
		T[] array = this[myEntityIndex];
		T[] array2 = bGFieldCachedArrayA[otherEntityIndex];
		bool flag = BGUtil.IsEmpty(array);
		bool flag2 = BGUtil.IsEmpty(array2);
		if (flag & flag2)
		{
			return true;
		}
		if (flag | flag2)
		{
			return false;
		}
		if (array.Length != array2.Length)
		{
			return false;
		}
		for (int i = 0; i < array.Length; i++)
		{
			T myValue = array[i];
			T myValue2 = array2[i];
			if (!AreEqual(myValue, myValue2))
			{
				return false;
			}
		}
		return true;
	}

	protected abstract bool AreEqual(T myValue, T myValue2);
}
