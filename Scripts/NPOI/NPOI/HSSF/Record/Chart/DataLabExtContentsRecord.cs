using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class DataLabExtContentsRecord : RowDataRecord
{
	public const short sid = 2155;

	protected override int DataSize => base.DataSize;

	public override short Sid => 2155;

	public DataLabExtContentsRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
