using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class DVALRecord : StandardRecord, ICloneable
{
	public const short sid = 434;

	private short field_1_options;

	private int field_2_horiz_pos;

	private int field_3_vert_pos;

	private int field_cbo_id;

	private int field_5_dv_no;

	public short Options
	{
		get
		{
			return field_1_options;
		}
		set
		{
			field_1_options = value;
		}
	}

	public int HorizontalPos
	{
		get
		{
			return field_2_horiz_pos;
		}
		set
		{
			field_2_horiz_pos = value;
		}
	}

	public int VerticalPos
	{
		get
		{
			return field_3_vert_pos;
		}
		set
		{
			field_3_vert_pos = value;
		}
	}

	public int ObjectID
	{
		get
		{
			return field_cbo_id;
		}
		set
		{
			field_cbo_id = value;
		}
	}

	public int DVRecNo
	{
		get
		{
			return field_5_dv_no;
		}
		set
		{
			field_5_dv_no = value;
		}
	}

	protected override int DataSize => 18;

	public override short Sid => 434;

	public DVALRecord()
	{
		field_cbo_id = -1;
		field_5_dv_no = 0;
	}

	public DVALRecord(RecordInputStream in1)
	{
		field_1_options = in1.ReadShort();
		field_2_horiz_pos = in1.ReadInt();
		field_3_vert_pos = in1.ReadInt();
		field_cbo_id = in1.ReadInt();
		field_5_dv_no = in1.ReadInt();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[DVAL]\n");
		stringBuilder.Append("    .options      = ").Append(Options).Append('\n');
		stringBuilder.Append("    .horizPos     = ").Append(HorizontalPos).Append('\n');
		stringBuilder.Append("    .vertPos      = ").Append(VerticalPos).Append('\n');
		stringBuilder.Append("    .comboObjectID   = ").Append(StringUtil.ToHexString(ObjectID)).Append("\n");
		stringBuilder.Append("    .DVRecordsNumber = ").Append(StringUtil.ToHexString(DVRecNo)).Append("\n");
		stringBuilder.Append("[/DVAL]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(Options);
		out1.WriteInt(HorizontalPos);
		out1.WriteInt(VerticalPos);
		out1.WriteInt(ObjectID);
		out1.WriteInt(DVRecNo);
	}

	public override object Clone()
	{
		return new DVALRecord
		{
			field_1_options = field_1_options,
			field_2_horiz_pos = field_2_horiz_pos,
			field_3_vert_pos = field_3_vert_pos,
			field_cbo_id = field_cbo_id,
			field_5_dv_no = field_5_dv_no
		};
	}
}
