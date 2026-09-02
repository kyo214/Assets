using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class DefaultTextRecord : StandardRecord
{
	public const short sid = 4132;

	private short field_1_categoryDataType;

	protected override int DataSize => 2;

	public override short Sid => 4132;

	public TextFormatInfo FormatType
	{
		get
		{
			return (TextFormatInfo)field_1_categoryDataType;
		}
		set
		{
			field_1_categoryDataType = (short)value;
		}
	}

	public DefaultTextRecord()
	{
	}

	public DefaultTextRecord(RecordInputStream in1)
	{
		field_1_categoryDataType = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[DEFAULTTEXT]\n");
		stringBuilder.Append("    .categoryDataType     = ").Append("0x").Append(HexDump.ToHex((short)FormatType))
			.Append(" (")
			.Append(FormatType)
			.Append(" )");
		stringBuilder.Append(Environment.NewLine);
		stringBuilder.Append("[/DEFAULTTEXT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_categoryDataType);
	}

	public override object Clone()
	{
		return new DefaultTextRecord
		{
			field_1_categoryDataType = field_1_categoryDataType
		};
	}
}
