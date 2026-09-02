using System;
using NPOI.Util;

namespace NPOI.HSSF.Record.Chart;

public class LegendExceptionRecord : RowDataRecord
{
	public const short sid = 4163;

	protected override int DataSize => base.DataSize;

	public override short Sid => 4163;

	public short LegendEntry
	{
		get
		{
			throw new NotImplementedException();
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public LegendExceptionRecord(RecordInputStream ris)
		: base(ris)
	{
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		base.Serialize(out1);
	}
}
