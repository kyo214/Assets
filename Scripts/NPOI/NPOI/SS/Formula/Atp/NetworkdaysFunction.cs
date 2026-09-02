using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Functions;

namespace NPOI.SS.Formula.Atp;

public class NetworkdaysFunction : FreeRefFunction
{
	public static FreeRefFunction instance = new NetworkdaysFunction(ArgumentsEvaluator.instance);

	private ArgumentsEvaluator evaluator;

	private NetworkdaysFunction(ArgumentsEvaluator anEvaluator)
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
			double num = evaluator.EvaluateDateArg(args[0], rowIndex, columnIndex);
			double num2 = evaluator.EvaluateDateArg(args[1], rowIndex, columnIndex);
			if (num > num2)
			{
				return ErrorEval.NAME_INVALID;
			}
			ValueEval arg = ((args.Length == 3) ? args[2] : null);
			double[] holidays = evaluator.EvaluateDatesArg(arg, rowIndex, columnIndex);
			return new NumberEval(WorkdayCalculator.instance.CalculateWorkdays(num, num2, holidays));
		}
		catch (EvaluationException)
		{
			return ErrorEval.VALUE_INVALID;
		}
	}
}
