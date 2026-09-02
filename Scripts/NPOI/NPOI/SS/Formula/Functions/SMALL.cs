using System;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class SMALL : AggregateFunction
{
	protected internal override double Evaluate(double[] ops)
	{
		if (ops.Length < 2)
		{
			throw new EvaluationException(ErrorEval.NUM_ERROR);
		}
		double[] array = new double[ops.Length - 1];
		int k = (int)ops[^1];
		Array.Copy(ops, 0, array, 0, array.Length);
		return StatsLib.kthSmallest(array, k);
	}
}
