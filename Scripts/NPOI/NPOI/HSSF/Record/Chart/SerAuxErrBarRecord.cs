using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class SerAuxErrBarRecord : RowDataRecord
{
	public const short sid = 4187;

	protected override int DataSize => base.DataSize;

	public override short Sid => 4187;

	public SerAuxErrBarRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
