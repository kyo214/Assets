using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Dec2Hex : Var1or2ArgFunction, FreeRefFunction
{
	public static FreeRefFunction instance = new Dec2Hex();

	private static long MinValue = -549755813888L;

	private static long MaxValue = 549755813887L;

	private static int DEFAULT_PLACES_VALUE = 10;

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval number, ValueEval places)
	{
		ValueEval singleValue;
		try
		{
			singleValue = OperandResolver.GetSingleValue(number, srcRowIndex, srcColumnIndex);
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
		if (num < (double)MinValue || num > (double)MaxValue)
		{
			return ErrorEval.NUM_ERROR;
		}
		int num2 = 0;
		if (num < 0.0)
		{
			num2 = DEFAULT_PLACES_VALUE;
		}
		else if (places != null)
		{
			ValueEval singleValue2;
			try
			{
				singleValue2 = OperandResolver.GetSingleValue(places, srcRowIndex, srcColumnIndex);
			}
			catch (EvaluationException ex2)
			{
				return ex2.GetErrorEval();
			}
			double num3 = OperandResolver.ParseDouble(OperandResolver.CoerceValueToString(singleValue2));
			if (double.IsNaN(num3))
			{
				return ErrorEval.VALUE_INVALID;
			}
			num2 = (int)num3;
			if (num2 < 0)
			{
				return ErrorEval.NUM_ERROR;
			}
		}
		string text = "";
		text = ((num2 == 0) ? $"{(long)num:X}" : string.Format("{0:X" + num2 + "}", (int)num));
		if (num < 0.0)
		{
			text = "FF" + text.Substring(2);
		}
		return new StringEval(text.ToUpper());
	}

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0)
	{
		return Evaluate(srcRowIndex, srcColumnIndex, arg0, null);
	}

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		if (args.Length == 1)
		{
			return Evaluate(ec.RowIndex, ec.ColumnIndex, args[0]);
		}
		if (args.Length == 2)
		{
			return Evaluate(ec.RowIndex, ec.ColumnIndex, args[0], args[1]);
		}
		return ErrorEval.VALUE_INVALID;
	}
}
