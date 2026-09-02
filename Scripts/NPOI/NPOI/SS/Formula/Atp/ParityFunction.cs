using System;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;

namespace NPOI.SS.Formula.Atp;

public class ParityFunction : FreeRefFunction
{
	public static readonly FreeRefFunction IS_EVEN = new ParityFunction(0);

	public static readonly FreeRefFunction IS_ODD = new ParityFunction(1);

	private int _desiredParity;

	private ParityFunction(int desiredParity)
	{
		_desiredParity = desiredParity;
	}

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		if (args.Length != 1)
		{
			return ErrorEval.VALUE_INVALID;
		}
		int num;
		try
		{
			num = EvaluateArgParity(args[0], ec.RowIndex, ec.ColumnIndex);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		return BoolEval.ValueOf(num == _desiredParity);
	}

	private static int EvaluateArgParity(ValueEval arg, int srcCellRow, int srcCellCol)
	{
		double num = OperandResolver.CoerceValueToDouble(OperandResolver.GetSingleValue(arg, srcCellRow, srcCellCol));
		if (num < 0.0)
		{
			num = 0.0 - num;
		}
		return (int)((long)Math.Floor(num) & 1);
	}
}
