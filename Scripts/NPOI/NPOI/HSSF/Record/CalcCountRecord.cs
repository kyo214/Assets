using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class CalcCountRecord : StandardRecord, ICloneable
{
	public const short sid = 12;

	private short field_1_iterations;

	public short Iterations
	{
		get
		{
			return field_1_iterations;
		}
		set
		{
			field_1_iterations = value;
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 12;

	public CalcCountRecord()
	{
	}

	public CalcCountRecord(RecordInputStream in1)
	{
		field_1_iterations = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[CALCCOUNT]\n");
		stringBuilder.Append("    .iterations     = ").Append(StringUtil.ToHexString(Iterations)).Append("\n");
		stringBuilder.Append("[/CALCCOUNT]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(Iterations);
	}

	public override object Clone()
	{
		return new CalcCountRecord
		{
			field_1_iterations = field_1_iterations
		};
	}
}
