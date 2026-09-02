using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class YMultRecord : RowDataRecord
{
	public const short sid = 2135;

	protected override int DataSize => base.DataSize;

	public override short Sid => 2135;

	public YMultRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
