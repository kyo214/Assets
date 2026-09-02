using System;
using NPOI.Util;

namespace NPOI.OpenXml4Net.Exceptions;

public class OpenXML4NetRuntimeException : RuntimeException
{
	public OpenXML4NetRuntimeException(string msg)
		: base(msg)
	{
	}

	public OpenXML4NetRuntimeException(string msg, Exception reason)
		: base(msg, reason)
	{
	}
}
