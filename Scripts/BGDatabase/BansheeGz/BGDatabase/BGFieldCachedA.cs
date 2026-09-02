using System;
using System.Collections.Generic;
using UnityEngine;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldCachedA<T, TStoreType> : BGField<T>, BGStorageI<TStoreType>, BGStorable<TStoreType>
{
	protected TStoreType[] StoreItems = Array.Empty<TStoreType>();

	protected internal int StoreCount;

	protected BGStoreFieldI<TStoreType> Store => new BGStoreFieldAdapter<T, TStoreType>(this);

	public override T this[BGId entityId]
	{
		get
		{
			int num = base.Meta.FindEntityIndex(entityId);
			if (num == -1)
			{
				Debug.LogException(new BGException("Can not find entity with specified id=$, meta=$, field=$. Default value returned. ", entityId, base.MetaName, Name));
				return default;
			}
			return this[num];
		}
		set
		{
			int num = base.Meta.FindEntityIndex(entityId);
			if (num == -1)
			{
				Debug.LogException(new BGException("Can not find entity with specified id=$, meta=$, field=$. Setting value is skipped. ", entityId, base.MetaName, Name));
			}
			else
			{
				this[num] = value;
			}
		}
	}

	protected internal int StoreMinSize
	{
		set
		{
			if (StoreCount < value)
			{
				StoreMinCapacity = value;
				StoreCount = value;
			}
		}
	}

	protected internal int StoreMinCapacity
	{
		set
		{
			if (StoreItems.Length < value)
			{
				int num = ((StoreItems.Length == 0) ? 4 : (StoreItems.Length * 2));
				if (num < value)
				{
					num = value;
				}
				TStoreType[] array = new TStoreType[num];
				if (StoreCount > 0)
				{
					Array.Copy(StoreItems, 0, array, 0, StoreCount);
				}
				StoreItems = array;
			}
		}
	}

	protected BGFieldCachedA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGFieldCachedA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	public override void ForEachValue(Action<int> action)
	{
		StoreForEachKey(action);
	}

	public override void OnDelete()
	{
		ClearValues();
	}

	public override void ClearValues()
	{
		StoreClear();
	}

	public override void ClearValue(int entityIndex)
	{
		if (base.events.On)
		{
			if (entityIndex >= StoreCount)
			{
				ThrowIndexOutOfBoundOnWrite(entityIndex);
			}
			TStoreType val = StoreItems[entityIndex];
			bool flag = !EqualityComparer<TStoreType>.Default.Equals(val, default);
			StoreItems[entityIndex] = default;
			if (flag)
			{
				FireStoredValueChanged(base.Meta[entityIndex], val, default);
			}
		}
		else
		{
			ClearValueNoEvent(entityIndex);
		}
	}

	protected void ClearValueNoEvent(int entityIndex)
	{
		if (entityIndex >= StoreCount)
		{
			ThrowIndexOutOfBoundOnWrite(entityIndex);
		}
		StoreItems[entityIndex] = default;
	}

	[Obsolete("Use CloneTo(BGCloneContextField context) instead")]
	public override BGField CloneTo(BGMetaEntity meta, bool copyValues)
	{
		return CloneTo(new BGCloneContextField(meta, copyValues));
	}

	public override BGField CloneTo(BGCloneContextField context)
	{
		BGField clone = base.CloneTo(context);
		context.OnAfterFieldCreated?.Invoke(clone);
		if (context.copyValues)
		{
			base.Meta.ForEachEntity((BGEntity entity) =>
			{
				clone.CopyValue(this, entity.Id, entity.Index, entity.Id);
			});
		}
		return clone;
	}

	public TStoreType GetStoredValue(int index)
	{
		if (index >= StoreCount)
		{
			ThrowIndexOutOfBoundOnRead(index);
		}
		return StoreItems[index];
	}

	public virtual void SetStoredValue(int entityIndex, TStoreType value)
	{
		if (base.events.On)
		{
			TStoreType storedValue = GetStoredValue(entityIndex);
			if (!object.Equals(storedValue, value))
			{
				StoreItems[entityIndex] = value;
				FireStoredValueChanged(base.Meta[entityIndex], storedValue, value);
			}
		}
		else
		{
			if (entityIndex >= StoreCount)
			{
				ThrowIndexOutOfBoundOnWrite(entityIndex);
			}
			StoreItems[entityIndex] = value;
		}
	}

	public override void CopyValue(BGField fromField, BGId fromEntityId, int fromEntityIndex, BGId toEntityId)
	{
		if (fromEntityIndex != -1 && !fromField.IsDeleted)
		{
			int num = base.Meta.FindEntityIndex(toEntityId);
			if (num != -1)
			{
				StoreItems[num] = ((BGStorable<TStoreType>)fromField).GetStoredValue(fromEntityIndex);
			}
		}
	}

	public override void DuplicateValue(BGId fromEntityId, int fromEntityIndex, BGId toEntityId)
	{
		CopyValue(this, fromEntityId, fromEntityIndex, toEntityId);
	}

	public override void Swap(int entityIndex1, int entityIndex2)
	{
		StoreSwap(entityIndex1, entityIndex2);
	}

	public override void MoveEntitiesValues(int fromIndex, int toIndex, int numberOfValues)
	{
		if (fromIndex != toIndex)
		{
			int storeCount = StoreCount;
			if (numberOfValues <= 0)
			{
				throw new BGException("Invalid numberOfEntities: $. It should be more than 0", numberOfValues);
			}
			if (fromIndex < 0)
			{
				throw new BGException("Invalid fromIndex: $. It should be equal or more than 0", fromIndex);
			}
			if (fromIndex >= storeCount)
			{
				throw new BGException("Invalid fromIndex: $. It should be less than number of entities $", fromIndex, storeCount);
			}
			if (fromIndex + numberOfValues > storeCount)
			{
				throw new BGException("Invalid fromIndex: $. fromIndex + numberOfEntities should not exceed the number of entities $", fromIndex, storeCount);
			}
			if (toIndex < 0)
			{
				throw new BGException("Invalid toIndex: $. It should be equal or more than 0", toIndex);
			}
			if (toIndex >= storeCount)
			{
				throw new BGException("Invalid toIndex: $. It should be less than number of entities $", toIndex, storeCount);
			}
			if (toIndex + numberOfValues > storeCount)
			{
				throw new BGException("Invalid toIndex: $. toIndex + numberOfEntities should not exceed the number of entities $", toIndex, storeCount);
			}
			StoreMoveValues(fromIndex, toIndex, numberOfValues);
		}
	}

	public override bool AreStoredValuesEqual(BGField field, int myEntityIndex, int otherEntityIndex)
	{
		if (!(field is BGFieldCachedA<T, TStoreType> bGFieldCachedA))
		{
			return false;
		}
		if (myEntityIndex >= StoreCount)
		{
			ThrowIndexOutOfBoundOnRead(myEntityIndex);
		}
		if (otherEntityIndex >= bGFieldCachedA.StoreCount)
		{
			bGFieldCachedA.ThrowIndexOutOfBoundOnRead(otherEntityIndex);
		}
		TStoreType myValue = StoreItems[myEntityIndex];
		TStoreType otherValue = bGFieldCachedA.StoreItems[otherEntityIndex];
		return AreStoredValuesEqual(myValue, otherValue);
	}

	protected virtual bool AreStoredValuesEqual(TStoreType myValue, TStoreType otherValue)
	{
		return object.Equals(myValue, otherValue);
	}

	public override void OnEntityAdd(BGEntity entity)
	{
		StoreMinSize = base.Meta.CountEntities;
	}

	public override void OnEntityDelete(BGEntity entity)
	{
		StoreDeleteAt(entity.Index);
	}

	public override void OnCreate()
	{
		StoreMinSize = base.Meta.CountEntities;
	}

	public TStoreType[] CopyRawValues()
	{
		return StoreCopyRawValues();
	}

	public void FireStoredValueChanged(BGEntity entity, TStoreType oldValue, TStoreType newValue)
	{
		if (!base.events.ConsumeOnChange(base.MetaId))
		{
			using (BGEventArgsFieldWithValue<T, TStoreType> eventArgs = BGEventArgsFieldWithValue<T, TStoreType>.GetInstance(entity, this, oldValue, newValue))
			{
				FireValueChanged(eventArgs);
			}
			base.Meta.FireStoredValueChanged(this, entity, oldValue, newValue, nested: true);
			base.events.FireAnyChange();
		}
	}

	protected internal TStoreType StoreGet(int index)
	{
		if (index >= StoreCount)
		{
			ThrowIndexOutOfBoundOnRead(index);
		}
		return StoreItems[index];
	}

	protected internal void StoreSet(int index, TStoreType value)
	{
		if (index >= StoreCount)
		{
			ThrowIndexOutOfBoundOnWrite(index);
		}
		StoreItems[index] = value;
	}

	protected void ThrowIndexOutOfBoundOnRead(int index)
	{
		if (base.IsDeleted)
		{
			throw new Exception("An attempt to read value from field [" + Name + "], which was deleted or unloaded. Field can be unloaded when database is reloaded");
		}
		throw new Exception("Index is out of bounds while trying to read a value from field [" + base.FullName + "], it's greater or equal to maxIndex, " + index + ">=" + StoreCount);
	}

	protected void ThrowIndexOutOfBoundOnWrite(int index)
	{
		if (base.IsDeleted)
		{
			throw new Exception("An attempt to set value to field [" + Name + "], which was deleted or unloaded. Field can be unloaded when database is reloaded");
		}
		throw new Exception("Index is out of bounds while trying to set a value to field [" + base.FullName + "], it's greater or equal to maxIndex, " + index + ">=" + StoreCount);
	}

	protected internal void StoreDeleteAt(int index)
	{
		if (StoreCount > index)
		{
			StoreCount--;
			int num = StoreCount - index;
			if (num > 0)
			{
				Array.Copy(StoreItems, index + 1, StoreItems, index, num);
			}
			StoreItems[StoreCount] = default;
		}
	}

	protected internal void StoreClear()
	{
		StoreItems = Array.Empty<TStoreType>();
		StoreCount = 0;
	}

	protected internal void StoreAdd(TStoreType item)
	{
		StoreMinCapacity = StoreCount + 1;
		StoreItems[StoreCount] = item;
		StoreCount++;
	}

	protected internal void StoreSwap(int index1, int index2)
	{
		TStoreType[] storeItems = StoreItems;
		TStoreType[] storeItems2 = StoreItems;
		TStoreType val = StoreItems[index2];
		TStoreType val2 = StoreItems[index1];
		storeItems[index1] = val;
		storeItems2[index2] = val2;
	}

	protected internal void StoreMoveValues(int fromIndex, int toIndex, int numberOfElements)
	{
		TStoreType[] array = new TStoreType[numberOfElements];
		Array.Copy(StoreItems, fromIndex, array, 0, numberOfElements);
		if (fromIndex > toIndex)
		{
			if (toIndex + numberOfElements < fromIndex)
			{
				Array.Copy(StoreItems, toIndex, StoreItems, toIndex + numberOfElements, fromIndex - toIndex);
			}
			else
			{
				int num = fromIndex - toIndex;
				Array.Copy(StoreItems, toIndex, StoreItems, fromIndex + numberOfElements - num, num);
			}
		}
		else if (fromIndex + numberOfElements <= toIndex)
		{
			Array.Copy(StoreItems, fromIndex + numberOfElements, StoreItems, fromIndex, toIndex - fromIndex);
		}
		else
		{
			Array.Copy(StoreItems, fromIndex + numberOfElements, StoreItems, fromIndex, toIndex - fromIndex);
		}
		Array.Copy(array, 0, StoreItems, toIndex, numberOfElements);
	}

	protected internal void StoreForEachKey(Action<int> action)
	{
		int storeCount = StoreCount;
		TStoreType y = default;
		EqualityComparer<TStoreType> equalityComparer = EqualityComparer<TStoreType>.Default;
		for (int i = 0; i < storeCount; i++)
		{
			TStoreType x = StoreItems[i];
			if (!equalityComparer.Equals(x, y))
			{
				action(i);
			}
		}
	}

	protected internal void StoreForEachKeyValue(Action<int, TStoreType> action)
	{
		int storeCount = StoreCount;
		for (int i = 0; i < storeCount; i++)
		{
			action(i, StoreItems[i]);
		}
	}

	protected internal TStoreType[] StoreCopyRawValues()
	{
		TStoreType[] array = new TStoreType[StoreCount];
		Array.Copy(StoreItems, array, StoreCount);
		return array;
	}
}
public abstract class BGFieldCachedA<T> : BGFieldCachedA<T, T>
{
	public override T this[int index]
	{
		get
		{
			if (index >= StoreCount)
			{
				ThrowIndexOutOfBoundOnRead(index);
			}
			return StoreItems[index];
		}
	}

	protected BGFieldCachedA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGFieldCachedA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	public override void ClearValue(int entityIndex)
	{
		if (base.events.On)
		{
			T val = this[entityIndex];
			bool flag = !EqualityComparer<T>.Default.Equals(val, default);
			if (flag)
			{
				FireBeforeValueChanged(base.Meta[entityIndex], val, default);
			}
			ClearValueNoEvent(entityIndex);
			if (flag)
			{
				FireValueChanged(base.Meta[entityIndex], val, default);
			}
		}
		else
		{
			ClearValueNoEvent(entityIndex);
		}
	}
}
