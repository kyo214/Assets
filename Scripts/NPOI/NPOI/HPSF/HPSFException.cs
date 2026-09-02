using System;

namespace NPOI.HPSF;

[Serializable]
public class HPSFException : Exception
{
	public Exception Reason => base.InnerException;

	public HPSFException()
	{
	}

	public HPSFException(string msg)
		: base(msg)
	{
	}

	public HPSFException(Exception reason)
		: base("", reason)
	{
	}

	public HPSFException(string msg, Exception reason)
		: base(msg, reason)
	{
	}
}
