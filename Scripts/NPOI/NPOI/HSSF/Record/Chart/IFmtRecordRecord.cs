using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class IFmtRecordRecord : StandardRecord
{
	public const short sid = 4174;

	private short field_1_formatIndex;

	protected override int DataSize => 2;

	public override short Sid => 4174;

	public short FormatIndex
	{
		get
		{
			return field_1_formatIndex;
		}
		set
		{
			field_1_formatIndex = value;
		}
	}

	public IFmtRecordRecord()
	{
	}

	public IFmtRecordRecord(RecordInputStream in1)
	{
		field_1_formatIndex = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[IFMT]\n");
		stringBuilder.Append("    .formatIndex          = ").Append("0x").Append(HexDump.ToHex(FormatIndex))
			.Append(" (")
			.Append(FormatIndex)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/IFMT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_formatIndex);
	}

	public override object Clone()
	{
		return new IFmtRecordRecord
		{
			field_1_formatIndex = field_1_formatIndex
		};
	}
}
