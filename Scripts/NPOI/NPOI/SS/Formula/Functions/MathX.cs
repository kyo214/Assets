using System;

namespace NPOI.SS.Formula.Functions;

public class MathX
{
	private MathX()
	{
	}

	public static double Round(double n, int p)
	{
		if (double.IsNaN(n) || double.IsInfinity(n))
		{
			return double.NaN;
		}
		if (double.MaxValue == n)
		{
			return double.MaxValue;
		}
		if (double.MinValue == n)
		{
			return 0.0;
		}
		if (p >= 0)
		{
			return (double)Math.Round((decimal)n, p, MidpointRounding.AwayFromZero);
		}
		int num = (int)Math.Pow(10.0, Math.Abs(p));
		return (double)(Math.Round((decimal)n / (decimal)num, MidpointRounding.AwayFromZero) * (decimal)num);
	}

	public static double RoundUp(double n, int p)
	{
		if (double.IsNaN(n) || double.IsInfinity(n))
		{
			return double.NaN;
		}
		if (double.MaxValue == n)
		{
			return double.MaxValue;
		}
		if (double.MinValue == n)
		{
			double num = 1.0;
			while (p > 0)
			{
				num /= 10.0;
				p--;
			}
			return num;
		}
		if (p != 0)
		{
			double num2 = Math.Pow(10.0, p);
			double num3 = (double)(decimal)Math.Abs(n * num2);
			return (double)Sign(n) * ((num3 == (double)(long)num3) ? (num3 / num2) : (Math.Round(num3 + 0.5) / num2));
		}
		double num4 = Math.Abs(n);
		return (double)Sign(n) * ((num4 == (double)(long)num4) ? num4 : ((double)((long)num4 + 1)));
	}

	public static double RoundDown(double n, int p)
	{
		if (double.IsNaN(n) || double.IsInfinity(n))
		{
			return double.NaN;
		}
		if (double.MaxValue == n)
		{
			return double.MaxValue;
		}
		if (double.MinValue == n)
		{
			return 0.0;
		}
		if (p != 0)
		{
			double num = Math.Pow(10.0, p);
			return (double)Sign(n) * Math.Round(Math.Abs(n) * num - 0.5, MidpointRounding.AwayFromZero) / num;
		}
		return (long)n;
	}

	public static short Sign(double d)
	{
		return (short)((d != 0.0) ? ((!(d < 0.0)) ? 1 : (-1)) : 0);
	}

	public static double Average(double[] values)
	{
		double num = 0.0;
		int i = 0;
		for (int num2 = values.Length; i < num2; i++)
		{
			num += values[i];
		}
		return num / (double)values.Length;
	}

	public static double Sum(double[] values)
	{
		double num = 0.0;
		int i = 0;
		for (int num2 = values.Length; i < num2; i++)
		{
			num += values[i];
		}
		return num;
	}

	public static double Sumsq(double[] values)
	{
		double num = 0.0;
		int i = 0;
		for (int num2 = values.Length; i < num2; i++)
		{
			num += values[i] * values[i];
		}
		return num;
	}

	public static double Product(double[] values)
	{
		double num = 0.0;
		if (values != null && values.Length != 0)
		{
			num = 1.0;
			int i = 0;
			for (int num2 = values.Length; i < num2; i++)
			{
				num *= values[i];
			}
		}
		return num;
	}

	public static double Min(double[] values)
	{
		double num = double.PositiveInfinity;
		int i = 0;
		for (int num2 = values.Length; i < num2; i++)
		{
			num = Math.Min(num, values[i]);
		}
		return num;
	}

	public static double Max(double[] values)
	{
		double num = double.NegativeInfinity;
		int i = 0;
		for (int num2 = values.Length; i < num2; i++)
		{
			num = Math.Max(num, values[i]);
		}
		return num;
	}

	public static double Floor(double n, double s)
	{
		if ((n < 0.0 && s > 0.0) || (n > 0.0 && s < 0.0) || (s == 0.0 && n != 0.0))
		{
			return double.NaN;
		}
		return (n == 0.0 || s == 0.0) ? 0.0 : (Math.Floor(n / s) * s);
	}

