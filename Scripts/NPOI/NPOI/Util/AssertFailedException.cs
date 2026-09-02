using System;

namespace NPOI.Util;

internal class AssertFailedException : ApplicationException
{
	public AssertFailedException(string message)
		: base(message)
	{
	}
}
