using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class GutsRecord : StandardRecord, ICloneable
{
	public const short sid = 128;

	private short field_1_left_row_gutter;

	private short field_2_top_col_gutter;

	private short field_3_row_level_max;

	private short field_4_col_level_max;

	public short LeftRowGutter
	{
		get
		{
			return field_1_left_row_gutter;
		}
		set
		{
			field_1_left_row_gutter = value;
		}
	}

	public short TopColGutter
	{
		get
		{
			return field_2_top_col_gutter;
		}
		set
		{
			field_2_top_col_gutter = value;
		}
	}

	public short RowLevelMax
	{
		get
		{
			return field_3_row_level_max;
		}
		set
		{
			field_3_row_level_max = value;
		}
	}

	public short ColLevelMax
	{
		get
		{
			return field_4_col_level_max;
		}
		set
		{
			field_4_col_level_max = value;
		}
	}

	protected override int DataSize => 8;

	public override short Sid => 128;

	public GutsRecord()
	{
	}

	public GutsRecord(RecordInputStream in1)
	{
		field_1_left_row_gutter = in1.ReadShort();
		field_2_top_col_gutter = in1.ReadShort();
		field_3_row_level_max = in1.ReadShort();
		field_4_col_level_max = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[GUTS]\n");
		stringBuilder.Append("    .leftgutter     = ").Append(StringUtil.ToHexString(LeftRowGutter)).Append("\n");
		stringBuilder.Append("    .topgutter      = ").Append(StringUtil.ToHexString(TopColGutter)).Append("\n");
		stringBuilder.Append("    .rowlevelmax    = ").Append(StringUtil.ToHexString(RowLevelMax)).Append("\n");
		stringBuilder.Append("    .collevelmax    = ").Append(StringUtil.ToHexString(ColLevelMax)).Append("\n");
		stringBuilder.Append("[/GUTS]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(LeftRowGutter);
		out1.WriteShort(TopColGutter);
		out1.WriteShort(RowLevelMax);
		out1.WriteShort(ColLevelMax);
	}

	public override object Clone()
	{
		return new GutsRecord
		{
			field_1_left_row_gutter = field_1_left_row_gutter,
			field_2_top_col_gutter = field_2_top_col_gutter,
			field_3_row_level_max = field_3_row_level_max,
			field_4_col_level_max = field_4_col_level_max
		};
	}
}
