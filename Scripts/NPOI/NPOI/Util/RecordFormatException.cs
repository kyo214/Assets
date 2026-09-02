using System;

namespace NPOI.Util;

[Serializable]
public class RecordFormatException : RuntimeException
{
	public RecordFormatException(string exception)
		: base(exception)
	{
	}

	public RecordFormatException(string exception, Exception ex)
		: base(exception, ex)
	{
	}

	public RecordFormatException(Exception ex)
		: base(ex)
	{
	}
}
