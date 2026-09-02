using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Quotient : Fixed2ArgFunction, FreeRefFunction
{
	public static FreeRefFunction instance = new Quotient();

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval venumerator, ValueEval vedenominator)
	{
		double num = 0.0;
		try
		{
			num = OperandResolver.CoerceValueToDouble(OperandResolver.GetSingleValue(venumerator, srcRowIndex, srcColumnIndex));
		}
		catch (EvaluationException)
		{
			return ErrorEval.VALUE_INVALID;
		}
		double num2 = 0.0;
		try
		{
			num2 = OperandResolver.CoerceValueToDouble(OperandResolver.GetSingleValue(vedenominator, srcRowIndex, srcColumnIndex));
		}
		catch (EvaluationException)
		{
			return ErrorEval.VALUE_INVALID;
		}
		if (num2 == 0.0)
		{
			return ErrorEval.DIV_ZERO;
		}
		return new NumberEval((int)(num / num2));
	}

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		if (args.Length != 2)
		{
			return ErrorEval.VALUE_INVALID;
		}
		return Evaluate(ec.RowIndex, ec.ColumnIndex, args[0], args[1]);
	}
}
