using System.Globalization;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeFloat : BGCalcTypeCode<float>
{
	public const byte Code = 5;

	public override bool SupportDefaultValue => true;

	public override byte TypeCode => 5;

	public override object DefaultValue => 0f;

	public override string Name => "float";

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		writer.AddFloat((float)value);
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		return reader.ReadFloat();
	}

	public override string ValueToString(object value)
	{
		return ((float)value).ToString(CultureInfo.InvariantCulture);
	}

	public override object ValueFromString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return 0f;
		}
		return float.Parse(value);
	}

	public override bool CanBeConvertedFrom(BGCalcTypeCode otherCode)
	{
		if (otherCode == null)
		{
			return false;
		}
		byte typeCode = otherCode.TypeCode;
		if (typeCode == 4 || (uint)(typeCode - 17) <= 3u)
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
			4 => (float)(int)value, 
			17 => (float)(int)(byte)value, 
			18 => (float)(short)value, 
			19 => (float)(sbyte)value, 
			20 => (float)(int)(ushort)value, 
			_ => value, 
		};
	}
}
