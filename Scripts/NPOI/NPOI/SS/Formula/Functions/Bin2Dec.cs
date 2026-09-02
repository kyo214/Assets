using System;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Bin2Dec : Fixed1ArgFunction, FreeRefFunction
{
	public static FreeRefFunction instance = new Bin2Dec();

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval numberVE)
	{
		string text;
		if (numberVE is RefEval)
		{
			RefEval obj = (RefEval)numberVE;
			text = OperandResolver.CoerceValueToString(obj.GetInnerValueEval(obj.FirstSheetIndex));
		}
		else
		{
			text = OperandResolver.CoerceValueToString(numberVE);
		}
		if (text.Length > 10)
		{
			return ErrorEval.NUM_ERROR;
		}
		string text2;
		bool flag;
		if (text.Length < 10)
		{
			text2 = text;
			flag = true;
		}
		else
		{
			text2 = text.Substring(1);
			flag = text.StartsWith("0");
		}
		string s;
		try
		{
			if (flag)
			{
				s = getDecimalValue(text2).ToString();
			}
			else
			{
				string unsigned = toggleBits(text2);
				int decimalValue = getDecimalValue(unsigned);
				s = "-" + (decimalValue + 1);
			}
		}
		catch (FormatException)
		{
			return ErrorEval.NUM_ERROR;
		}
		return new NumberEval(long.Parse(s));
	}

	private int getDecimalValue(string unsigned)
	{
		int num = 0;
		int length = unsigned.Length;
		int num2 = length - 1;
		for (int i = 0; i < length; i++)
		{
			int num3 = (int)((double)int.Parse(unsigned.Substring(i, 1)) * Math.Pow(2.0, num2));
			num += num3;
			num2--;
		}
		return num;
	}

	private static string toggleBits(string s)
	{
		string text = Convert.ToString(Convert.ToInt64(s, 2) ^ ((1L << s.Length) - 1), 2);
		while (text.Length < s.Length)
		{
			text = "0" + text;
		}
		return text;
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
