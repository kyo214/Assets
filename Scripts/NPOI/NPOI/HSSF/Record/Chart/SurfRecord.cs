using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class SurfRecord : RowDataRecord
{
	public const short sid = 4159;

	protected override int DataSize => base.DataSize;

	public override short Sid => 4159;

	public SurfRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
