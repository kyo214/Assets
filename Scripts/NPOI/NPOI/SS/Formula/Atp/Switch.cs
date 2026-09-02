using System;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;

namespace NPOI.SS.Formula.Atp;

public class Switch : FreeRefFunction
{
	public static FreeRefFunction instance = new Switch();

	private Switch()
	{
	}

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		if (args.Length < 3)
		{
			return ErrorEval.NA;
		}
		ValueEval singleValue;
		try
		{
			singleValue = OperandResolver.GetSingleValue(args[0], ec.RowIndex, ec.ColumnIndex);
		}
		catch (Exception)
		{
			return ErrorEval.NA;
		}
		for (int i = 1; i < args.Length; i += 2)
		{
			try
			{
				ValueEval singleValue2 = OperandResolver.GetSingleValue(args[i], ec.RowIndex, ec.ColumnIndex);
				ValueEval result = args[i + 1];
				ValueEval valueEval = new EqualEval().Evaluate(new ValueEval[2] { singleValue, singleValue2 }, ec.RowIndex, ec.ColumnIndex);
				if (valueEval is BoolEval && ((BoolEval)valueEval).BooleanValue)
				{
					return result;
				}
			}
			catch (EvaluationException)
			{
				return ErrorEval.NA;
			}
			if (i + 2 == args.Length - 1)
			{
				return args[^1];
			}
		}
		return ErrorEval.NA;
	}
}
