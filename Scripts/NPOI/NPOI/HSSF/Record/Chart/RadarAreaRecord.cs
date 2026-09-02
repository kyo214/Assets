using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class RadarAreaRecord : RowDataRecord
{
	public const short sid = 4160;

	protected override int DataSize => base.DataSize;

	public override short Sid => 4160;

	public RadarAreaRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
