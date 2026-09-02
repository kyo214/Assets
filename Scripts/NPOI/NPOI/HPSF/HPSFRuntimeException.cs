using System;
using NPOI.Util;

namespace NPOI.HPSF;

[Serializable]
public class HPSFRuntimeException : RuntimeException
{
	public HPSFRuntimeException()
	{
	}

	public HPSFRuntimeException(string msg)
		: base(msg)
	{
	}

	public HPSFRuntimeException(Exception reason)
		: base(reason.Message, reason)
	{
	}

	public HPSFRuntimeException(string msg, Exception reason)
		: base(msg, reason)
	{
	}
}
