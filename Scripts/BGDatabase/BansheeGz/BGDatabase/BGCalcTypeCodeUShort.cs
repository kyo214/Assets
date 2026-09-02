using System.Globalization;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeUShort : BGCalcTypeCode<ushort>
{
	public const byte Code = 20;

	public override bool SupportDefaultValue => true;

	public override byte TypeCode => 20;

	public override object DefaultValue => (ushort)0;

	public override string Name => "ushort";

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		writer.AddUShort((ushort)value);
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		return reader.ReadUShort();
	}

	public override string ValueToString(object value)
	{
		return value.ToString();
	}

	public override object ValueFromString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return (ushort)0;
		}
		return ushort.Parse(value, CultureInfo.InvariantCulture);
	}

	public override bool CanBeConvertedFrom(BGCalcTypeCode otherCode)
	{
		if (otherCode == null)
		{
			return false;
		}
		byte typeCode = otherCode.TypeCode;
		if ((uint)(typeCode - 4) <= 1u || (uint)(typeCode - 17) <= 2u)
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
			5 => (ushort)(float)value, 
			17 => (ushort)(byte)value, 
			4 => (ushort)(int)value, 
			19 => (ushort)(sbyte)value, 
			18 => (ushort)(short)value, 
			_ => value, 
		};
	}
}
