using System;
using NPOI.Util;

namespace NPOI.SS.Util;

public class NumberComparer
{
	public static int Compare(double a, double b)
	{
		long num = BitConverter.DoubleToInt64Bits(a);
		long num2 = BitConverter.DoubleToInt64Bits(b);
		int biasedExponent = IEEEDouble.GetBiasedExponent(num);
		int biasedExponent2 = IEEEDouble.GetBiasedExponent(num2);
		if (biasedExponent == 2047)
		{
			throw new ArgumentException("Special double values are not allowed: " + ToHex(a));
		}
		if (biasedExponent2 == 2047)
		{
			throw new ArgumentException("Special double values are not allowed: " + ToHex(a));
		}
		bool flag = num < 0;
		bool flag2 = num2 < 0;
		if (flag != flag2)
		{
			if (!flag)
			{
				return 1;
			}
			return -1;
		}
		int num3 = biasedExponent - biasedExponent2;
		int num4 = Math.Abs(num3);
		if (num4 > 1)
		{
			if (!flag)
			{
				return num3;
			}
			return -num3;
		}
		if (num4 != 1 && num == num2)
		{
			return 0;
		}
		if (biasedExponent == 0)
		{
			if (biasedExponent2 == 0)
			{
				return CompareSubnormalNumbers(num & 0xFFFFFFFFFFFFFL, num2 & 0xFFFFFFFFFFFFFL, flag);
			}
			return -CompareAcrossSubnormalThreshold(num2, num, flag);
		}
		if (biasedExponent2 == 0)
		{
			return CompareAcrossSubnormalThreshold(num, num2, flag);
		}
		ExpandedDouble expandedDouble = ExpandedDouble.FromRawBitsAndExponent(num, biasedExponent - 1023);
		ExpandedDouble expandedDouble2 = ExpandedDouble.FromRawBitsAndExponent(num2, biasedExponent2 - 1023);
		NormalisedDecimal normalisedDecimal = expandedDouble.NormaliseBaseTen().RoundUnits();
		NormalisedDecimal other = expandedDouble2.NormaliseBaseTen().RoundUnits();
		num3 = normalisedDecimal.CompareNormalised(other);
		if (flag)
		{
			return -num3;
		}
		return num3;
	}

	private static int CompareSubnormalNumbers(long fracA, long fracB, bool isNegative)
	{
		int num = ((fracA > fracB) ? 1 : ((fracA < fracB) ? (-1) : 0));
		if (!isNegative)
		{
			return num;
		}
		return -num;
	}

	private static int CompareAcrossSubnormalThreshold(long normalRawBitsA, long subnormalRawBitsB, bool isNegative)
	{
		long num = subnormalRawBitsB & 0xFFFFFFFFFFFFFL;
		if (num == 0L)
		{
			if (!isNegative)
			{
				return 1;
			}
			return -1;
		}
		long num2 = normalRawBitsA & 0xFFFFFFFFFFFFFL;
		if (num2 <= 7 && num >= 4503599627370490L)
		{
			if (num2 == 7 && num == 4503599627370490L)
			{
				return 0;
			}
			if (!isNegative)
			{
				return -1;
			}
			return 1;
		}
		if (!isNegative)
		{
			return 1;
		}
		return -1;
	}

	private static string ToHex(double a)
	{
		return "0x" + StringUtil.ToHexString(BitConverter.DoubleToInt64Bits(a)).ToUpper();
	}
}
