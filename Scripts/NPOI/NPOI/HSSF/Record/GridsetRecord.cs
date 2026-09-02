using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class GridsetRecord : StandardRecord, ICloneable
{
	public const short sid = 130;

	public short field_1_gridset_flag;

	public bool Gridset
	{
		get
		{
			return field_1_gridset_flag == 1;
		}
		set
		{
			if (value)
			{
				field_1_gridset_flag = 1;
			}
			else
			{
				field_1_gridset_flag = 0;
			}
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 130;

	public GridsetRecord()
	{
	}

	public GridsetRecord(RecordInputStream in1)
	{
		field_1_gridset_flag = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[GRIDSET]\n");
		stringBuilder.Append("    .gridset        = ").Append(Gridset).Append("\n");
		stringBuilder.Append("[/GRIDSET]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_gridset_flag);
	}

	public override object Clone()
	{
		return new GridsetRecord
		{
			field_1_gridset_flag = field_1_gridset_flag
		};
	}
}
