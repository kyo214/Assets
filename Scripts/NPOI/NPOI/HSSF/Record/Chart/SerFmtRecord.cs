using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class SerFmtRecord : RowDataRecord
{
	public const short sid = 4189;

	protected override int DataSize => base.DataSize;

	public override short Sid => 4189;

	public SerFmtRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
