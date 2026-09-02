using System;

namespace NPOI.HPSF;

[Serializable]
public class IllegalPropertySetDataException : HPSFRuntimeException
{
	public IllegalPropertySetDataException()
	{
	}

	public IllegalPropertySetDataException(string msg)
		: base(msg)
	{
	}

	public IllegalPropertySetDataException(Exception reason)
		: base(reason)
	{
	}

	public IllegalPropertySetDataException(string msg, Exception reason)
		: base(msg, reason)
	{
	}
}
