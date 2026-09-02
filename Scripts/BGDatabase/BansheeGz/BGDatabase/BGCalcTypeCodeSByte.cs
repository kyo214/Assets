using System.Globalization;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeSByte : BGCalcTypeCode<sbyte>
{
	public const byte Code = 19;

	public override bool SupportDefaultValue => true;

	public override byte TypeCode => 19;

	public override object DefaultValue => (sbyte)0;

	public override string Name => "sbyte";

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		writer.AddSByte((sbyte)value);
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		return reader.ReadSByte();
	}

	public override string ValueToString(object value)
	{
		return value.ToString();
	}

	public override object ValueFromString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return (sbyte)0;
		}
		return sbyte.Parse(value, CultureInfo.InvariantCulture);
	}

	public override bool CanBeConvertedFrom(BGCalcTypeCode otherCode)
	{
		if (otherCode == null)
		{
			return false;
		}
		byte typeCode = otherCode.TypeCode;
		if ((uint)(typeCode - 4) <= 1u || (uint)(typeCode - 17) <= 1u || typeCode == 20)
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
			5 => (sbyte)(float)value, 
			4 => (sbyte)(int)value, 
			18 => (sbyte)(short)value, 
			17 => (sbyte)(byte)value, 
			20 => (sbyte)(ushort)value, 
			_ => value, 
		};
	}
}
