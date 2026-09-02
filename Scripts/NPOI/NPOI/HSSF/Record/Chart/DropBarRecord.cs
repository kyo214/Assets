using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class DropBarRecord : RowDataRecord
{
	public const short sid = 4157;

	protected override int DataSize => base.DataSize;

	public override short Sid => 4157;

	public DropBarRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
