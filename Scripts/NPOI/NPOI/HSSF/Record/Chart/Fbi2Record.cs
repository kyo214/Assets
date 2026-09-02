using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class Fbi2Record : RowDataRecord
{
	public const short sid = 4200;

	protected override int DataSize => base.DataSize;

	public override short Sid => 4200;

	public Fbi2Record(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
