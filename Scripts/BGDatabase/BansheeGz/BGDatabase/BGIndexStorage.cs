using System;
using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

internal abstract class BGIndexStorage
{
	protected bool dirty;

	public readonly BGIndex dbIndex;

	public abstract int Count { get; }

	public BGIndexStorage(BGIndex dbIndex)
	{
		this.dbIndex = dbIndex;
	}

	public void MarkDirty()
	{
		dirty = true;
	}
}
internal class BGIndexStorage<T> : BGIndexStorage where T : IComparable<T>
{
	public readonly BGField<T> typedField;

	private readonly SortedSet<BGIndexStorageItem<T>> store = new SortedSet<BGIndexStorageItem<T>>();

	public override int Count => store.Count;

	public T Min => store.Min.key;

	public T Max => store.Max.key;

	public BGIndexStorage(BGIndex dbIndex, BGField<T> typedField)
		: base(dbIndex)
	{
		this.typedField = typedField;
		AttachListeners();
		Build();
	}

	private void AttachListeners()
	{
		dbIndex.OnUnload += Dispose;
		dbIndex.Meta.Repo.Events.OnBatchUpdate += BatchListener;
		dbIndex.Meta.AnyEntityAdded += EntityAddedListener;
		dbIndex.Meta.AnyEntityBeforeDeleted += EntityBeforeDeletedListener;
		dbIndex.Meta.EntitiesOrderChanged += EntityOrderChangedListener;
		dbIndex.Field.ValueChanged += FieldValueListener;
	}

	private void Dispose(BGObject obj)
	{
		dbIndex.OnUnload -= Dispose;
		dbIndex.Meta.Repo.Events.OnBatchUpdate -= BatchListener;
		dbIndex.Meta.AnyEntityAdded -= EntityAddedListener;
		dbIndex.Meta.AnyEntityBeforeDeleted -= EntityBeforeDeletedListener;
		dbIndex.Meta.EntitiesOrderChanged -= EntityOrderChangedListener;
		dbIndex.Field.ValueChanged -= FieldValueListener;
	}

	private void BatchListener(object sender, BGEventArgsBatch e)
	{
		if (!dirty)
		{
			BGId id = dbIndex.Meta.Id;
			if (e.WasEntitiesAdded(id) || e.WasEntitiesDeleted(id) || e.WasEntitiesUpdated(id) || e.WasEntitiesOrderChanged(id))
			{
				dirty = true;
			}
		}
	}

	private void EntityOrderChangedListener(object sender, BGEventArgsEntitiesOrder e)
	{
		dirty = true;
	}

	private void EntityBeforeDeletedListener(object sender, BGEventArgsAnyEntity e)
	{
		if (!dirty && !Remove(e.Entity))
		{
			dirty = true;
		}
	}

	private void EntityAddedListener(object sender, BGEventArgsAnyEntity e)
	{
		if (!dirty)
		{
			BGEntity entity = e.Entity;
			T key = GetKey(entity.Index);
			Add(key, entity);
		}
	}

	private void FieldValueListener(object sender, BGEventArgsField e)
	{
		dirty = true;
		if (e is BGEventArgsFieldWithValue bGEventArgsFieldWithValue)
		{
			BGField field = bGEventArgsFieldWithValue.GetField();
			object oldValue = bGEventArgsFieldWithValue.GetOldValue();
			object newValue = bGEventArgsFieldWithValue.GetNewValue();
			if (Remove((T)oldValue, e.Entity))
			{
				Add((T)newValue, e.Entity);
				dirty = false;
			}
		}
	}

	private void Build()
	{
		store.Clear();
		typedField.Meta.ForEachEntity((BGEntity entity) =>
		{
			Add(typedField[entity.Index], entity);
		});
	}

	private T GetKey(int entityIndex)
	{
		return typedField[entityIndex];
	}

	private T GetKey(BGEntity entity)
	{
		return typedField[entity.Index];
	}

	public void Add(BGEntity entity)
	{
		Add(GetKey(entity), entity);
	}

	internal void Add(T key, BGEntity entity)
	{
		store.Add(new BGIndexStorageItem<T>(key, entity));
	}

	private bool Remove(BGEntity entity)
	{
		return Remove(GetKey(entity), entity);
	}

	private bool Remove(T key, BGEntity entity)
	{
		return store.Remove(new BGIndexStorageItem<T>(key, entity));
	}

	internal void GetRange<TEntity>(List<TEntity> result, BGIndexStorageItem<T> from, BGIndexStorageItem<T> to, bool fromInclusive, bool toInclusive) where TEntity : BGEntity
	{
		bool flag = fromInclusive || BGIndexStorageItem<T>.EternityMinus == from;
		using (SortedSet<BGIndexStorageItem<T>>.Enumerator enumerator = store.GetViewBetween(from, to).GetEnumerator())
		{
			if (!flag)
			{
				while (enumerator.MoveNext())
				{
					BGIndexStorageItem<T> current = enumerator.Current;
					if (!object.Equals(current.key, from.key))
					{
						flag = true;
						result.Add((TEntity)current.entity);
						break;
					}
				}
			}
			if (flag)
			{
				while (enumerator.MoveNext())
				{
					BGIndexStorageItem<T> current2 = enumerator.Current;
					result.Add((TEntity)current2.entity);
				}
			}
		}
		if (toInclusive || BGIndexStorageItem<T>.Eternity == to)
		{
			return;
		}
		int num = result.Count - 1;
		while (num >= 0)
		{
			TEntity val = result[num];
			T val2 = typedField[val.Index];
			if (object.Equals(val2, to.key))
			{
				result.RemoveAt(num);
				num--;
				continue;
			}
			break;
		}
	}
}
