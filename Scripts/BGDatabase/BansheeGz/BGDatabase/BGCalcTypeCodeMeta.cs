namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeMeta : BGCalcTypeCode<BGMetaEntity>
{
	public const byte Code = 12;

	public override bool SupportDefaultValue => true;

	public override byte TypeCode => 12;

	public override object DefaultValue => null;

	public override string Name => "meta";

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		writer.AddId(((BGMetaEntity)value)?.Id ?? BGId.Empty);
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		BGId id = reader.ReadId();
		if (!id.IsEmpty)
		{
			return BGRepo.I.GetMeta(id);
		}
		return null;
	}

	public override string ValueToString(object value)
	{
		return ((BGMetaEntity)value)?.Id.ToString();
	}

	public override object ValueFromString(string value)
	{
		if (BGId.TryParse(value, out var id))
		{
			return BGRepo.I.GetMeta(id);
		}
		return null;
	}
}
