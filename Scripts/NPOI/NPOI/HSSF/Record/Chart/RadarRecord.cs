using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class RadarRecord : RowDataRecord
{
	public const short sid = 4158;

	protected override int DataSize => base.DataSize;

	public override short Sid => 4158;

	public RadarRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
