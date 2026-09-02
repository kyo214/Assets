using System;

namespace NPOI.SS.Formula.Functions;

public class BaseNumberUtils
{
	public static double ConvertToDecimal(string value, int base1, int maxNumberOfPlaces)
	{
		if (string.IsNullOrEmpty(value))
		{
			return 0.0;
		}
		long num = value.Length;
		if (num > maxNumberOfPlaces)
		{
			throw new ArgumentException();
		}
		double num2 = 0.0;
		long num3 = 0L;
		bool flag = true;
		char[] array = value.ToCharArray();
		foreach (char c in array)
		{
			long num4 = (('0' <= c && c <= '9') ? (c - 48) : (('A' <= c && c <= 'Z') ? (10 + (c - 65)) : (('a' > c || c > 'z') ? base1 : (10 + (c - 97)))));
			if (num4 < base1)
			{
				if (flag)
				{
					flag = false;
					num3 = num4;
				}
				num2 = num2 * (double)base1 + (double)num4;
				continue;
			}
			throw new ArgumentException("character not allowed");
		}
		if (!flag && num == maxNumberOfPlaces && num3 >= base1 / 2)
		{
			num2 = GetTwoComplement(base1, maxNumberOfPlaces, num2);
			num2 *= -1.0;
		}
		return num2;
	}

	private static double GetTwoComplement(double base1, double maxNumberOfPlaces, double decimalValue)
	{
		return Math.Pow(base1, maxNumberOfPlaces) - decimalValue;
	}
}
