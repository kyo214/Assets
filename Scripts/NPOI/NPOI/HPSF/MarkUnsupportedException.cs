using System;

namespace NPOI.HPSF;

[Serializable]
public class MarkUnsupportedException : HPSFException
{
	public MarkUnsupportedException()
	{
	}

	public MarkUnsupportedException(string msg)
		: base(msg)
	{
	}

	public MarkUnsupportedException(Exception reason)
		: base(reason)
	{
	}

	public MarkUnsupportedException(string msg, Exception reason)
		: base(msg, reason)
	{
	}
}
