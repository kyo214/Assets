using System;

namespace BansheeGz.BGDatabase;

internal class BGKeyStorageKey1 : BGKeyStorageKeyI, IEquatable<BGKeyStorageKeyI>
{
	internal object Value0;

	public BGKeyStorageKey1(object value0)
	{
		Value0 = value0;
	}

	public bool IsValueEquals(object value, int index)
	{
		return object.Equals(Value0, value);
	}

	public BGKeyStorageKeyI Clone()
	{
		return new BGKeyStorageKey1(Value0);
	}

	public override int GetHashCode()
	{
		if (Value0 == null)
		{
			return 0;
		}
		return Value0.GetHashCode();
	}

	public bool Equals(BGKeyStorageKeyI key)
	{
		return key.IsValueEquals(Value0, 0);
	}
}
internal class BGKeyStorageKey1<T0> : BGKeyStorageKeyI, IEquatable<BGKeyStorageKeyI>
{
	public static readonly BGObjectPool<BGKeyStorageKey1<T0>> Pool = new BGObjectPool<BGKeyStorageKey1<T0>>(() => new BGKeyStorageKey1<T0>(default), (BGKeyStorageKey1<T0> k) =>
	{
		k.Value0 = default;
	});

	internal T0 Value0;

	private BGKeyStorageKey1(T0 value0)
	{
		Value0 = value0;
	}

	public bool IsValueEquals(object otherValue, int index)
	{
		if (Value0 != null)
		{
			return Value0.Equals(otherValue);
		}
		return otherValue == null;
	}

	public BGKeyStorageKeyI Clone()
	{
		return new BGKeyStorageKey1<T0>(Value0);
	}

	public bool Equals(BGKeyStorageKeyI key)
	{
		if (key == null)
		{
			return false;
		}
		if (key is BGKeyStorageKey1 bGKeyStorageKey)
		{
			return IsValueEquals(bGKeyStorageKey.Value0, 0);
		}
		return IsValueEquals(((BGKeyStorageKey1<T0>)key).Value0, 0);
	}

	public override int GetHashCode()
	{
		if (Value0 == null)
		{
			return 0;
		}
		return Value0.GetHashCode();
	}
}
