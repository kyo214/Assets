using System;
using System.Text;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class NumberRecord : CellRecord, ICloneable
{
	public const short sid = 515;

	private double field_4_value;

	protected override string RecordName => "NUMBER";

	protected override int ValueDataSize => 8;

	public double Value
	{
		get
		{
			return field_4_value;
		}
		set
		{
			field_4_value = value;
		}
	}

	public override short Sid => 515;

	public NumberRecord()
	{
	}

	public NumberRecord(RecordInputStream in1)
		: base(in1)
	{
		field_4_value = in1.ReadDouble();
	}

	protected override void AppendValueText(StringBuilder sb)
	{
		sb.Append("  .value= ").Append(NumberToTextConverter.ToText(field_4_value));
	}

	protected override void SerializeValue(ILittleEndianOutput out1)
	{
		out1.WriteDouble(Value);
	}

	public override object Clone()
	{
		NumberRecord numberRecord = new NumberRecord();
		CopyBaseFields(numberRecord);
		numberRecord.field_4_value = field_4_value;
		return numberRecord;
	}
}
