using System.Globalization;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeInt : BGCalcTypeCode<int>
{
	public const byte Code = 4;

	public override bool SupportDefaultValue => true;

	public override byte TypeCode => 4;

	public override object DefaultValue => 0;

	public override string Name => "int";

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		writer.AddInt((int)value);
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		return reader.ReadInt();
	}

	public override string ValueToString(object value)
	{
		return ((int)value).ToString(CultureInfo.InvariantCulture);
	}

	public override object ValueFromString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return 0;
		}
		return int.Parse(value);
	}

	public override bool CanBeConvertedFrom(BGCalcTypeCode otherCode)
	{
		if (otherCode == null)
		{
			return false;
		}
		byte typeCode = otherCode.TypeCode;
		if (typeCode == 5 || (uint)(typeCode - 17) <= 3u)
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
			5 => (int)(float)value, 
			17 => (int)(byte)value, 
			18 => (int)(short)value, 
			19 => (int)(sbyte)value, 
			20 => (int)(ushort)value, 
			_ => value, 
		};
	}
}
