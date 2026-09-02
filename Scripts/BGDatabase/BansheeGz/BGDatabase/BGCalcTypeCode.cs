using System;

namespace BansheeGz.BGDatabase;

public abstract class BGCalcTypeCode
{
	public abstract byte TypeCode { get; }

	public abstract string Name { get; }

	public abstract Type Type { get; }

	public abstract bool SupportDefaultValue { get; }

	public abstract object DefaultValue { get; }

	public virtual string TypeTitle => Name;

	public virtual bool AreEqual(object o1, object o2)
	{
		return object.Equals(o1, o2);
	}

	public abstract void ValueToBytes(BGBinaryWriter writer, object value);

	public abstract object ValueFromBytes(BGBinaryReader reader);

	public abstract string ValueToString(object value);

	public abstract object ValueFromString(string value);

	public virtual bool CanBeConvertedFrom(BGCalcTypeCode otherCode)
	{
		return false;
	}

	public virtual object ConvertFrom(BGCalcTypeCode otherCode, object value)
	{
		return value;
	}

	protected bool Equals(BGCalcTypeCode other)
	{
		return TypeCode == other.TypeCode;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (this == obj)
		{
			return true;
		}
		if (obj.GetType() != GetType())
		{
			return false;
		}
		return Equals((BGCalcTypeCode)obj);
	}

	public override int GetHashCode()
	{
		return TypeCode;
	}
}
public abstract class BGCalcTypeCode<T> : BGCalcTypeCode
{
	public override Type Type => typeof(T);
}
