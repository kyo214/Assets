using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class RichTextStreamRecord : RowDataRecord
{
	public const short sid = 2214;

	protected override int DataSize => base.DataSize;

	public override short Sid => 2214;

	public RichTextStreamRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
