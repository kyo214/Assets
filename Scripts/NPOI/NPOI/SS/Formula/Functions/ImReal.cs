using System;
using System.Text.RegularExpressions;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class ImReal : Fixed1ArgFunction, FreeRefFunction
{
	public static FreeRefFunction instance = new ImReal();

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval inumberVE)
	{
		ValueEval singleValue;
		try
		{
			singleValue = OperandResolver.GetSingleValue(inumberVE, srcRowIndex, srcColumnIndex);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		string input = OperandResolver.CoerceValueToString(singleValue);
		System.Text.RegularExpressions.Match match = Imaginary.COMPLEX_NUMBER_PATTERN.Match(input);
		bool num = match.Success && !string.IsNullOrEmpty(match.Groups[0].Value);
		string value = "";
		if (num)
		{
			string value2 = match.Groups[2].Value;
			bool flag = value2.Length != 0;
			if (value2.Length == 0)
			{
				return new StringEval(Convert.ToString(0));
			}
			if (flag)
			{
				string text = "";
				string value3 = match.Groups[Imaginary.GROUP1_REAL_SIGN].Value;
				if (value3.Length != 0 && !value3.Equals("+"))
				{
					text = value3;
				}
				string value4 = match.Groups[Imaginary.GROUP2_IMAGINARY_INTEGER_OR_DOUBLE].Value;
				value = ((value4.Length == 0) ? (text + "1") : (text + value4));
			}
			return new StringEval(value);
		}
		return ErrorEval.NUM_ERROR;
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
