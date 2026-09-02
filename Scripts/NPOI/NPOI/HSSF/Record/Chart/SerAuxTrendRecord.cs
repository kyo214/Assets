using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class SerAuxTrendRecord : RowDataRecord
{
	public const short sid = 4171;

	protected override int DataSize => base.DataSize;

	public override short Sid => 4171;

	public SerAuxTrendRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
