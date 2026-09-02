using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class FrtFontListRecord : RowDataRecord
{
	public const short sid = 2138;

	protected override int DataSize => base.DataSize;

	public override short Sid => 2138;

	public FrtFontListRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
