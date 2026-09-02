using System;

namespace BansheeGz.BGDatabase;

internal class BGKeyStorageKey2 : BGKeyStorageKeyI, IEquatable<BGKeyStorageKeyI>
{
	internal object Value0;

	internal object Value1;

	public BGKeyStorageKey2(object value0, object value1)
	{
		Value0 = value0;
		Value1 = value1;
	}

	public override int GetHashCode()
	{
		return (((Value0 != null) ? Value0.GetHashCode() : 0) * 397) ^ ((Value1 != null) ? Value1.GetHashCode() : 0);
	}

	public bool IsValueEquals(object value, int index)
	{
		return index switch
		{
			0 => object.Equals(Value0, value), 
			1 => object.Equals(Value1, value), 
			_ => false, 
		};
	}

	public BGKeyStorageKeyI Clone()
	{
		return new BGKeyStorageKey2(Value0, Value1);
	}

	public bool Equals(BGKeyStorageKeyI key)
	{
		if (key.IsValueEquals(Value0, 0))
		{
			return key.IsValueEquals(Value1, 1);
		}
		return false;
	}
}
internal class BGKeyStorageKey2<T0, T1> : BGKeyStorageKeyI, IEquatable<BGKeyStorageKeyI>
{
	public static readonly BGObjectPool<BGKeyStorageKey2<T0, T1>> Pool = new BGObjectPool<BGKeyStorageKey2<T0, T1>>(() => new BGKeyStorageKey2<T0, T1>(default, default), (BGKeyStorageKey2<T0, T1> k) =>
	{
		k.Value0 = default;
		k.Value1 = default;
	});

	internal T0 Value0;

	internal T1 Value1;

	public BGKeyStorageKey2(T0 value0, T1 value1)
	{
		Value0 = value0;
		Value1 = value1;
	}

	public bool IsValueEquals(object otherValue, int index)
	{
		switch (index)
		{
		case 0:
			if (Value0 != null)
			{
				return Value0.Equals(otherValue);
			}
			return otherValue == null;
		case 1:
			if (Value1 != null)
			{
				return Value1.Equals(otherValue);
			}
			return otherValue == null;
		default:
			return false;
		}
	}

	public BGKeyStorageKeyI Clone()
	{
		return new BGKeyStorageKey2<T0, T1>(Value0, Value1);
	}

	public bool Equals(BGKeyStorageKeyI key)
	{
		if (key == null)
		{
			return false;
		}
		if (key is BGKeyStorageKey2 { Value0: var value } bGKeyStorageKey)
		{
			if (!((Value0 == null) ? (value == null) : Value0.Equals(value)))
			{
				return false;
			}
			object value2 = bGKeyStorageKey.Value1;
			if (Value1 != null)
			{
				return Value1.Equals(value2);
			}
			return value2 == null;
		}
		BGKeyStorageKey2<T0, T1> bGKeyStorageKey2 = (BGKeyStorageKey2<T0, T1>)key;
		T0 value3 = bGKeyStorageKey2.Value0;
		if (!((Value0 == null) ? (value3 == null) : Value0.Equals(value3)))
		{
			return false;
		}
		T1 value4 = bGKeyStorageKey2.Value1;
		if (Value1 != null)
		{
			return Value1.Equals(value4);
		}
		return value4 == null;
	}

	public override int GetHashCode()
	{
		return (((Value0 != null) ? Value0.GetHashCode() : 0) * 397) ^ ((Value1 != null) ? Value1.GetHashCode() : 0);
	}
}
