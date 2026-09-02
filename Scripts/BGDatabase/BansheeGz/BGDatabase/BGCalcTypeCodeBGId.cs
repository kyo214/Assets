namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeBGId : BGCalcTypeCode<BGId>
{
	public const byte Code = 6;

	public override bool SupportDefaultValue => true;

	public override byte TypeCode => 6;

	public override object DefaultValue => BGId.Empty;

	public override string Name => "BGId";

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		writer.AddId((BGId)value);
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		return reader.ReadId();
	}

	public override string ValueToString(object value)
	{
		return ((BGId)value/*cast due to constrained. prefix*/).ToString();
	}

	public override object ValueFromString(string value)
	{
		BGId id;
		return (!BGId.TryParse(value, out id)) ? BGId.Empty : id;
	}
}
