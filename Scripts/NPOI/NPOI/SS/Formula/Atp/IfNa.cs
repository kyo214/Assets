using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;

namespace NPOI.SS.Formula.Atp;

public class IfNa : FreeRefFunction
{
	public static FreeRefFunction instance = new IfNa();

	private IfNa()
	{
	}

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		if (args.Length != 2)
		{
			return ErrorEval.VALUE_INVALID;
		}
		try
		{
			return OperandResolver.GetSingleValue(args[0], ec.RowIndex, ec.ColumnIndex);
		}
		catch (EvaluationException ex)
		{
			ValueEval errorEval = ex.GetErrorEval();
			if (errorEval != ErrorEval.NA)
			{
				return errorEval;
			}
		}
		try
		{
			return OperandResolver.GetSingleValue(args[1], ec.RowIndex, ec.ColumnIndex);
		}
		catch (EvaluationException ex2)
		{
			return ex2.GetErrorEval();
		}
	}
}
