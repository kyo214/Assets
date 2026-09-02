using System;

namespace NPOI.SS.Formula.Functions;

public class FinanceLib
{
	private FinanceLib()
	{
	}

	public static double fv(double r, double n, double y, double p, bool t)
	{
		double num = 0.0;
		if (r == 0.0)
		{
			return -1.0 * (p + n * y);
		}
		double num2 = r + 1.0;
		return (1.0 - Math.Pow(num2, n)) * (t ? num2 : 1.0) * y / r - p * Math.Pow(num2, n);
	}

	public static double pv(double r, double n, double y, double f, bool t)
	{
		double num = 0.0;
		if (r == 0.0)
		{
			return -1.0 * (n * y + f);
		}
		double num2 = r + 1.0;
		return ((1.0 - Math.Pow(num2, n)) / r * (t ? num2 : 1.0) * y - f) / Math.Pow(num2, n);
	}

	public static double npv(double r, double[] cfs)
	{
		double num = 0.0;
		double num2 = r + 1.0;
		double num3 = num2;
		int i = 0;
		for (int num4 = cfs.Length; i < num4; i++)
		{
			num += cfs[i] / num3;
			num3 *= num2;
		}
		return num;
	}

	public static double pmt(double r, double n, double p, double f, bool t)
	{
		double num = 0.0;
		if (r == 0.0)
		{
			return -1.0 * (f + p) / n;
		}
		double num2 = r + 1.0;
		return (f + p * Math.Pow(num2, n)) * r / ((t ? num2 : 1.0) * (1.0 - Math.Pow(num2, n)));
	}

	public static double nper(double r, double y, double p, double f, bool t)
	{
		double num = 0.0;
		if (r == 0.0)
		{
			return -1.0 * (f + p) / y;
		}
		double num2 = r + 1.0;
		double num3 = (t ? num2 : 1.0) * y / r;
		double num4 = ((num3 - f < 0.0) ? Math.Log(f - num3) : Math.Log(num3 - f));
		double num5 = ((num3 - f < 0.0) ? Math.Log(0.0 - p - num3) : Math.Log(p + num3));
		double num6 = Math.Log(num2);
		return (num4 - num5) / num6;
	}
}
