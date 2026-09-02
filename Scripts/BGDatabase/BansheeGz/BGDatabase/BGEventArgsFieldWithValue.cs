namespace BansheeGz.BGDatabase;

public abstract class BGEventArgsFieldWithValue : BGEventArgsField
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
public class BGEventArgsFieldWithValue<T, TStoreType> : BGEventArgsFieldWithValue
{
	private static readonly BGObjectPoolNTS<BGEventArgsFieldWithValue<T, TStoreType>> pool = new BGObjectPoolNTS<BGEventArgsFieldWithValue<T, TStoreType>>(() => new BGEventArgsFieldWithValue<T, TStoreType>());

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

	private BGEventArgsFieldWithValue()
	{
	}

	public static BGEventArgsFieldWithValue<T, TStoreType> GetInstance(BGEntity entity, BGField<T> field, TStoreType oldValue, TStoreType newValue)
	{
		BGEventArgsFieldWithValue<T, TStoreType> bGEventArgsFieldWithValue = pool.Get();
		bGEventArgsFieldWithValue.Field = field;
		bGEventArgsFieldWithValue.FieldId = field.Id;
		bGEventArgsFieldWithValue.Entity = entity;
		bGEventArgsFieldWithValue.OldValue = oldValue;
		bGEventArgsFieldWithValue.NewValue = newValue;
		return bGEventArgsFieldWithValue;
	}

	public override void Clear()
	{
		base.Clear();
		Field = null;
		OldValue = default;
		NewValue = default;
	}
}
public class BGEventArgsFieldWithValue<T> : BGEventArgsFieldWithValue
{
	private static readonly BGObjectPoolNTS<BGEventArgsFieldWithValue<T>> pool = new BGObjectPoolNTS<BGEventArgsFieldWithValue<T>>(() => new BGEventArgsFieldWithValue<T>());

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

	private BGEventArgsFieldWithValue()
	{
	}

	public static BGEventArgsFieldWithValue<T> GetInstance(BGEntity entity, BGField<T> field, T oldValue, T newValue)
	{
		BGEventArgsFieldWithValue<T> bGEventArgsFieldWithValue = pool.Get();
		bGEventArgsFieldWithValue.Field = field;
		bGEventArgsFieldWithValue.FieldId = field.Id;
		bGEventArgsFieldWithValue.Entity = entity;
		bGEventArgsFieldWithValue.OldValue = oldValue;
		bGEventArgsFieldWithValue.NewValue = newValue;
		return bGEventArgsFieldWithValue;
	}

	public override void Clear()
	{
		base.Clear();
		Field = null;
		OldValue = default;
		NewValue = default;
	}
}
