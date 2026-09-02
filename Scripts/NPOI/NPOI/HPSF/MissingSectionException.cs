using System;

namespace NPOI.HPSF;

[Serializable]
public class MissingSectionException : HPSFRuntimeException
{
	public MissingSectionException()
	{
	}

	public MissingSectionException(string msg)
		: base(msg)
	{
	}

	public MissingSectionException(Exception reason)
		: base(reason)
	{
	}

	public MissingSectionException(string msg, Exception reason)
		: base(msg, reason)
	{
	}
}
