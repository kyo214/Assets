using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public abstract class BGFieldDictionaryBasedA<T, TStoreType> : BGField<T>, BGStorable<TStoreType>
{
	protected readonly Dictionary<BGId, TStoreType> storage = new Dictionary<BGId, TStoreType>();

	public override T this[BGId entityId]
	{
		get
		{
			BGEntity entity = base.Meta.GetEntity(entityId);
			if (entity == null)
			{
				throw new BGException("Can not get entity with Id=$", entityId);
			}
			storage.TryGetValue(entityId, out var value);
			return Convert(entity, value);
		}
		set
		{
			BGEntity entity = base.Meta.GetEntity(entityId);
			if (entity == null)
			{
				throw new BGException("Can not get entity with Id=$", entityId);
			}
			Set(entity, value);
		}
	}

	public override T this[int index]
	{
		get
		{
			BGEntity entity = base.Meta.GetEntity(index);
			if (entity == null)
			{
				throw new BGException("Can not get entity with index=$", index);
			}
			storage.TryGetValue(entity.Id, out var value);
			return Convert(entity, value);
		}
		set
		{
			BGEntity entity = base.Meta.GetEntity(index);
			if (entity == null)
			{
				throw new BGException("Can not get entity with index=$", index);
			}
			Set(entity, value);
		}
	}

	protected BGFieldDictionaryBasedA(BGMetaEntity meta, string name)
		: base(meta, name)
	{
	}

	protected BGFieldDictionaryBasedA(BGMetaEntity meta, BGId id, string name)
		: base(meta, id, name)
	{
	}

	private void Set(BGEntity e, T value)
	{
		if (!storage.TryGetValue(e.Id, out var value2))
		{
			value2 = default;
		}
		TStoreType val = Convert(e, value);
		storage[e.Id] = val;
		FireStoredValueChanged(e, value2, val);
	}

	public void SetStoredValue(int entityIndex, TStoreType value)
	{
		BGEntity entity = base.Meta.GetEntity(entityIndex);
		if (entity == null)
		{
			throw new BGException("Can not get entity with index=$", entityIndex);
		}
		if (!storage.TryGetValue(entity.Id, out var value2))
		{
			value2 = default;
		}
		storage[entity.Id] = value;
		FireStoredValueChanged(entity, value2, value);
	}

	public TStoreType GetStoredValue(int entityIndex)
	{
		BGEntity entity = base.Meta.GetEntity(entityIndex);
		if (entity == null)
		{
			throw new BGException("Can not get entity with index=$", entityIndex);
		}
		if (storage.TryGetValue(entity.Id, out var value))
		{
			return value;
		}
		return default;
	}

	public override bool AreStoredValuesEqual(BGField field, int myEntityIndex, int otherEntityIndex)
	{
		if (!(field is BGFieldDictionaryBasedA<T, TStoreType> bGFieldDictionaryBasedA))
		{
			return false;
		}
		BGEntity entity = base.Meta.GetEntity(myEntityIndex);
		BGEntity entity2 = bGFieldDictionaryBasedA.Meta.GetEntity(otherEntityIndex);
		TStoreType myValue = default;
		if (storage.TryGetValue(entity.Id, out var value))
		{
			myValue = value;
		}
		TStoreType otherValue = default;
		if (bGFieldDictionaryBasedA.storage.TryGetValue(entity2.Id, out var value2))
		{
			otherValue = value2;
		}
		return AreStoredValuesEqual(myValue, otherValue);
	}

	protected virtual bool AreStoredValuesEqual(TStoreType myValue, TStoreType otherValue)
	{
		return object.Equals(myValue, otherValue);
	}

	public override void MoveEntitiesValues(int fromIndex, int toIndex, int numberOfValues)
	{
	}

	public override void Swap(int entityIndex1, int entityIndex2)
	{
	}

	public override void CopyValue(BGField fromField, BGId fromEntityId, int fromEntityIndex, BGId toEntityId)
	{
		if (!(fromField is BGFieldDictionaryBasedA<T, TStoreType> bGFieldDictionaryBasedA) || fromEntityIndex == -1 || bGFieldDictionaryBasedA.IsDeleted)
		{
			return;
		}
		BGEntity entity = base.Meta.GetEntity(toEntityId);
		if (entity == null)
		{
			return;
		}
		TStoreType val = default;
		if (bGFieldDictionaryBasedA.storage.TryGetValue(fromEntityId, out var value))
		{
			val = value;
		}
		if (val == null)
		{
			storage.Remove(toEntityId);
		}
		else if (val.GetType().IsValueType)
		{
			storage[toEntityId] = val;
		}
		else if (!(val is BGFieldDictionaryClonebleValueI bGFieldDictionaryClonebleValueI))
		{
			if (!(val is ICloneable cloneable))
			{
				throw new Exception("Can not copy value cause the value is not cloneable and not a struct");
			}
			storage[toEntityId] = (TStoreType)cloneable.Clone();
		}
		else
		{
			storage[toEntityId] = (TStoreType)bGFieldDictionaryClonebleValueI.CloneTo(entity);
		}
	}

	public override void DuplicateValue(BGId fromEntityId, int fromEntityIndex, BGId toEntityId)
	{
		CopyValue(this, fromEntityId, fromEntityIndex, toEntityId);
	}

	public override void ClearValue(int entityIndex)
	{
		BGEntity entity = base.Meta.GetEntity(entityIndex);
		if (base.events.On)
		{
			if (storage.TryGetValue(entity.Id, out var value))
			{
				ClearValueNoEvent(entity.Id);
				FireStoredValueChanged(base.Meta[entityIndex], value, default);
			}
		}
		else
		{
			ClearValueNoEvent(entity.Id);
		}
	}

	private void ClearValueNoEvent(BGId id)
	{
		storage.Remove(id);
	}

	public override void ClearValues()
	{
		storage.Clear();
	}

	public override void OnDelete()
	{
		ClearValues();
	}

	public override void ForEachValue(Action<int> action)
	{
		foreach (BGId key in storage.Keys)
		{
			BGEntity entity = base.Meta.GetEntity(key);
			if (entity != null)
			{
				action(entity.Index);
			}
		}
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

	public override void OnEntityDelete(BGEntity entity)
	{
		storage.Remove(entity.Id);
	}

	public void FireStoredValueChanged(BGEntity entity, TStoreType oldValue, TStoreType newValue)
	{
		if (!base.events.ConsumeOnChange(base.MetaId))
		{
			using (BGEventArgsFieldWithValue<T, TStoreType> eventArgs = BGEventArgsFieldWithValue<T, TStoreType>.GetInstance(entity, this, oldValue, newValue))
			{
				FireValueChanged(eventArgs);
			}
			base.Meta.FireValueChanged(this, entity, nested: true);
			base.events.FireAnyChange();
		}
	}

	public override byte[] ToBytes(int entityIndex)
	{
		TStoreType storedValue = GetStoredValue(entityIndex);
		if (storedValue == null)
		{
			return null;
		}
		return ValueToBytes(storedValue);
	}

	public override void FromBytes(int entityIndex, ArraySegment<byte> segment)
	{
		SetStoredValue(entityIndex, ValueFromBytes(entityIndex, segment));
	}

	public override string ToString(int entityIndex)
	{
		TStoreType storedValue = GetStoredValue(entityIndex);
		if (storedValue == null)
		{
			return null;
		}
		return ValueToString(storedValue);
	}

	public override void FromString(int entityIndex, string value)
	{
		SetStoredValue(entityIndex, ValueFromString(entityIndex, value));
	}

	protected abstract TStoreType Convert(BGEntity entity, T value);

	protected abstract T Convert(BGEntity entity, TStoreType value);

	protected abstract byte[] ValueToBytes(TStoreType value);

	protected abstract TStoreType ValueFromBytes(int entityIndex, ArraySegment<byte> segment);

	protected abstract string ValueToString(TStoreType value);

	protected abstract TStoreType ValueFromString(int entityIndex, string value);
}
