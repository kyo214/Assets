using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class ScatterRecord : RowDataRecord
{
	public const short sid = 4123;

	protected override int DataSize => base.DataSize;

	public override short Sid => 4123;

	public ScatterRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
