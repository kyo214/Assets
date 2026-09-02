using System;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Irr : Function
{
	public ValueEval Evaluate(ValueEval[] args, int srcRowIndex, int srcColumnIndex)
	{
		if (args.Length == 0 || args.Length > 2)
		{
			return ErrorEval.VALUE_INVALID;
		}
		try
		{
			double[] values = AggregateFunction.ValueCollector.CollectValues(args[0]);
			double guess = ((args.Length != 2) ? 0.1 : NumericFunction.SingleOperandEvaluate(args[1], srcRowIndex, srcColumnIndex));
			double num = irr(values, guess);
			NumericFunction.CheckValue(num);
			return new NumberEval(num);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
	}

	public static double irr(double[] income)
	{
		return irr(income, 0.1);
	}

	public static double irr(double[] values, double guess)
	{
		int num = 20;
		double num2 = 1E-07;
		double num3 = guess;
		for (int i = 0; i < num; i++)
		{
			double num4 = 0.0;
			double num5 = 0.0;
			for (int j = 0; j < values.Length; j++)
			{
				num4 += values[j] / Math.Pow(1.0 + num3, j);
				num5 += (double)(-j) * values[j] / Math.Pow(1.0 + num3, j + 1);
			}
			double num6 = num3 - num4 / num5;
			if (Math.Abs(num6 - num3) <= num2)
			{
				return num6;
			}
			num3 = num6;
		}
		return double.NaN;
	}
}
