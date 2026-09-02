using System;
using System.Text;

namespace NPOI.SS.Util;

public class NumberToTextConverter
{
	private const long EXCEL_NAN_BITS = -276939487313920L;

	private const int MAX_TEXT_LEN = 20;

	private NumberToTextConverter()
	{
	}

	public static string ToText(double value)
	{
		return RawDoubleBitsToText(BitConverter.DoubleToInt64Bits(value));
	}

	public static string RawDoubleBitsToText(long pRawBits)
	{
		long num = pRawBits;
		bool flag = num < 0;
		if (flag)
		{
			num &= 0x7FFFFFFFFFFFFFFFL;
		}
		if (num == 0L)
		{
			if (!flag)
			{
				return "0";
			}
			return "-0";
		}
		ExpandedDouble expandedDouble = new ExpandedDouble(num);
		if (expandedDouble.GetBinaryExponent() < -1022)
		{
			if (!flag)
			{
				return "0";
			}
			return "-0";
		}
		if (expandedDouble.GetBinaryExponent() == 1024)
		{
			if (num == -276939487313920L)
			{
				return "3.484840871308E+308";
			}
			flag = false;
		}
		NormalisedDecimal pnd = expandedDouble.NormaliseBaseTen();
		StringBuilder stringBuilder = new StringBuilder(21);
		if (flag)
		{
			stringBuilder.Append('-');
		}
		ConvertToText(stringBuilder, pnd);
		return stringBuilder.ToString();
	}

	private static void ConvertToText(StringBuilder sb, NormalisedDecimal pnd)
	{
		NormalisedDecimal normalisedDecimal = pnd.RoundUnits();
		int num = normalisedDecimal.GetDecimalExponent();
		string text;
		if (Math.Abs(num) > 98)
		{
			text = normalisedDecimal.GetSignificantDecimalDigitsLastDigitRounded();
			if (text.Length == 16)
			{
				num++;
			}
		}
		else
		{
			text = normalisedDecimal.GetSignificantDecimalDigits();
		}
		int countSigDigits = CountSignifantDigits(text);
		if (num < 0)
		{
			FormatLessThanOne(sb, text, num, countSigDigits);
		}
		else
		{
			FormatGreaterThanOne(sb, text, num, countSigDigits);
		}
	}

	private static void FormatLessThanOne(StringBuilder sb, string decimalDigits, int decExponent, int countSigDigits)
	{
		int num = -decExponent - 1;
		if (NeedsScientificNotation(2 + num + countSigDigits))
		{
			sb.Append(decimalDigits[0]);
			if (countSigDigits > 1)
			{
				sb.Append('.');
				sb.Append(decimalDigits.Substring(1, countSigDigits - 1));
			}
			sb.Append("E-");
			AppendExp(sb, -decExponent);
		}
		else
		{
			sb.Append("0.");
			for (int num2 = num; num2 > 0; num2--)
			{
				sb.Append('0');
			}
			sb.Append(decimalDigits.Substring(0, countSigDigits));
		}
	}

	private static void FormatGreaterThanOne(StringBuilder sb, string decimalDigits, int decExponent, int countSigDigits)
	{
		if (decExponent > 19)
		{
			sb.Append(decimalDigits[0]);
			if (countSigDigits > 1)
			{
				sb.Append('.');
				sb.Append(decimalDigits.Substring(1, countSigDigits - 1));
			}
			sb.Append("E+");
			AppendExp(sb, decExponent);
			return;
		}
		int num = countSigDigits - decExponent - 1;
		if (num > 0)
		{
			sb.Append(decimalDigits.Substring(0, decExponent + 1));
			sb.Append('.');
			sb.Append(decimalDigits.Substring(decExponent + 1, num));
			return;
		}
		sb.Append(decimalDigits.Substring(0, countSigDigits));
		for (int num2 = -num; num2 > 0; num2--)
		{
			sb.Append('0');
		}
	}

	private static bool NeedsScientificNotation(int nDigits)
	{
		return nDigits > 20;
	}

	private static int CountSignifantDigits(string sb)
	{
		int num = sb.Length - 1;
		while (sb[num] == '0')
		{
			num--;
			if (num < 0)
			{
				throw new Exception("No non-zero digits found");
			}
		}
		return num + 1;
	}

	private static void AppendExp(StringBuilder sb, int val)
	{
		if (val < 10)
		{
			sb.Append('0');
			sb.Append((char)(48 + val));
		}
		else
		{
			sb.Append(val);
		}
	}
}
