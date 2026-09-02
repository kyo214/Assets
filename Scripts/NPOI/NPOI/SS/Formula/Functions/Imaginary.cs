using System;
using System.Text.RegularExpressions;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Imaginary : Fixed1ArgFunction, FreeRefFunction
{
	public static FreeRefFunction instance = new Imaginary();

	public static string GROUP1_REAL_SIGN_REGEX = "([+-]?)";

	public static string GROUP2_REAL_INTEGER_OR_DOUBLE_REGEX = "([0-9]+\\.[0-9]+|[0-9]*)";

	public static string GROUP3_IMAGINARY_SIGN_REGEX = "([+-]?)";

	public static string GROUP4_IMAGINARY_INTEGER_OR_DOUBLE_REGEX = "([0-9]+\\.[0-9]+|[0-9]*)";

	public static string GROUP5_IMAGINARY_GROUP_REGEX = "([ij]?)";

	public static Regex COMPLEX_NUMBER_PATTERN = new Regex(GROUP1_REAL_SIGN_REGEX + GROUP2_REAL_INTEGER_OR_DOUBLE_REGEX + GROUP3_IMAGINARY_SIGN_REGEX + GROUP4_IMAGINARY_INTEGER_OR_DOUBLE_REGEX + GROUP5_IMAGINARY_GROUP_REGEX);

	public static int GROUP1_REAL_SIGN = 1;

	public static int GROUP2_IMAGINARY_INTEGER_OR_DOUBLE = 2;

	public static int GROUP3_IMAGINARY_SIGN = 3;

	public static int GROUP4_IMAGINARY_INTEGER_OR_DOUBLE = 4;

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
		System.Text.RegularExpressions.Match match = COMPLEX_NUMBER_PATTERN.Match(input);
		bool num = match.Success && match.Groups[0].Length > 0;
		string value = "";
		if (num)
		{
			string value2 = match.Groups[5].Value;
			bool flag = value2.Equals("i") || value2.Equals("j");
			if (value2.Length == 0)
			{
				return new StringEval(Convert.ToString(0));
			}
			if (flag)
			{
				string text = "";
				string value3 = match.Groups[GROUP3_IMAGINARY_SIGN].Value;
				if (value3.Length != 0 && !value3.Equals("+"))
				{
					text = value3;
				}
				string value4 = match.Groups[GROUP4_IMAGINARY_INTEGER_OR_DOUBLE].Value;
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
