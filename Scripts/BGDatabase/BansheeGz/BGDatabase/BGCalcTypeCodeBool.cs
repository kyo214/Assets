namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeBool : BGCalcTypeCode<bool>
{
	public const byte Code = 2;

	public override bool SupportDefaultValue => true;

	public override byte TypeCode => 2;

	public override object DefaultValue => false;

	public override string Name => "bool";

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		writer.AddBool((bool)value);
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		return reader.ReadBool();
	}

	public override string ValueToString(object value)
	{
		if (!(bool)value)
		{
			return "0";
		}
		return "1";
	}

	public override object ValueFromString(string value)
	{
		return string.Equals(value, "1");
	}
}
