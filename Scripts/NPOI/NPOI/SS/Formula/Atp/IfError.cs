using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;

namespace NPOI.SS.Formula.Atp;

internal class IfError : FreeRefFunction
{
	public static FreeRefFunction Instance = new IfError();

	private IfError()
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
			return EvaluateInternal(args[0], args[1], ec.RowIndex, ec.ColumnIndex);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
	}

	private static ValueEval EvaluateInternal(ValueEval arg, ValueEval iferror, int srcCellRow, int srcCellCol)
	{
		arg = WorkbookEvaluator.DereferenceResult(arg, srcCellRow, srcCellCol);
		if (arg is ErrorEval)
		{
			if (iferror is MissingArgEval)
			{
				return new StringEval(string.Empty);
			}
			return iferror;
		}
		return arg;
	}
}
