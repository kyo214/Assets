using System;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;

namespace NPOI.SS.Formula.Atp;

internal class WorkdayFunction : FreeRefFunction
{
	public static FreeRefFunction instance = new WorkdayFunction(ArgumentsEvaluator.instance);

	private ArgumentsEvaluator evaluator;

	private WorkdayFunction(ArgumentsEvaluator anEvaluator)
	{
		evaluator = anEvaluator;
	}

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		if (args.Length < 2 || args.Length > 3)
		{
			return ErrorEval.VALUE_INVALID;
		}
		int rowIndex = ec.RowIndex;
		int columnIndex = ec.ColumnIndex;
		try
		{
			double start = evaluator.EvaluateDateArg(args[0], rowIndex, columnIndex);
			int workdays = (int)Math.Floor(evaluator.EvaluateNumberArg(args[1], rowIndex, columnIndex));
			ValueEval arg = ((args.Length == 3) ? args[2] : null);
			double[] holidays = evaluator.EvaluateDatesArg(arg, rowIndex, columnIndex);
			return new NumberEval(DateUtil.GetExcelDate(WorkdayCalculator.instance.CalculateWorkdays(start, workdays, holidays)));
		}
		catch (EvaluationException)
		{
			return ErrorEval.VALUE_INVALID;
		}
	}
}
