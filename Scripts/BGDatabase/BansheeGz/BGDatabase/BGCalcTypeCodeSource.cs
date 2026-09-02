using System.Globalization;

namespace BansheeGz.BGDatabase;

public class BGCalcTypeCodeSource : BGCalcTypeCode<BGCalcUnitSourceEnum>
{
	public const byte Code = 9;

	public override bool SupportDefaultValue => true;

	public override byte TypeCode => 9;

	public override object DefaultValue => BGCalcUnitSourceEnum.DB_Object;

	public override string Name => "sourceMode";

	public override void ValueToBytes(BGBinaryWriter writer, object value)
	{
		writer.AddByte((byte)(BGCalcUnitSourceEnum)value);
	}

	public override object ValueFromBytes(BGBinaryReader reader)
	{
		return (BGCalcUnitSourceEnum)reader.ReadByte();
	}

	public override string ValueToString(object value)
	{
		return ((byte)(BGCalcUnitSourceEnum)value).ToString(CultureInfo.InvariantCulture);
	}

	public override object ValueFromString(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return BGCalcUnitSourceEnum.DB_Object;
		}
		return (BGCalcUnitSourceEnum)byte.Parse(value);
	}
}
