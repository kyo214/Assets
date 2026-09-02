using System;
using System.Text;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Complex : Var2or3ArgFunction, FreeRefFunction
{
	public static FreeRefFunction Instance = new Complex();

	public static string DEFAULT_SUFFIX = "i";

	public static string SUPPORTED_SUFFIX = "j";

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval real_num, ValueEval i_num)
	{
		return Evaluate(srcRowIndex, srcColumnIndex, real_num, i_num, new StringEval(DEFAULT_SUFFIX));
	}

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval real_num, ValueEval i_num, ValueEval suffix)
	{
		ValueEval singleValue;
		try
		{
			singleValue = OperandResolver.GetSingleValue(real_num, srcRowIndex, srcColumnIndex);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		double num = 0.0;
		try
		{
			num = OperandResolver.CoerceValueToDouble(singleValue);
		}
		catch (EvaluationException)
		{
			return ErrorEval.VALUE_INVALID;
		}
		ValueEval singleValue2;
		try
		{
			singleValue2 = OperandResolver.GetSingleValue(i_num, srcRowIndex, srcColumnIndex);
		}
		catch (EvaluationException ex3)
		{
			return ex3.GetErrorEval();
		}
		double num2 = 0.0;
		try
		{
			num2 = OperandResolver.CoerceValueToDouble(singleValue2);
		}
		catch (EvaluationException)
		{
			return ErrorEval.VALUE_INVALID;
		}
		string text = OperandResolver.CoerceValueToString(suffix);
		if (text.Length == 0)
		{
			text = DEFAULT_SUFFIX;
		}
		if (text.Equals(DEFAULT_SUFFIX.ToUpper()) || text.Equals(SUPPORTED_SUFFIX.ToUpper()))
		{
			return ErrorEval.VALUE_INVALID;
		}
		if (!text.Equals(DEFAULT_SUFFIX) && !text.Equals(SUPPORTED_SUFFIX))
		{
			return ErrorEval.VALUE_INVALID;
		}
		StringBuilder stringBuilder = new StringBuilder("");
		if (num != 0.0)
		{
			if (isDoubleAnInt(num))
			{
				stringBuilder.Append((int)num);
			}
			else
			{
				stringBuilder.Append(num);
			}
		}
		if (num2 != 0.0)
		{
			if (stringBuilder.Length != 0 && num2 > 0.0)
			{
				stringBuilder.Append("+");
			}
			if (num2 != 1.0 && num2 != -1.0)
			{
				if (isDoubleAnInt(num2))
				{
					stringBuilder.Append((int)num2);
				}
				else
				{
					stringBuilder.Append(num2);
				}
			}
			stringBuilder.Append(text);
		}
		return new StringEval(stringBuilder.ToString());
	}

	private bool isDoubleAnInt(double number)
	{
		if (number == Math.Floor(number))
		{
			return !double.IsInfinity(number);
		}
		return false;
	}

	public ValueEval Evaluate(ValueEval[] args, OperationEvaluationContext ec)
	{
		if (args.Length == 2)
		{
			return Evaluate(ec.RowIndex, ec.ColumnIndex, args[0], args[1]);
		}
		if (args.Length == 3)
		{
			return Evaluate(ec.RowIndex, ec.ColumnIndex, args[0], args[1], args[2]);
		}
		return ErrorEval.VALUE_INVALID;
	}
}
