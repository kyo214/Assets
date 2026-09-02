using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class DefaultColWidthRecord : StandardRecord, ICloneable
{
	public const short sid = 85;

	private int field_1_col_width;

	public const int DEFAULT_COLUMN_WIDTH = 8;

	internal int offsetForFilePointer;

	public int ColWidth
	{
		get
		{
			return field_1_col_width;
		}
		set
		{
			field_1_col_width = value;
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 85;

	public DefaultColWidthRecord()
	{
		field_1_col_width = 8;
	}

	public DefaultColWidthRecord(RecordInputStream in1)
	{
		field_1_col_width = in1.ReadUShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[DEFAULTCOLWIDTH]\n");
		stringBuilder.Append("    .colwidth      = ").Append(StringUtil.ToHexString(ColWidth)).Append("\n");
		stringBuilder.Append("[/DEFAULTCOLWIDTH]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(ColWidth);
	}

	public override object Clone()
	{
		return new DefaultColWidthRecord
		{
			field_1_col_width = field_1_col_width
		};
	}
}
