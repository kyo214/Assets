using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class CrtMlFrtRecord : RowDataRecord
{
	public const short sid = 2206;

	protected override int DataSize => base.DataSize;

	public override short Sid => 2206;

	public CrtMlFrtRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
