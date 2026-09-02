using System;

namespace NPOI.HPSF;

[Serializable]
public class NoSingleSectionException : HPSFRuntimeException
{
	public NoSingleSectionException()
	{
	}

	public NoSingleSectionException(string msg)
		: base(msg)
	{
	}

	public NoSingleSectionException(Exception reason)
		: base(reason)
	{
	}

	public NoSingleSectionException(string msg, Exception reason)
		: base(msg, reason)
	{
	}
}
