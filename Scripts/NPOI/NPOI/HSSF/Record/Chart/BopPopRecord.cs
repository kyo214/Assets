using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class BopPopRecord : RowDataRecord
{
	public const short sid = 4193;

	protected override int DataSize => base.DataSize;

	public override short Sid => 4193;

	public BopPopRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
