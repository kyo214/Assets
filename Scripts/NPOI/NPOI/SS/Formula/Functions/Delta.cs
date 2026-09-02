using NPOI.SS.Formula.Eval;
using NPOI.SS.Util;

namespace NPOI.SS.Formula.Functions;

public class Delta : Fixed2ArgFunction, FreeRefFunction
{
	public static FreeRefFunction instance = new Delta();

	private static NumberEval ONE = new NumberEval(1.0);

	private static NumberEval ZERO = new NumberEval(0.0);

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg1, ValueEval arg2)
	{
		ValueEval singleValue;
		try
		{
			singleValue = OperandResolver.GetSingleValue(arg1, srcRowIndex, srcColumnIndex);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		double num = OperandResolver.ParseDouble(OperandResolver.CoerceValueToString(singleValue));
		if (double.IsNaN(num))
		{
			return ErrorEval.VALUE_INVALID;
		}
		ValueEval singleValue2;
		try
		{
			singleValue2 = OperandResolver.GetSingleValue(arg2, srcRowIndex, srcColumnIndex);
		}
		catch (EvaluationException ex2)
		{
			return ex2.GetErrorEval();
		}
		double num2 = OperandResolver.ParseDouble(OperandResolver.CoerceValueToString(singleValue2));
		if (double.IsNaN(num2))
		{
			return ErrorEval.VALUE_INVALID;
		}
		if (NumberComparer.Compare(num, num2) != 0)
		{
			return ZERO;
		}
		return ONE;
	}

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		if (args.Length == 2)
		{
			return Evaluate(ec.RowIndex, ec.ColumnIndex, args[0], args[1]);
		}
		return ErrorEval.VALUE_INVALID;
	}
}
