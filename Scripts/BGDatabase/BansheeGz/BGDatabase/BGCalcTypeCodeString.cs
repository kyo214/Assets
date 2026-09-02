namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeString : BGCalcTypeCode<string>
{
	public const byte Code = 3;

	public override bool SupportDefaultValue => true;

	public override byte TypeCode => 3;

	public override object DefaultValue => "";

	public override string Name => "string";

	public override bool AreEqual(object o1, object o2)
	{
		string text = (string)o1;
		string value = (string)o2;
		bool flag = string.IsNullOrEmpty(text);
		bool flag2 = string.IsNullOrEmpty(value);
		if (flag & flag2)
		{
			return true;
		}
		if (flag | flag2)
		{
			return false;
		}
		return text.Equals(value);
	}

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		writer.AddString((string)value);
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		return reader.ReadString() ?? "";
	}

	public override string ValueToString(object value)
	{
		return ((string)value) ?? "";
	}

	public override object ValueFromString(string value)
	{
		return value ?? "";
	}

	public override bool CanBeConvertedFrom(BGCalcTypeCode otherCode)
	{
		if (otherCode == null)
		{
			return false;
		}
		byte typeCode = otherCode.TypeCode;
		if ((uint)(typeCode - 4) <= 1u || (uint)(typeCode - 17) <= 3u)
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
		byte typeCode = otherCode.TypeCode;
		if ((uint)(typeCode - 4) <= 1u || (uint)(typeCode - 17) <= 3u)
		{
			return value.ToString();
		}
		return value;
	}
}
