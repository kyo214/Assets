using System;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Hex2Dec : Fixed1ArgFunction, FreeRefFunction
{
	public static FreeRefFunction instance = new Hex2Dec();

	private static int HEXADECIMAL_BASE = 16;

	private static int MAX_NUMBER_OF_PLACES = 10;

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval numberVE)
	{
		string value;
		if (numberVE is RefEval)
		{
			RefEval obj = (RefEval)numberVE;
			value = OperandResolver.CoerceValueToString(obj.GetInnerValueEval(obj.FirstSheetIndex));
		}
		else
		{
			value = OperandResolver.CoerceValueToString(numberVE);
		}
		try
		{
			return new NumberEval(BaseNumberUtils.ConvertToDecimal(value, HEXADECIMAL_BASE, MAX_NUMBER_OF_PLACES));
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
