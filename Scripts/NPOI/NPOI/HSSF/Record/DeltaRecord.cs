using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class DeltaRecord : StandardRecord, ICloneable
{
	public const short sid = 16;

	public const double DEFAULT_VALUE = 0.001;

	private double field_1_max_change;

	public double MaxChange
	{
		get
		{
			return field_1_max_change;
		}
		set
		{
			field_1_max_change = value;
		}
	}

	protected override int DataSize => 8;

	public override short Sid => 16;

	public DeltaRecord(double maxChange)
	{
		field_1_max_change = maxChange;
	}

	public DeltaRecord(RecordInputStream in1)
	{
		field_1_max_change = in1.ReadDouble();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[DELTA]\n");
		stringBuilder.Append("    .maxChange      = ").Append(MaxChange).Append("\n");
		stringBuilder.Append("[/DELTA]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteDouble(MaxChange);
	}

	public override object Clone()
	{
		return this;
	}
}
