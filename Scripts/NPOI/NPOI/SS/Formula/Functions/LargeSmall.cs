using System;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class LargeSmall : Fixed2ArgFunction
{
	private bool _isLarge;

	protected LargeSmall(bool isLarge)
	{
		_isLarge = isLarge;
	}

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
		if (num < 1.0)
		{
			return ErrorEval.NUM_ERROR;
		}
		int num2 = (int)Math.Ceiling(num);
		double num3;
		try
		{
			double[] array = AggregateFunction.ValueCollector.CollectValues(arg0);
			if (num2 > array.Length)
			{
				return ErrorEval.NUM_ERROR;
			}
			num3 = (_isLarge ? StatsLib.kthLargest(array, num2) : StatsLib.kthSmallest(array, num2));
			NumericFunction.CheckValue(num3);
		}
		catch (EvaluationException ex2)
		{
			return ex2.GetErrorEval();
		}
		return new NumberEval(num3);
	}
}
