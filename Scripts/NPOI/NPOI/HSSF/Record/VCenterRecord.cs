using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class VCenterRecord : StandardRecord
{
	public const short sid = 132;

	private short field_1_vcenter;

	public bool VCenter
	{
		get
		{
			return field_1_vcenter == 1;
		}
		set
		{
			if (value)
			{
				field_1_vcenter = 1;
			}
			else
			{
				field_1_vcenter = 0;
			}
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 132;

	public VCenterRecord()
	{
	}

	public VCenterRecord(RecordInputStream in1)
	{
		field_1_vcenter = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[VCENTER]\n");
		stringBuilder.Append("    .vcenter        = ").Append(VCenter).Append("\n");
		stringBuilder.Append("[/VCENTER]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_vcenter);
	}

	public override object Clone()
	{
		return new VCenterRecord
		{
			field_1_vcenter = field_1_vcenter
		};
	}
}
