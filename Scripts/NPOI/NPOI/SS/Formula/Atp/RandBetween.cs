using System;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;

namespace NPOI.SS.Formula.Atp;

internal class RandBetween : FreeRefFunction
{
	private Random _rnd;

	public static FreeRefFunction Instance = new RandBetween();

	private RandBetween()
	{
		_rnd = new Random();
	}

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		if (args.Length != 2)
		{
			return ErrorEval.VALUE_INVALID;
		}
		double num;
		double num2;
		try
		{
			num = OperandResolver.CoerceValueToDouble(OperandResolver.GetSingleValue(args[0], ec.RowIndex, ec.ColumnIndex));
			num2 = OperandResolver.CoerceValueToDouble(OperandResolver.GetSingleValue(args[1], ec.RowIndex, ec.ColumnIndex));
			if (num > num2)
			{
				return ErrorEval.NUM_ERROR;
			}
		}
		catch (EvaluationException)
		{
			return ErrorEval.VALUE_INVALID;
		}
		num = Math.Ceiling(num);
		num2 = Math.Floor(num2);
		if (num > num2)
		{
			num2 = num;
		}
		return new NumberEval(num + (double)(int)(_rnd.NextDouble() * (num2 - num + 1.0)));
	}
}
