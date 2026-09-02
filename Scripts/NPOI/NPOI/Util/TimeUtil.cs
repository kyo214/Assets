using System;

namespace NPOI.Util;

public static class TimeUtil
{
	public static long CurrentMillis()
	{
		return (DateTime.Now.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks) / 10000;
	}
}
