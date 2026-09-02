using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.SS.Format;
using NPOI.SS.Util;

namespace NPOI.SS.UserModel;

public class FractionFormat : FormatBase
{
	private class SimpleFractionException : Exception
	{
		public SimpleFractionException(string message)
			: base(message)
		{
		}
	}

	private static Regex DENOM_FORMAT_PATTERN = new Regex("(?:(#+)|(\\d+))", RegexOptions.Compiled);

	private static int MAX_DENOM_POW = 4;

	private int exactDenom;

	private int maxDenom;

	private string wholePartFormatString;

	public FractionFormat(string wholePartFormatString, string denomFormatString)
	{
		this.wholePartFormatString = wholePartFormatString;
		Match match = DENOM_FORMAT_PATTERN.Match(denomFormatString);
		int num = -1;
		int num2 = -1;
		if (match.Success)
		{
			if (match.Groups[2] != null && match.Groups[2].Success)
			{
				try
				{
					num = int.Parse(match.Groups[2].Value);
					if (num == 0)
					{
						num = -1;
					}
				}
				catch (FormatException)
				{
				}
			}
			else if (match.Groups[1] != null && match.Groups[1].Success)
			{
				int length = match.Groups[1].Value.Length;
				num2 = (int)Math.Pow(10.0, (length > MAX_DENOM_POW) ? MAX_DENOM_POW : length);
			}
			else
			{
				num = 100;
			}
		}
		if (num <= 0 && num2 <= 0)
		{
			num = 100;
		}
		exactDenom = num;
		maxDenom = num2;
	}

	public string Format(string num)
	{
		double result = 0.0;
		double.TryParse(num, out result);
		bool flag = result < 0.0;
		double num2 = Math.Abs(result);
		double num3 = Math.Floor(num2);
		double num4 = num2 - num3;
		if (num3 + num4 == 0.0)
		{
			return "0";
		}
		if (num2 < (double)(1 / Math.Max(exactDenom, maxDenom)))
		{
			return "0";
		}
		if (num3 + (double)(int)num4 == num3 + num4)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (flag)
			{
				stringBuilder.Append("-");
			}
			stringBuilder.Append((int)num3);
			return stringBuilder.ToString();
		}
		SimpleFraction simpleFraction = null;
		try
		{
			simpleFraction = ((exactDenom <= 0) ? SimpleFraction.BuildFractionMaxDenominator(num4, maxDenom) : SimpleFraction.BuildFractionExactDenominator(num4, exactDenom));
		}
		catch (SimpleFractionException)
		{
			return result.ToString();
		}
		StringBuilder stringBuilder2 = new StringBuilder();
		if (flag)
		{
			stringBuilder2.Append("-");
		}
		if ("".Equals(wholePartFormatString))
		{
			int value = simpleFraction.Denominator * (int)num3 + simpleFraction.Numerator;
			stringBuilder2.Append(value).Append("/").Append(simpleFraction.Denominator);
			return stringBuilder2.ToString();
		}
		if (simpleFraction.Numerator == 0)
		{
			stringBuilder2.Append((int)num3);
			return stringBuilder2.ToString();
		}
		if (simpleFraction.Numerator == simpleFraction.Denominator)
		{
			stringBuilder2.Append((int)num3 + 1);
			return stringBuilder2.ToString();
		}
		if (num3 > 0.0)
		{
			stringBuilder2.Append((int)num3).Append(" ");
		}
		stringBuilder2.Append(simpleFraction.Numerator).Append("/").Append(simpleFraction.Denominator);
		return stringBuilder2.ToString();
	}

	protected override StringBuilder Format(object obj, StringBuilder toAppendTo, int pos)
	{
		return toAppendTo.Append(Format(obj.ToString()));
	}

	public override string Format(object obj, CultureInfo culture)
	{
		return Format(obj.ToString());
	}

	public override object ParseObject(string source, int pos)
	{
		throw new NotImplementedException("Reverse parsing not supported");
	}

	public override StringBuilder Format(object obj, StringBuilder toAppendTo, CultureInfo culture)
	{
		return toAppendTo.Append(Format(obj, culture));
	}
}
