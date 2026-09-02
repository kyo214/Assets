using System;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Dec2Bin : Var1or2ArgFunction, FreeRefFunction
{
	public static FreeRefFunction instance = new Dec2Bin();

	private static long MinValue = -512L;

	private static long MaxValue = 511L;

	private static int DEFAULT_PLACES_VALUE = 10;

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval numberVE, ValueEval placesVE)
	{
		ValueEval singleValue;
		try
		{
			singleValue = OperandResolver.GetSingleValue(numberVE, srcRowIndex, srcColumnIndex);
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
		int num2;
		if (num < 0.0 || placesVE == null)
		{
			num2 = DEFAULT_PLACES_VALUE;
		}
		else
		{
			ValueEval singleValue2;
			try
			{
				singleValue2 = OperandResolver.GetSingleValue(placesVE, srcRowIndex, srcColumnIndex);
			}
			catch (EvaluationException ex2)
			{
				return ex2.GetErrorEval();
			}
			double d = OperandResolver.ParseDouble(OperandResolver.CoerceValueToString(singleValue2));
			if (double.IsNaN(d))
			{
				return ErrorEval.VALUE_INVALID;
			}
			num2 = (int)Math.Floor(d);
			if (num2 < 0 || num2 == 0)
			{
				return ErrorEval.NUM_ERROR;
			}
		}
		string text = Convert.ToString((int)Math.Floor(num), 2);
		if (text.Length > DEFAULT_PLACES_VALUE)
		{
			text = text.Substring(text.Length - DEFAULT_PLACES_VALUE);
		}
		if (text.Length > num2)
		{
			return ErrorEval.NUM_ERROR;
		}
		return new StringEval(text);
	}

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval numberVE)
	{
		return Evaluate(srcRowIndex, srcColumnIndex, numberVE, null);
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
