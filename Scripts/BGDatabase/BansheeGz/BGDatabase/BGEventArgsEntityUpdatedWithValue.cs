namespace BansheeGz.BGDatabase;

public abstract class BGEventArgsEntityUpdatedWithValue : BGEventArgsEntityUpdated
{
	public bool IsStoredValue { get; protected set; }

	public abstract object GetOldValue();

	public abstract object GetNewValue();

	public abstract BGField GetField();

	public override string ToString()
	{
		return $"BGEventArgsEntityUpdatedWithValue: field [{GetField()}], entity [{base.Entity}], oldValue [{GetOldValue()}], newValue [{GetNewValue()}]";
	}
}
public class BGEventArgsEntityUpdatedWithValue<T, TStoreType> : BGEventArgsEntityUpdatedWithValue
{
	private static readonly BGObjectPoolNTS<BGEventArgsEntityUpdatedWithValue<T, TStoreType>> pool = new BGObjectPoolNTS<BGEventArgsEntityUpdatedWithValue<T, TStoreType>>(() => new BGEventArgsEntityUpdatedWithValue<T, TStoreType>());

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

	public static BGEventArgsEntityUpdatedWithValue<T, TStoreType> GetInstance(BGEntity entity, BGField<T> field, TStoreType oldValue, TStoreType newValue)
	{
		BGEventArgsEntityUpdatedWithValue<T, TStoreType> bGEventArgsEntityUpdatedWithValue = pool.Get();
		bGEventArgsEntityUpdatedWithValue.Field = field;
		bGEventArgsEntityUpdatedWithValue.FieldId = field.Id;
		bGEventArgsEntityUpdatedWithValue.Entity = entity;
		bGEventArgsEntityUpdatedWithValue.OldValue = oldValue;
		bGEventArgsEntityUpdatedWithValue.NewValue = newValue;
		return bGEventArgsEntityUpdatedWithValue;
	}

	public override void Clear()
	{
		base.Clear();
		Field = null;
		OldValue = default;
		NewValue = default;
	}
}
public class BGEventArgsEntityUpdatedWithValue<T> : BGEventArgsEntityUpdatedWithValue
{
	private static readonly BGObjectPoolNTS<BGEventArgsEntityUpdatedWithValue<T>> pool = new BGObjectPoolNTS<BGEventArgsEntityUpdatedWithValue<T>>(() => new BGEventArgsEntityUpdatedWithValue<T>());

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

	public static BGEventArgsEntityUpdatedWithValue<T> GetInstance(BGEntity entity, BGField<T> field, T oldValue, T newValue)
	{
		BGEventArgsEntityUpdatedWithValue<T> bGEventArgsEntityUpdatedWithValue = pool.Get();
		bGEventArgsEntityUpdatedWithValue.Field = field;
		bGEventArgsEntityUpdatedWithValue.FieldId = field.Id;
		bGEventArgsEntityUpdatedWithValue.Entity = entity;
		bGEventArgsEntityUpdatedWithValue.OldValue = oldValue;
		bGEventArgsEntityUpdatedWithValue.NewValue = newValue;
		return bGEventArgsEntityUpdatedWithValue;
	}

	public override void Clear()
	{
		base.Clear();
		Field = null;
		OldValue = default;
		NewValue = default;
	}
}
