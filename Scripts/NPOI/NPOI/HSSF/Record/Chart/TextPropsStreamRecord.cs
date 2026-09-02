using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class TextPropsStreamRecord : RowDataRecord
{
	public const short sid = 2213;

	protected override int DataSize => base.DataSize;

	public override short Sid => 2213;

	public TextPropsStreamRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
