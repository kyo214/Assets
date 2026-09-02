using System;

namespace NPOI.HPSF;

[Serializable]
public class UnexpectedPropertySetTypeException : HPSFException
{
	public UnexpectedPropertySetTypeException()
	{
	}

	public UnexpectedPropertySetTypeException(string msg)
		: base(msg)
	{
	}

	public UnexpectedPropertySetTypeException(Exception reason)
		: base(reason)
	{
	}

	public UnexpectedPropertySetTypeException(string msg, Exception reason)
		: base(msg, reason)
	{
	}
}
