using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class DimensionsRecord : StandardRecord, ICloneable
{
	public const short sid = 512;

	private int field_1_first_row;

	private int field_2_last_row;

	private int field_3_first_col;

	private int field_4_last_col;

	private short field_5_zero;

	public int FirstRow
	{
		get
		{
			return field_1_first_row;
		}
		set
		{
			field_1_first_row = value;
		}
	}

	public int LastRow
	{
		get
		{
			return field_2_last_row;
		}
		set
		{
			field_2_last_row = value;
		}
	}

	public int FirstCol
	{
		get
		{
			return field_3_first_col;
		}
		set
		{
			field_3_first_col = value;
		}
	}

	public int LastCol
	{
		get
		{
			return field_4_last_col;
		}
		set
		{
			field_4_last_col = value;
		}
	}

	protected override int DataSize => 14;

	public override short Sid => 512;

	public DimensionsRecord()
	{
	}

	public DimensionsRecord(RecordInputStream in1)
	{
		field_1_first_row = in1.ReadInt();
		field_2_last_row = in1.ReadInt();
		field_3_first_col = in1.ReadShort();
		field_4_last_col = in1.ReadShort();
		field_5_zero = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[DIMENSIONS]\n");
		stringBuilder.Append("    .firstrow       = ").Append(StringUtil.ToHexString(FirstRow)).Append("\n");
		stringBuilder.Append("    .lastrow        = ").Append(StringUtil.ToHexString(LastRow)).Append("\n");
		stringBuilder.Append("    .firstcol       = ").Append(StringUtil.ToHexString(FirstCol)).Append("\n");
		stringBuilder.Append("    .lastcol        = ").Append(StringUtil.ToHexString(LastCol)).Append("\n");
		stringBuilder.Append("    .zero           = ").Append(StringUtil.ToHexString(field_5_zero)).Append("\n");
		stringBuilder.Append("[/DIMENSIONS]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteInt(FirstRow);
		out1.WriteInt(LastRow);
		out1.WriteShort(FirstCol);
		out1.WriteShort(LastCol);
		out1.WriteShort(0);
	}

	public override object Clone()
	{
		return new DimensionsRecord
		{
			field_1_first_row = field_1_first_row,
			field_2_last_row = field_2_last_row,
			field_3_first_col = field_3_first_col,
			field_4_last_col = field_4_last_col,
			field_5_zero = field_5_zero
		};
	}
}
