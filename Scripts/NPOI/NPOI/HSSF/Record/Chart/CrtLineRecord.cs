using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class CrtLineRecord : RowDataRecord
{
	public const short sid = 4124;

	protected override int DataSize => base.DataSize;

	public override short Sid => 4124;

	public CrtLineRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
