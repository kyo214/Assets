using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldCachedClassListA<T> : BGFieldCachedListA<T> where T : class
{
	public override List<T> this[int entityIndex]
	{
		set
		{
			List<T> oldValue = this[entityIndex];
			BGEntity entity = base.Meta[entityIndex];
			FireBeforeValueChanged(entity, oldValue, value);
			StoreSet(entityIndex, value);
			FireValueChanged(entity, oldValue, value);
		}
	}

	protected BGFieldCachedClassListA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGFieldCachedClassListA(BGMetaEntity meta, BGId id, string name)
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
		BGFieldCachedClassListA<T> bGFieldCachedClassListA = (BGFieldCachedClassListA<T>)fromField;
		List<T> list = bGFieldCachedClassListA[fromEntityIndex];
		if (list == null)
		{
			ClearValueNoEvent(num);
			return;
		}
		List<T> list2 = new List<T>();
		if (typeof(ICloneable).IsAssignableFrom(typeof(T)))
		{
			foreach (T item in list)
			{
				list2.Add((T)((ICloneable)item).Clone());
			}
		}
		else
		{
			foreach (T item2 in list)
			{
				list2.Add(BGUtil.Clone(item2));
			}
		}
		StoreSet(num, list2);
	}
}
