using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class PrecisionRecord : StandardRecord
{
	public const short sid = 14;

	public short field_1_precision;

	public bool FullPrecision
	{
		get
		{
			return field_1_precision == 1;
		}
		set
		{
			if (value)
			{
				field_1_precision = 1;
			}
			else
			{
				field_1_precision = 0;
			}
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 14;

	public PrecisionRecord()
	{
	}

	public PrecisionRecord(RecordInputStream in1)
	{
		field_1_precision = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[PRECISION]\n");
		stringBuilder.Append("    .precision       = ").Append(FullPrecision).Append("\n");
		stringBuilder.Append("[/PRECISION]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_precision);
	}
}
