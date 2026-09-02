using System;

namespace NPOI.XSSF.Streaming;

public class SheetsFlushedException : Exception
{
	public SheetsFlushedException()
		: base("One or more sheets have been flushed, cannot evaluate all cells")
	{
	}
}
