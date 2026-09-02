using System;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldCachedStructArrayA<T> : BGFieldCachedArrayA<T> where T : struct
{
	public override T[] this[int entityIndex]
	{
		set
		{
			if (base.events.On)
			{
				T[] array = this[entityIndex];
				if (!BGUtil.ArraysValuesEqual(value, array))
				{
					BGEntity entity = base.Meta[entityIndex];
					FireBeforeValueChanged(entity, array, value);
					StoreSet(entityIndex, value);
					FireValueChanged(entity, array, value);
				}
			}
			else
			{
				StoreSet(entityIndex, value);
			}
		}
	}

	public BGFieldCachedStructArrayA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGFieldCachedStructArrayA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	public override void CopyValue(BGField fromField, BGId fromEntityId, int fromEntityIndex, BGId toEntityId)
	{
		if (fromEntityIndex == -1 || fromField.IsDeleted)
		{
			return;
		}
		int num = base.Meta.FindEntityIndex(toEntityId);
		if (num != -1)
		{
			BGFieldCachedStructArrayA<T> bGFieldCachedStructArrayA = (BGFieldCachedStructArrayA<T>)fromField;
			T[] array = bGFieldCachedStructArrayA[fromEntityIndex];
			if (array == null || array.Length == 0)
			{
				ClearValueNoEvent(num);
				return;
			}
			T[] array2 = new T[array.Length];
			Array.Copy(array, array2, array.Length);
			StoreSet(num, array2);
		}
	}
}
