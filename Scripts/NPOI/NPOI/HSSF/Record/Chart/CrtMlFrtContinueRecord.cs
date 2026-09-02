using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class CrtMlFrtContinueRecord : RowDataRecord
{
	public const short sid = 2207;

	protected override int DataSize => base.DataSize;

	public override short Sid => 2207;

	public CrtMlFrtContinueRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
