using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class PlotAreaRecord : StandardRecord
{
	public const short sid = 4149;

	protected override int DataSize => 0;

	public override short Sid => 4149;

	public PlotAreaRecord()
	{
	}

	public PlotAreaRecord(RecordInputStream in1)
	{
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[PLOTAREA]\n");
		stringBuilder.Append("[/PLOTAREA]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
	}

	public override object Clone()
	{
		return new PlotAreaRecord();
	}
}
