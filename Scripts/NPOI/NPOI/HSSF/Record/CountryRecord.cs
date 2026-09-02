using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class CountryRecord : StandardRecord
{
	public const short sid = 140;

	private short field_1_default_country;

	private short field_2_current_country;

	public short DefaultCountry
	{
		get
		{
			return field_1_default_country;
		}
		set
		{
			field_1_default_country = value;
		}
	}

	public short CurrentCountry
	{
		get
		{
			return field_2_current_country;
		}
		set
		{
			field_2_current_country = value;
		}
	}

	protected override int DataSize => 4;

	public override short Sid => 140;

	public CountryRecord()
	{
	}

	public CountryRecord(RecordInputStream in1)
	{
		field_1_default_country = in1.ReadShort();
		field_2_current_country = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[COUNTRY]\n");
		stringBuilder.Append("    .defaultcountry  = ").Append(StringUtil.ToHexString(DefaultCountry)).Append("\n");
		stringBuilder.Append("    .currentcountry  = ").Append(StringUtil.ToHexString(CurrentCountry)).Append("\n");
		stringBuilder.Append("[/COUNTRY]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(DefaultCountry);
		out1.WriteShort(CurrentCountry);
	}
}
