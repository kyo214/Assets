using System;

namespace NPOI.HPSF;

[Serializable]
public class NoFormatIDException : HPSFRuntimeException
{
	public NoFormatIDException()
	{
	}

	public NoFormatIDException(string msg)
		: base(msg)
	{
	}

	public NoFormatIDException(Exception reason)
		: base(reason)
	{
	}

	public NoFormatIDException(string msg, Exception reason)
		: base(msg, reason)
	{
	}
}
