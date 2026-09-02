using System;
using NPOI.Util;

namespace NPOI.SS.Format;

public class SimpleFraction
{
	private int denominator;

	private int numerator;

	public int Denominator => denominator;

	public int Numerator => numerator;

	public static SimpleFraction BuildFractionExactDenominator(double val, int exactDenom)
	{
		return new SimpleFraction((int)Math.Round(val * (double)exactDenom, MidpointRounding.AwayFromZero), exactDenom);
	}

	public static SimpleFraction BuildFractionMaxDenominator(double value, int maxDenominator)
	{
		return BuildFractionMaxDenominator(value, 0.0, maxDenominator, 100);
	}

	private static SimpleFraction BuildFractionMaxDenominator(double value, double epsilon, int maxDenominator, int maxIterations)
	{
		long num = long.MaxValue;
		double num2 = value;
		long num3 = (long)Math.Floor(num2);
		if (num3 > num)
		{
			throw new ArgumentException("Overflow trying to convert " + value + " to fraction (" + num3 + "/" + 1L + ")");
		}
		if (Math.Abs((double)num3 - value) < epsilon)
		{
			return new SimpleFraction((int)num3, 1);
		}
		long num4 = 1L;
		long num5 = 0L;
		long num6 = num3;
		long num7 = 1L;
		int num8 = 0;
		bool flag = false;
		long num11;
		long num12;
		do
		{
			num8++;
			double num9 = 1.0 / (num2 - (double)num3);
			long num10 = (long)Math.Floor(num9);
			num11 = num10 * num6 + num4;
			num12 = num10 * num7 + num5;
			if (epsilon == 0.0 && maxDenominator > 0 && Math.Abs(num12) > maxDenominator && Math.Abs(num7) < maxDenominator)
			{
				return new SimpleFraction((int)num6, (int)num7);
			}
			if (num11 > num || num12 > num)
			{
				throw new RuntimeException("Overflow trying to convert " + value + " to fraction (" + num11 + "/" + num12 + ")");
			}
			double num13 = (double)num11 / (double)num12;
			if (num8 < maxIterations && Math.Abs(num13 - value) > epsilon && num12 < maxDenominator)
			{
				num4 = num6;
				num6 = num11;
				num5 = num7;
				num7 = num12;
				num3 = num10;
				num2 = num9;
			}
			else
			{
				flag = true;
			}
		}
		while (!flag);
		if (num8 >= maxIterations)
		{
			throw new RuntimeException("Unable to convert " + value + " to fraction after " + maxIterations + " iterations");
		}
		if (num12 < maxDenominator)
		{
			return new SimpleFraction((int)num11, (int)num12);
		}
		return new SimpleFraction((int)num6, (int)num7);
	}

	public SimpleFraction(int numerator, int denominator)
	{
		this.numerator = numerator;
		this.denominator = denominator;
	}
}
