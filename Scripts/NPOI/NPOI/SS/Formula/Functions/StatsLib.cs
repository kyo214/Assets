using System;
using NPOI.Util;

namespace NPOI.SS.Formula.Functions;

public class StatsLib
{
	private StatsLib()
	{
	}

	public static double avedev(double[] v)
	{
		double num = 0.0;
		double num2 = 0.0;
		int i = 0;
		for (int num3 = v.Length; i < num3; i++)
		{
			num2 += v[i];
		}
		num = num2 / (double)v.Length;
		num2 = 0.0;
		int j = 0;
		for (int num4 = v.Length; j < num4; j++)
		{
			num2 += Math.Abs(v[j] - num);
		}
		return num2 / (double)v.Length;
	}

	public static double stdev(double[] v)
	{
		double result = double.NaN;
		if (v != null && v.Length > 1)
		{
			result = Math.Sqrt(devsq(v) / (double)(v.Length - 1));
		}
		return result;
	}

	public static double var(double[] v)
	{
		double result = double.NaN;
		if (v != null && v.Length > 1)
		{
			result = devsq(v) / (double)(v.Length - 1);
		}
		return result;
	}

	public static double varp(double[] v)
	{
		double result = double.NaN;
		if (v != null && v.Length > 1)
		{
			result = devsq(v) / (double)v.Length;
		}
		return result;
	}

	public static double mode(double[] v)
	{
		double result = double.NaN;
		if (v != null && v.Length > 1)
		{
			int[] array = new int[v.Length];
			Arrays.Fill(array, 1);
			int i = 0;
			for (int num = v.Length; i < num; i++)
			{
				int j = i + 1;
				for (int num2 = v.Length; j < num2; j++)
				{
					if (v[i] == v[j])
					{
						array[i]++;
					}
				}
			}
			double num3 = 0.0;
			int num4 = 0;
			int k = 0;
			for (int num5 = array.Length; k < num5; k++)
			{
				if (array[k] > num4)
				{
					num3 = v[k];
					num4 = array[k];
				}
			}
			result = ((num4 > 1) ? num3 : double.NaN);
		}
		return result;
	}

	public static double median(double[] v)
	{
		double result = double.NaN;
		if (v != null && v.Length >= 1)
		{
			int num = v.Length;
			Array.Sort(v);
			result = ((num % 2 == 0) ? ((v[num / 2] + v[num / 2 - 1]) / 2.0) : v[num / 2]);
		}
		return result;
	}

	public static double devsq(double[] v)
	{
		double result = double.NaN;
		if (v != null && v.Length >= 1)
		{
			double num = 0.0;
			double num2 = 0.0;
			int num3 = v.Length;
			for (int i = 0; i < num3; i++)
			{
				num2 += v[i];
			}
			num = num2 / (double)num3;
			num2 = 0.0;
			for (int j = 0; j < num3; j++)
			{
				num2 += (v[j] - num) * (v[j] - num);
			}
			result = ((num3 == 1) ? 0.0 : num2);
		}
		return result;
	}

	public static double kthLargest(double[] v, int k)
	{
		double result = double.NaN;
		k--;
		if (v != null && v.Length > k && k >= 0)
		{
			Array.Sort(v);
			result = v[v.Length - k - 1];
		}
		return result;
	}

	public static double kthSmallest(double[] v, int k)
	{
		double result = double.NaN;
		k--;
		if (v != null && v.Length > k && k >= 0)
		{
			Array.Sort(v);
			result = v[k];
		}
		return result;
	}
}
