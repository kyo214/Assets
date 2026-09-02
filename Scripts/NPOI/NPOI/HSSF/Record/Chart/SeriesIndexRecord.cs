using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class SeriesIndexRecord : StandardRecord
{
	public const short sid = 4197;

	private short field_1_index;

	protected override int DataSize => 2;

	public override short Sid => 4197;

	public short Index
	{
		get
		{
			return field_1_index;
		}
		set
		{
			field_1_index = value;
		}
	}

	public SeriesIndexRecord()
	{
	}

	public SeriesIndexRecord(RecordInputStream in1)
	{
		field_1_index = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[SINDEX]\n");
		stringBuilder.Append("    .index                = ").Append("0x").Append(HexDump.ToHex(Index))
			.Append(" (")
			.Append(Index)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/SINDEX]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_index);
	}

	public override object Clone()
	{
		return new SeriesIndexRecord
		{
			field_1_index = field_1_index
		};
	}
}
