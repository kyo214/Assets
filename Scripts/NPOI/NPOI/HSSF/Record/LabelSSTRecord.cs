using System;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

[Serializable]
public class LabelSSTRecord : CellRecord, ICloneable
{
	public const short sid = 253;

	private int field_4_sst_index;

	protected override string RecordName => "LABELSST";

	public int SSTIndex
	{
		get
		{
			return field_4_sst_index;
		}
		set
		{
			field_4_sst_index = value;
		}
	}

	protected override int ValueDataSize => 4;

	public override short Sid => 253;

	public LabelSSTRecord()
	{
	}

	public LabelSSTRecord(RecordInputStream in1)
		: base(in1)
	{
		field_4_sst_index = in1.ReadInt();
	}

	protected override void AppendValueText(StringBuilder sb)
	{
		sb.Append("  .sstIndex = ");
		sb.Append(HexDump.ShortToHex(SSTIndex));
	}

	protected override void SerializeValue(ILittleEndianOutput out1)
	{
		out1.WriteInt(SSTIndex);
	}

	public override object Clone()
	{
		LabelSSTRecord labelSSTRecord = new LabelSSTRecord();
		CopyBaseFields(labelSSTRecord);
		labelSSTRecord.field_4_sst_index = field_4_sst_index;
		return labelSSTRecord;
	}
}
