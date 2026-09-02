using System.Collections.Generic;

namespace BansheeGz.BGDatabase;

public class BGMTFieldCached<T> : BGMTField<T>
{
	protected List<T> values;

	protected internal override T this[int entityIndex]
	{
		get
		{
			return values[entityIndex];
		}
		set
		{
			values[entityIndex] = value;
		}
	}

	internal BGMTFieldCached(BGField field)
		: base(field.Id, field.Name)
	{
		MoveData(field);
	}

	private BGMTFieldCached(BGMTMeta meta, BGMTFieldCached<T> otherField)
		: base(meta, (BGMTField<T>)otherField)
	{
		values = new List<T>(otherField.values);
	}

	protected virtual void MoveData(BGField field)
	{
		if (!(field is BGStorageI<T> bGStorageI))
		{
			throw new BGException("Can not cast to BGStorageI<T>");
		}
		values = new List<T>(bGStorageI.CopyRawValues());
	}

	internal override void ResizeTo(int newCount)
	{
		if (values.Count < newCount)
		{
			values.AddRange(new T[newCount]);
		}
	}

	internal override void RemoveRange(int from, int count)
	{
		values.RemoveRange(from, count);
	}

	internal override BGMTField DeepClone(BGMTMeta meta)
	{
		return new BGMTFieldCached<T>(meta, this);
	}

	public override void CopyTo(BGField field, BGEntity entity, BGMTEntity fromEntity)
	{
		BGField<T> bGField = (BGField<T>)field;
		bGField[entity.Index] = this[fromEntity.Index];
	}
}
public abstract class BGMTFieldCached<T, TStoreValue> : BGMTField<T>, BGStorable<TStoreValue>
{
	protected List<TStoreValue> values;

	protected internal BGMTFieldCached(BGField field)
		: base(field.Id, field.Name)
	{
		MoveData(field);
	}

	protected internal BGMTFieldCached(BGMTMeta meta, BGMTFieldCached<T, TStoreValue> otherField)
		: base(meta, (BGMTField<T>)otherField)
	{
		values = new List<TStoreValue>(otherField.values);
	}

	protected virtual void MoveData(BGField field)
	{
		if (!(field is BGStorageI<TStoreValue> bGStorageI))
		{
			throw new BGException("Can not cast to BGStorageI<TStoreValue>");
		}
		values = new List<TStoreValue>(bGStorageI.CopyRawValues());
	}

	internal override void ResizeTo(int newCount)
	{
		if (values.Count < newCount)
		{
			values.AddRange(new TStoreValue[newCount]);
		}
	}

	internal override void RemoveRange(int from, int count)
	{
		values.RemoveRange(from, count);
	}

	public void SetStoredValue(int entityIndex, TStoreValue value)
	{
		values[entityIndex] = value;
	}

	public TStoreValue GetStoredValue(int entityIndex)
	{
		return values[entityIndex];
	}
}
