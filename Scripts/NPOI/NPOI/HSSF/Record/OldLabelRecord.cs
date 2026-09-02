using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class OldLabelRecord : OldCellRecord
{
	private static POILogger logger = POILogFactory.GetLogger(typeof(OldLabelRecord));

	public const short biff2_sid = 4;

	public const short biff345_sid = 516;

	private short field_4_string_len;

	private byte[] field_5_bytes;

	private CodepageRecord codepage;

	public short StringLength => field_4_string_len;

	public string Value => OldStringRecord.GetString(field_5_bytes, codepage);

	public int RecordSize
	{
		get
		{
			throw new RecordFormatException("Old Label Records are supported READ ONLY");
		}
	}

	protected override string RecordName => "OLD LABEL";

	public OldLabelRecord(RecordInputStream in1)
		: base(in1, in1.Sid == 4)
	{
		if (IsBiff2)
		{
			field_4_string_len = (short)in1.ReadUByte();
		}
		else
		{
			field_4_string_len = in1.ReadShort();
		}
		field_5_bytes = new byte[field_4_string_len];
		in1.Read(field_5_bytes, 0, field_4_string_len);
		if (in1.Remaining > 0)
		{
			logger.Log(3, "LabelRecord data remains: " + in1.Remaining + " : " + HexDump.ToHex(in1.ReadRemainder()));
		}
	}

	public void SetCodePage(CodepageRecord codepage)
	{
		this.codepage = codepage;
	}

	public int Serialize(int offset, byte[] data)
	{
		throw new RecordFormatException("Old Label Records are supported READ ONLY");
	}

	protected override void AppendValueText(StringBuilder sb)
	{
		sb.Append("    .string_len= ").Append(HexDump.ShortToHex(field_4_string_len)).Append("\n");
		sb.Append("    .value       = ").Append(Value).Append("\n");
	}
}
