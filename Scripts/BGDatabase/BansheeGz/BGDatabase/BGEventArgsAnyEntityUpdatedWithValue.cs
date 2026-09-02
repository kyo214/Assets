namespace BansheeGz.BGDatabase;

public abstract class BGEventArgsAnyEntityUpdatedWithValue : BGEventArgsAnyEntityUpdated
{
	public bool IsStoredValue { get; protected set; }

	public abstract object GetOldValue();

	public abstract object GetNewValue();

	public abstract BGField GetField();

	public override string ToString()
	{
		return $"BGEventArgsAnyEntityUpdatedWithValue: field [{GetField()}], entity [{base.Entity}], oldValue [{GetOldValue()}], newValue [{GetNewValue()}]";
	}
}
public class BGEventArgsAnyEntityUpdatedWithValue<T, TStoreType> : BGEventArgsAnyEntityUpdatedWithValue
{
	private static readonly BGObjectPoolNTS<BGEventArgsAnyEntityUpdatedWithValue<T, TStoreType>> pool = new BGObjectPoolNTS<BGEventArgsAnyEntityUpdatedWithValue<T, TStoreType>>(() => new BGEventArgsAnyEntityUpdatedWithValue<T, TStoreType>());

	protected override BGObjectPool Pool => pool;

	public TStoreType OldValue { get; private set; }

	public TStoreType NewValue { get; private set; }

	public BGField<T> Field { get; private set; }

	public override object GetOldValue()
	{
		return OldValue;
	}

	public override object GetNewValue()
	{
		return NewValue;
	}

	public override BGField GetField()
	{
		return Field;
	}

	public static BGEventArgsAnyEntityUpdatedWithValue<T, TStoreType> GetInstance(BGEntity entity, BGField<T> field, TStoreType oldValue, TStoreType newValue)
	{
		BGEventArgsAnyEntityUpdatedWithValue<T, TStoreType> bGEventArgsAnyEntityUpdatedWithValue = pool.Get();
		bGEventArgsAnyEntityUpdatedWithValue.Field = field;
		bGEventArgsAnyEntityUpdatedWithValue.FieldId = field.Id;
		bGEventArgsAnyEntityUpdatedWithValue.Entity = entity;
		bGEventArgsAnyEntityUpdatedWithValue.OldValue = oldValue;
		bGEventArgsAnyEntityUpdatedWithValue.NewValue = newValue;
		return bGEventArgsAnyEntityUpdatedWithValue;
	}

	public override void Clear()
	{
		base.Clear();
		Field = null;
		OldValue = default;
		NewValue = default;
	}
}
public class BGEventArgsAnyEntityUpdatedWithValue<T> : BGEventArgsAnyEntityUpdatedWithValue
{
	private static readonly BGObjectPoolNTS<BGEventArgsAnyEntityUpdatedWithValue<T>> pool = new BGObjectPoolNTS<BGEventArgsAnyEntityUpdatedWithValue<T>>(() => new BGEventArgsAnyEntityUpdatedWithValue<T>());

	protected override BGObjectPool Pool => pool;

	public T OldValue { get; private set; }

	public T NewValue { get; private set; }

	public BGField<T> Field { get; private set; }

	public override object GetOldValue()
	{
		return OldValue;
	}

	public override object GetNewValue()
	{
		return NewValue;
	}

	public override BGField GetField()
	{
		return Field;
	}

	public static BGEventArgsAnyEntityUpdatedWithValue<T> GetInstance(BGEntity entity, BGField<T> field, T oldValue, T newValue)
	{
		BGEventArgsAnyEntityUpdatedWithValue<T> bGEventArgsAnyEntityUpdatedWithValue = pool.Get();
		bGEventArgsAnyEntityUpdatedWithValue.Field = field;
		bGEventArgsAnyEntityUpdatedWithValue.FieldId = field.Id;
		bGEventArgsAnyEntityUpdatedWithValue.Entity = entity;
		bGEventArgsAnyEntityUpdatedWithValue.OldValue = oldValue;
		bGEventArgsAnyEntityUpdatedWithValue.NewValue = newValue;
		return bGEventArgsAnyEntityUpdatedWithValue;
	}

	public override void Clear()
	{
		base.Clear();
		Field = null;
		OldValue = default;
		NewValue = default;
	}
}
