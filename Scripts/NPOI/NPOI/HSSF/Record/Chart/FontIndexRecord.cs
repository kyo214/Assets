using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class FontIndexRecord : StandardRecord, ICloneable
{
	public const short sid = 4134;

	private short field_1_fontIndex;

	protected override int DataSize => 2;

	public override short Sid => 4134;

	public short FontIndex
	{
		get
		{
			return field_1_fontIndex;
		}
		set
		{
			field_1_fontIndex = value;
		}
	}

	public FontIndexRecord()
	{
	}

	public FontIndexRecord(RecordInputStream in1)
	{
		field_1_fontIndex = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[FONTX]\n");
		stringBuilder.Append("    .fontIndex            = ").Append("0x").Append(HexDump.ToHex(FontIndex))
			.Append(" (")
			.Append(FontIndex)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/FONTX]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_fontIndex);
	}

	public override object Clone()
	{
		return new FontIndexRecord
		{
			field_1_fontIndex = field_1_fontIndex
		};
	}
}
