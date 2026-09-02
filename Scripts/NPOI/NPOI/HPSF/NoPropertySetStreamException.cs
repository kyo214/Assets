using System;

namespace NPOI.HPSF;

[Serializable]
public class NoPropertySetStreamException : HPSFException
{
	public NoPropertySetStreamException()
	{
	}

	public NoPropertySetStreamException(string msg)
		: base(msg)
	{
	}

	public NoPropertySetStreamException(Exception reason)
		: base(reason)
	{
	}

	public NoPropertySetStreamException(string msg, Exception reason)
		: base(msg, reason)
	{
	}
}
