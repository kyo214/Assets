using System;

namespace NPOI.HSSF.EventUserModel;

[Serializable]
public class HSSFUserException : Exception
{
	public HSSFUserException()
	{
	}

	public HSSFUserException(string msg)
		: base(msg)
	{
	}

	public HSSFUserException(Exception reason)
	{
	}

	public HSSFUserException(string msg, Exception reason)
		: base(msg, reason)
	{
	}
}
