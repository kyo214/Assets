using System;
using System.Globalization;
using System.Text;
using NPOI.SS.Formula.Eval;

namespace NPOI.SS.Formula.Functions;

public class Value : Fixed1ArgFunction
{
	private const int MIN_DISTANCE_BETWEEN_THOUSANDS_SEPARATOR = 4;

	private const double ZERO = 0.0;

	public override ValueEval Evaluate(int srcRowIndex, int srcColumnIndex, ValueEval arg0)
	{
		ValueEval singleValue;
		try
		{
			singleValue = OperandResolver.GetSingleValue(arg0, srcRowIndex, srcColumnIndex);
		}
		catch (EvaluationException ex)
		{
			return ex.GetErrorEval();
		}
		double num = ConvertTextToNumber(OperandResolver.CoerceValueToString(singleValue));
		if (double.IsNaN(num))
		{
			return ErrorEval.VALUE_INVALID;
		}
		return new NumberEval(num);
	}

	private static double ConvertTextToNumber(string strText)
	{
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		int length = strText.Length;
		int i;
		for (i = 0; i < length; i++)
		{
			char c = strText[i];
			if (char.IsDigit(c))
			{
				break;
			}
			switch (c)
			{
			case '$':
				if (flag)
				{
					return double.NaN;
				}
				flag = true;
				continue;
			case '+':
				if (flag3 | flag2)
				{
					return double.NaN;
				}
				flag2 = true;
				continue;
			case '-':
				if (flag3 | flag2)
				{
					return double.NaN;
				}
				flag3 = true;
				continue;
			default:
				return double.NaN;
			case ' ':
				continue;
			case '.':
				break;
			}
			break;
		}
		if (i >= length)
		{
			if (flag | flag3 | flag2)
			{
				return double.NaN;
			}
			return 0.0;
		}
		bool flag5 = false;
		int num = -32768;
		StringBuilder stringBuilder = new StringBuilder(length);
		for (; i < length; i++)
		{
			char c2 = strText[i];
			if (char.IsDigit(c2))
			{
				stringBuilder.Append(c2);
				continue;
			}
			switch (c2)
			{
			case ' ':
			{
				string text = strText.Substring(i).Trim();
				if (text.Equals("%"))
				{
					flag4 = true;
				}
				else if (text.Length > 0)
				{
					return double.NaN;
				}
				break;
			}
			case '.':
				if (flag5)
				{
					return double.NaN;
				}
				if (i - num < 4)
				{
					return double.NaN;
				}
				flag5 = true;
				stringBuilder.Append('.');
				break;
			case ',':
				if (flag5)
				{
					return double.NaN;
				}
				if (i - num < 4)
				{
					return double.NaN;
				}
				num = i;
				break;
			case 'E':
			case 'e':
				if (i - num < 4)
				{
					return double.NaN;
				}
				stringBuilder.Append(strText.Substring(i));
				i = length;
				break;
			case '%':
				flag4 = true;
				break;
			default:
				return double.NaN;
			}
		}
		if (!flag5 && i - num < 4)
		{
			return double.NaN;
		}
		double num2;
		try
		{
			num2 = double.Parse(stringBuilder.ToString(), CultureInfo.InvariantCulture);
		}
		catch (FormatException)
		{
			return double.NaN;
		}
		double num3 = (flag3 ? (0.0 - num2) : num2);
		if (!flag4)
		{
			return num3;
		}
		return num3 / 100.0;
	}
}
