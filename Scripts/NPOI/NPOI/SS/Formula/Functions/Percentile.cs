using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Percentile : Fixed2ArgFunction
{
	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0, ValueEval arg1)
	{
		double num;
		try
		{
			num = OperandResolver.CoerceValueToDouble(OperandResolver.GetSingleValue(arg1, srcRowIndex, srcColumnIndex));
		}
		catch (EvaluationException)
		{
			return ErrorEval.VALUE_INVALID;
		}
		if (num < 0.0 || num > 1.0)
		{
			return ErrorEval.NUM_ERROR;
		}
		double num4;
		try
		{
			double[] array = AggregateFunction.ValueCollector.CollectValues(arg0);
			int num2 = array.Length;
			if (num2 == 0 || num2 > 8191)
			{
				return ErrorEval.NUM_ERROR;
			}
			double num3 = (double)(num2 - 1) * num + 1.0;
			if (num3 == 1.0)
			{
				num4 = StatsLib.kthSmallest(array, 1);
			}
			else if (num3 == (double)num2)
			{
				num4 = StatsLib.kthLargest(array, 1);
			}
			else
			{
				int num5 = (int)num3;
				double num6 = num3 - (double)num5;
				num4 = StatsLib.kthSmallest(array, num5) + num6 * (StatsLib.kthSmallest(array, num5 + 1) - StatsLib.kthSmallest(array, num5));
			}
			NumericFunction.CheckValue(num4);
		}
		catch (EvaluationException ex2)
		{
			return ex2.GetErrorEval();
		}
		return new NumberEval(num4);
	}
}
