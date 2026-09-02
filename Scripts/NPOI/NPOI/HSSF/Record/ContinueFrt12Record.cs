using NPOI.Util;

namespace NPOI.HSSF.Record;

public class ContinueFrt12Record : RowDataRecord
{
	public const short sid = 2175;

	protected override int DataSize => base.DataSize;

	public override short Sid => 2175;

	public ContinueFrt12Record(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