	public static double Ceiling(double n, double s)
	{
		if (n > 0.0 && s < 0.0)
		{
			return double.NaN;
		}
		return (n == 0.0 || s == 0.0) ? 0.0 : (Math.Ceiling(n / s) * s);
	}

	public static double Factorial(int n)
	{
		double num = 1.0;
		if (n >= 0)
		{
			if (n <= 170)
			{
				for (int i = 1; i <= n; i++)
				{
					num *= (double)i;
				}
			}
			else
			{
				num = double.PositiveInfinity;
			}
		}
		else
		{
			num = double.NaN;
		}
		return num;
	}

	public static double Mod(double n, double d)
	{
		double num = 0.0;
		if (d == 0.0)
		{
			return double.NaN;
		}
		if (Sign(n) == Sign(d))
		{
			return n % d;
		}
		return (n % d + d) % d;
	}

	public static double Acosh(double d)
	{
		return Math.Log(Math.Sqrt(Math.Pow(d, 2.0) - 1.0) + d);
	}

	public static double Asinh(double d)
	{
		return Math.Log(Math.Sqrt(d * d + 1.0) + d);
	}

	public static double Atanh(double d)
	{
		return Math.Log((1.0 + d) / (1.0 - d)) / 2.0;
	}

	public static double Cosh(double d)
	{
		double num = Math.Pow(Math.E, d);
		double num2 = Math.Pow(Math.E, 0.0 - d);
		d = (num + num2) / 2.0;
		return d;
	}

	public static double Sinh(double d)
	{
		double num = Math.Pow(Math.E, d);
		double num2 = Math.Pow(Math.E, 0.0 - d);
		d = (num - num2) / 2.0;
		return d;
	}

	public static double Tanh(double d)
	{
		double num = Math.Pow(Math.E, d);
		double num2 = Math.Pow(Math.E, 0.0 - d);
		d = (num - num2) / (num + num2);
		return d;
	}

	public static double SumProduct(double[][] arrays)
	{
		double num = 0.0;
		try
		{
			int num2 = arrays.Length;
			int num3 = arrays[0].Length;
			for (int i = 0; i < num3; i++)
			{
				double num4 = 1.0;
				for (int j = 0; j < num2; j++)
				{
					num4 *= arrays[j][i];
				}
				num += num4;
			}
		}
		catch (IndexOutOfRangeException)
		{
			num = double.NaN;
		}
		return num;
	}

	public static double Sumx2my2(double[] xarr, double[] yarr)
	{
		double num = 0.0;
		try
		{
			int i = 0;
			for (int num2 = xarr.Length; i < num2; i++)
			{
				num += (xarr[i] + yarr[i]) * (xarr[i] - yarr[i]);
			}
		}
		catch (IndexOutOfRangeException)
		{
			num = double.NaN;
		}
		return num;
	}

	public static double Sumx2py2(double[] xarr, double[] yarr)
	{
		double num = 0.0;
		try
		{
			int i = 0;
			for (int num2 = xarr.Length; i < num2; i++)
			{
				num += xarr[i] * xarr[i] + yarr[i] * yarr[i];
			}
		}
		catch (IndexOutOfRangeException)
		{
			num = double.NaN;
		}
		return num;
	}

	public static double Sumxmy2(double[] xarr, double[] yarr)
	{
		double num = 0.0;
		try
		{
			int i = 0;
			for (int num2 = xarr.Length; i < num2; i++)
			{
				double num3 = xarr[i] - yarr[i];
				num += num3 * num3;
			}
		}
		catch (IndexOutOfRangeException)
		{
			num = double.NaN;
		}
		return num;
	}

	public static double NChooseK(int n, int k)
	{
		double num = 1.0;
		if (n < 0 || k < 0 || n < k)
		{
			return double.NaN;
		}
		int n2 = Math.Min(n - k, k);
		for (int i = Math.Max(n - k, k); i < n; i++)
		{
			num *= (double)(i + 1);
		}
		return num / Factorial(n2);
	}
}
