using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Countifs : FreeRefFunction
{
	public static FreeRefFunction instance = new Countifs();

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		double num = double.NaN;
		if (args.Length == 0 || args.Length % 2 > 0)
		{
			return ErrorEval.VALUE_INVALID;
		}
		int num2 = 0;
		while (num2 < args.Length)
		{
			ValueEval valueEval = args[num2];
			ValueEval valueEval2 = args[num2 + 1];
			num2 += 2;
			NumberEval numberEval = (NumberEval)new Countif().Evaluate(new ValueEval[2] { valueEval, valueEval2 }, ec.RowIndex, ec.ColumnIndex);
			if (double.IsNaN(num))
			{
				num = numberEval.NumberValue;
			}
			else if (numberEval.NumberValue < num)
			{
				num = numberEval.NumberValue;
			}
		}
		return new NumberEval(double.IsNaN(num) ? 0.0 : num);
	}
}
