using System;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Oct2Dec : Fixed1ArgFunction, FreeRefFunction
{
	public static FreeRefFunction instance = new Oct2Dec();

	private static int MAX_NUMBER_OF_PLACES = 10;

	private static int OCTAL_BASE = 8;

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval numberVE)
	{
		string value = OperandResolver.CoerceValueToString(numberVE);
		try
		{
			return new NumberEval(BaseNumberUtils.ConvertToDecimal(value, OCTAL_BASE, MAX_NUMBER_OF_PLACES));
		}
		catch (ArgumentException)
		{
			return ErrorEval.NUM_ERROR;
		}
	}

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		if (args.Length != 1)
		{
			return ErrorEval.VALUE_INVALID;
		}
		return Evaluate(ec.RowIndex, ec.ColumnIndex, args[0]);
	}
}
