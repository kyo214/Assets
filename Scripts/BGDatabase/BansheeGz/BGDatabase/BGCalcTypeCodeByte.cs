using System.Globalization;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeByte : BGCalcTypeCode<byte>
{
	public const byte Code = 17;

	public override bool SupportDefaultValue => true;

	public override byte TypeCode => 17;

	public override object DefaultValue => (byte)0;

	public override string Name => "byte";

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		writer.AddByte((byte)value);
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		return reader.ReadByte();
	}

	public override string ValueToString(object value)
	{
		return value.ToString();
	}

	public override object ValueFromString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return (byte)0;
		}
		return byte.Parse(value, CultureInfo.InvariantCulture);
	}

	public override bool CanBeConvertedFrom(BGCalcTypeCode otherCode)
	{
		if (otherCode == null)
		{
			return false;
		}
		byte typeCode = otherCode.TypeCode;
		if ((uint)(typeCode - 4) <= 1u || (uint)(typeCode - 18) <= 2u)
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
			5 => (byte)(float)value, 
			4 => (byte)(int)value, 
			18 => (byte)(short)value, 
			19 => (byte)(sbyte)value, 
			20 => (byte)(ushort)value, 
			_ => value, 
		};
	}
}
