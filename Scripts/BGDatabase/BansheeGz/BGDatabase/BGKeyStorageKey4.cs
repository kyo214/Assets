using System;

namespace BansheeGz.BGDatabase;

internal class BGKeyStorageKey4 : BGKeyStorageKeyI, IEquatable<BGKeyStorageKeyI>
{
	internal object Value0;

	internal object Value1;

	internal object Value2;

	internal object Value3;

	public BGKeyStorageKey4(object value0, object value1, object value2, object value3)
	{
		Value0 = value0;
		Value1 = value1;
		Value2 = value2;
		Value3 = value3;
	}

	public override int GetHashCode()
	{
		int num = ((Value0 != null) ? Value0.GetHashCode() : 0);
		num = (num * 397) ^ ((Value1 != null) ? Value1.GetHashCode() : 0);
		num = (num * 397) ^ ((Value2 != null) ? Value2.GetHashCode() : 0);
		return (num * 397) ^ ((Value3 != null) ? Value3.GetHashCode() : 0);
	}

	public bool IsValueEquals(object value, int index)
	{
		return index switch
		{
			0 => object.Equals(Value0, value), 
			1 => object.Equals(Value1, value), 
			2 => object.Equals(Value2, value), 
			3 => object.Equals(Value3, value), 
			_ => false, 
		};
	}

	public BGKeyStorageKeyI Clone()
	{
		return new BGKeyStorageKey4(Value0, Value1, Value2, Value3);
	}

	public bool Equals(BGKeyStorageKeyI key)
	{
		if (key.IsValueEquals(Value0, 0) && key.IsValueEquals(Value1, 1) && key.IsValueEquals(Value2, 2))
		{
			return key.IsValueEquals(Value3, 3);
		}
		return false;
	}
}
internal class BGKeyStorageKey4<T0, T1, T2, T3> : BGKeyStorageKeyI, IEquatable<BGKeyStorageKeyI>
{
	public static readonly BGObjectPool<BGKeyStorageKey4<T0, T1, T2, T3>> Pool = new BGObjectPool<BGKeyStorageKey4<T0, T1, T2, T3>>(() => new BGKeyStorageKey4<T0, T1, T2, T3>(default, default, default, default), (BGKeyStorageKey4<T0, T1, T2, T3> k) =>
	{
		k.Value0 = default;
		k.Value1 = default;
		k.Value2 = default;
		k.Value3 = default;
	});

	internal T0 Value0;

	internal T1 Value1;

	internal T2 Value2;

	internal T3 Value3;

	public BGKeyStorageKey4(T0 value0, T1 value1, T2 value2, T3 value3)
	{
		Value0 = value0;
		Value1 = value1;
		Value2 = value2;
		Value3 = value3;
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
		case 3:
			if (Value3 != null)
			{
				return Value3.Equals(otherValue);
			}
			return otherValue == null;
		default:
			return false;
		}
	}

	public BGKeyStorageKeyI Clone()
	{
		return new BGKeyStorageKey4<T0, T1, T2, T3>(Value0, Value1, Value2, Value3);
	}

	public bool Equals(BGKeyStorageKeyI key)
	{
		if (key == null)
		{
			return false;
		}
		if (key is BGKeyStorageKey4 { Value0: var value } bGKeyStorageKey)
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
			if (!((Value2 == null) ? (value3 == null) : Value2.Equals(value3)))
			{
				return false;
			}
			object value4 = bGKeyStorageKey.Value3;
			if (Value3 != null)
			{
				return Value3.Equals(value4);
			}
			return value4 == null;
		}
		BGKeyStorageKey4<T0, T1, T2, T3> bGKeyStorageKey2 = (BGKeyStorageKey4<T0, T1, T2, T3>)key;
		T0 value5 = bGKeyStorageKey2.Value0;
		if (!((Value0 == null) ? (value5 == null) : Value0.Equals(value5)))
		{
			return false;
		}
		T1 value6 = bGKeyStorageKey2.Value1;
		if (!((Value1 == null) ? (value6 == null) : Value1.Equals(value6)))
		{
			return false;
		}
		T2 value7 = bGKeyStorageKey2.Value2;
		if (!((Value2 == null) ? (value7 == null) : Value2.Equals(value7)))
		{
			return false;
		}
		T3 value8 = bGKeyStorageKey2.Value3;
		if (Value3 != null)
		{
			return Value3.Equals(value8);
		}
		return value8 == null;
	}

	public override int GetHashCode()
	{
		int num = ((Value0 != null) ? Value0.GetHashCode() : 0);
		num = (num * 397) ^ ((Value1 != null) ? Value1.GetHashCode() : 0);
		num = (num * 397) ^ ((Value2 != null) ? Value2.GetHashCode() : 0);
		return (num * 397) ^ ((Value3 != null) ? Value3.GetHashCode() : 0);
	}
}
