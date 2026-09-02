using System;

namespace BansheeGz.BGDatabase;

internal class BGKeyStorageKey3 : BGKeyStorageKeyI, IEquatable<BGKeyStorageKeyI>
{
	internal object Value0;

	internal object Value1;

	internal object Value2;

	public BGKeyStorageKey3(object value0, object value1, object value2)
	{
		Value0 = value0;
		Value1 = value1;
		Value2 = value2;
	}

	public override int GetHashCode()
	{
		int num = ((Value0 != null) ? Value0.GetHashCode() : 0);
		num = (num * 397) ^ ((Value1 != null) ? Value1.GetHashCode() : 0);
		return (num * 397) ^ ((Value2 != null) ? Value2.GetHashCode() : 0);
	}

	public bool IsValueEquals(object value, int index)
	{
		return index switch
		{
			0 => object.Equals(Value0, value), 
			1 => object.Equals(Value1, value), 
			2 => object.Equals(Value2, value), 
			_ => false, 
		};
	}

	public BGKeyStorageKeyI Clone()
	{
		return new BGKeyStorageKey3(Value0, Value1, Value2);
	}

	public bool Equals(BGKeyStorageKeyI key)
	{
		if (key.IsValueEquals(Value0, 0) && key.IsValueEquals(Value1, 1))
		{
			return key.IsValueEquals(Value2, 2);
		}
		return false;
	}
}
internal class BGKeyStorageKey3<T0, T1, T2> : BGKeyStorageKeyI, IEquatable<BGKeyStorageKeyI>
{
	public static readonly BGObjectPool<BGKeyStorageKey3<T0, T1, T2>> Pool = new BGObjectPool<BGKeyStorageKey3<T0, T1, T2>>(() => new BGKeyStorageKey3<T0, T1, T2>(default, default, default), (BGKeyStorageKey3<T0, T1, T2> k) =>
	{
		k.Value0 = default;
		k.Value1 = default;
		k.Value2 = default;
	});

	internal T0 Value0;

	internal T1 Value1;

	internal T2 Value2;

	public BGKeyStorageKey3(T0 value0, T1 value1, T2 value2)
	{
		Value0 = value0;
		Value1 = value1;
		Value2 = value2;
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
		case 2:
			if (Value2 != null)
			{
				return Value2.Equals(otherValue);
			}
			return otherValue == null;
		default:
			return false;
		}
	}

	public BGKeyStorageKeyI Clone()
	{
		return new BGKeyStorageKey3<T0, T1, T2>(Value0, Value1, Value2);
	}

	public bool Equals(BGKeyStorageKeyI key)
	{
		if (key == null)
		{
			return false;
		}
		if (key is BGKeyStorageKey3 { Value0: var value } bGKeyStorageKey)
		{
			if (!((Value0 == null) ? (value == null) : Value0.Equals(value)))
			{
				return false;
			}
			object value2 = bGKeyStorageKey.Value1;
			if (!((Value1 == null) ? (value2 == null) : Value1.Equals(value2)))
			{
				return false;
			}
			object value3 = bGKeyStorageKey.Value2;
			if (Value2 != null)
			{
				return Value2.Equals(value3);
			}
			return value3 == null;
		}
		BGKeyStorageKey3<T0, T1, T2> bGKeyStorageKey2 = (BGKeyStorageKey3<T0, T1, T2>)key;
		T0 value4 = bGKeyStorageKey2.Value0;
		if (!((Value0 == null) ? (value4 == null) : Value0.Equals(value4)))
		{
			return false;
		}
		T1 value5 = bGKeyStorageKey2.Value1;
		if (!((Value1 == null) ? (value5 == null) : Value1.Equals(value5)))
		{
			return false;
		}
		T2 value6 = bGKeyStorageKey2.Value2;
		if (Value2 != null)
		{
			return Value2.Equals(value6);
		}
		return value6 == null;
	}

	public override int GetHashCode()
	{
		int num = ((Value0 != null) ? Value0.GetHashCode() : 0);
		num = (num * 397) ^ ((Value1 != null) ? Value1.GetHashCode() : 0);
		return (num * 397) ^ ((Value2 != null) ? Value2.GetHashCode() : 0);
	}
}
