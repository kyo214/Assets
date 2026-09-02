using System;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Mirr : MultiOperandNumericFunction
{
	protected override int MaxNumOperands => 3;

	public Mirr()
		: base(isReferenceBoolCounted: false, isBlankCounted: false)
	{
	}

	protected internal override double Evaluate(double[] values)
	{
		double financeRate = values[^1];
		double reinvestRate = values[^2];
		double[] array = new double[values.Length - 2];
		Array.Copy(values, 0, array, 0, array.Length);
		bool flag = true;
		double[] array2 = array;
		foreach (double num in array2)
		{
			flag &= num < 0.0;
		}
		if (flag)
		{
			return -1.0;
		}
		bool flag2 = true;
		array2 = array;
		foreach (double num2 in array2)
		{
			flag2 &= num2 > 0.0;
		}
		if (flag2)
		{
			throw new EvaluationException(ErrorEval.DIV_ZERO);
		}
		return mirr(array, financeRate, reinvestRate);
	}

	private static double mirr(double[] in1, double financeRate, double reinvestRate)
	{
		double result = 0.0;
		int num = in1.Length - 1;
		double num2 = 0.0;
		double num3 = 0.0;
		int num4 = 0;
		double[] array = in1;
		foreach (double num5 in array)
		{
			if (num5 < 0.0)
			{
				num2 += num5 / Math.Pow(1.0 + financeRate + reinvestRate, num4++);
			}
		}
		array = in1;
		foreach (double num6 in array)
		{
			if (num6 > 0.0)
			{
				num3 += num6 * Math.Pow(1.0 + financeRate, num - num4++);
			}
		}
		if (num3 != 0.0 && num2 != 0.0)
		{
			result = Math.Pow((0.0 - num3) / num2, 1.0 / (double)num) - 1.0;
		}
		return result;
	}
}
