using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class ShapePropsStreamRecord : RowDataRecord
{
	public const short sid = 2212;

	protected override int DataSize => base.DataSize;

	public override short Sid => 2212;

	public ShapePropsStreamRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
