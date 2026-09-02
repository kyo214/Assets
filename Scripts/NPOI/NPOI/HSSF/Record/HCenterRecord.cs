using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class HCenterRecord : StandardRecord, ICloneable
{
	public const short sid = 131;

	private short field_1_hcenter;

	public bool HCenter
	{
		get
		{
			return field_1_hcenter == 1;
		}
		set
		{
			if (value)
			{
				field_1_hcenter = 1;
			}
			else
			{
				field_1_hcenter = 0;
			}
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 131;

	public HCenterRecord()
	{
	}

	public HCenterRecord(RecordInputStream in1)
	{
		field_1_hcenter = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[HCENTER]\n");
		stringBuilder.Append("    .hcenter        = ").Append(HCenter).Append("\n");
		stringBuilder.Append("[/HCENTER]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_hcenter);
	}

	public override object Clone()
	{
		return new HCenterRecord
		{
			field_1_hcenter = field_1_hcenter
		};
	}
}
