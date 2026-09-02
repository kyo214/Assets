using System.Globalization;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeShort : BGCalcTypeCode<short>
{
	public const byte Code = 18;

	public override bool SupportDefaultValue => true;

	public override byte TypeCode => 18;

	public override object DefaultValue => (short)0;

	public override string Name => "short";

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		writer.AddShort((short)value);
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		return reader.ReadShort();
	}

	public override string ValueToString(object value)
	{
		return value.ToString();
	}

	public override object ValueFromString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return (short)0;
		}
		return short.Parse(value, CultureInfo.InvariantCulture);
	}

	public override bool CanBeConvertedFrom(BGCalcTypeCode otherCode)
	{
		if (otherCode == null)
		{
			return false;
		}
		byte typeCode = otherCode.TypeCode;
		if ((uint)(typeCode - 4) <= 1u || typeCode == 17 || (uint)(typeCode - 19) <= 1u)
		{
			return true;
		}
		return false;
	}

	public override object ConvertFrom(BGCalcTypeCode otherCode, object value)
	{
		if (otherCode == null)
		{
			return value;
		}
		return otherCode.TypeCode switch
		{
			5 => (short)(float)value, 
			17 => (short)(byte)value, 
			4 => (short)(int)value, 
			19 => (short)(sbyte)value, 
			20 => (short)(ushort)value, 
			_ => value, 
		};
	}
}
