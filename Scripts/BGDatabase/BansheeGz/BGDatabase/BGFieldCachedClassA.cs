using System;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldCachedClassA<T> : BGFieldCachedA<T> where T : class
{
	public override T this[int entityIndex]
	{
		set
		{
			T oldValue = this[entityIndex];
			BGEntity entity = base.Meta[entityIndex];
			FireBeforeValueChanged(entity, oldValue, value);
			StoreSet(entityIndex, value);
			FireValueChanged(entity, oldValue, value);
		}
	}

	protected BGFieldCachedClassA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGFieldCachedClassA(BGMetaEntity meta, BGId id, string name)
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
		if (num == -1)
		{
			return;
		}
		BGFieldCachedClassA<T> bGFieldCachedClassA = (BGFieldCachedClassA<T>)fromField;
		T val = bGFieldCachedClassA[fromEntityIndex];
		if (val != null)
		{
			if (val is ICloneable cloneable)
			{
				StoreSet(num, (T)cloneable.Clone());
			}
			else
			{
				StoreSet(num, BGUtil.Clone(val));
			}
		}
		else
		{
			ClearValueNoEvent(num);
		}
	}
}
