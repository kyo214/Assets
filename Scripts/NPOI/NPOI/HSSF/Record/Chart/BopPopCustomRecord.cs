using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class BopPopCustomRecord : RowDataRecord
{
	public const short sid = 4199;

	protected override int DataSize => base.DataSize;

	public override short Sid => 4199;

	public BopPopCustomRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
