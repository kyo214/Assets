using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class DefaultRowHeightRecord : StandardRecord, ICloneable
{
	public const short sid = 549;

	private short field_1_option_flags;

	private short field_2_row_height;

	public const short DEFAULT_ROW_HEIGHT = 255;

	internal short OptionFlags
	{
		get
		{
			return field_1_option_flags;
		}
		set
		{
			field_1_option_flags = value;
		}
	}

	public short RowHeight
	{
		get
		{
			return field_2_row_height;
		}
		set
		{
			field_2_row_height = value;
		}
	}

	protected override int DataSize => 4;

	public override short Sid => 549;

	public DefaultRowHeightRecord()
	{
		field_1_option_flags = 0;
		field_2_row_height = 255;
	}

	public DefaultRowHeightRecord(RecordInputStream in1)
	{
		field_1_option_flags = in1.ReadShort();
		field_2_row_height = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[DEFAULTROWHEIGHT]\n");
		stringBuilder.Append("    .optionflags    = ").Append(StringUtil.ToHexString(OptionFlags)).Append("\n");
		stringBuilder.Append("    .rowheight      = ").Append(StringUtil.ToHexString(RowHeight)).Append("\n");
		stringBuilder.Append("[/DEFAULTROWHEIGHT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(OptionFlags);
		out1.WriteShort(RowHeight);
	}

	public override object Clone()
	{
		return new DefaultRowHeightRecord
		{
			field_1_option_flags = field_1_option_flags,
			field_2_row_height = field_2_row_height
		};
	}
}
