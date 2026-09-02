using System;
using NPOI.Util;

namespace NPOI.HSSF.Record.CF;

public class DataBarThreshold : Threshold, ICloneable
{
	public DataBarThreshold()
	{
	}

	public DataBarThreshold(ILittleEndianInput in1)
		: base(in1)
	{
	}

	public object Clone()
	{
		DataBarThreshold dataBarThreshold = new DataBarThreshold();
		CopyTo(dataBarThreshold);
		return dataBarThreshold;
	}
}
