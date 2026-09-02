using System;

namespace NPOI.XSSF.Streaming;

public class RowFlushedException : Exception
{
	public RowFlushedException(int rowNum)
		: base("Row " + rowNum + " has been flushed, cannot evaluate all cells")
	{
	}
}
