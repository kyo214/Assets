using System;

namespace NPOI.HSSF.Util;

public class RKUtil
{
	public static double DecodeNumber(int number)
	{
		long num = number;
		num >>= 2;
		double num2 = 0.0;
		num2 = (((number & 2) != 2) ? BitConverter.Int64BitsToDouble(num << 34) : ((double)num));
		if ((number & 1) == 1)
		{
			num2 /= 100.0;
		}
		return num2;
	}
}
