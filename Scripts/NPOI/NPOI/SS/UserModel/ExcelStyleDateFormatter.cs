using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using NPOI.SS.Util;

namespace NPOI.SS.UserModel;

public class ExcelStyleDateFormatter : SimpleDateFormat
{
	public const char MMMMM_START_SYMBOL = '\ue001';

	public const char MMMMM_TRUNCATE_SYMBOL = '\ue002';

	public const char H_BRACKET_SYMBOL = '\ue010';

	public const char HH_BRACKET_SYMBOL = '\ue011';

	public const char M_BRACKET_SYMBOL = '\ue012';

	public const char MM_BRACKET_SYMBOL = '\ue013';

	public const char S_BRACKET_SYMBOL = '\ue014';

	public const char SS_BRACKET_SYMBOL = '\ue015';

	public const char L_BRACKET_SYMBOL = '\ue016';

	public const char LL_BRACKET_SYMBOL = '\ue017';

	public const char QUOTE_SYMBOL = '\ue009';

	private static DecimalFormat format1digit;

	private static DecimalFormat format2digits;

	private static DecimalFormat format3digit;

	private static DecimalFormat format4digits;

	private double dateToBeFormatted;

	static ExcelStyleDateFormatter()
	{
		format1digit = new DecimalFormat("0");
		format2digits = new DecimalFormat("00");
		format3digit = new DecimalFormat("0");
		format4digits = new DecimalFormat("00");
	}

	public ExcelStyleDateFormatter()
	{
	}

	public ExcelStyleDateFormatter(string pattern)
		: base(ProcessFormatPattern(pattern))
	{
	}

	private static string DateTimeMatchEvaluator(Match match)
	{
		return match.Groups[1].Value;
	}

	private static string ProcessFormatPattern(string f)
	{
		return Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(Regex.Replace(f.Replace("MMMMM", "\ue001MMM\ue002"), "\\[H\\]", '\ue010'.ToString(), RegexOptions.IgnoreCase), "\\[HH\\]", '\ue011'.ToString(), RegexOptions.IgnoreCase), "\\[m\\]", '\ue012'.ToString(), RegexOptions.IgnoreCase), "\\[mm\\]", '\ue013'.ToString(), RegexOptions.IgnoreCase), "\\[s\\]", '\ue014'.ToString(), RegexOptions.IgnoreCase), "\\[ss\\]", '\ue015'.ToString(), RegexOptions.IgnoreCase).Replace("s.000", "s.fff").Replace("s.00", "s.\ue017")
			.Replace("s.0", "s.\ue016")
			.Replace("\"", '\ue009'.ToString()), "(?<![M%])M(?!M)", "%M");
	}

	public void SetDateToBeFormatted(double date)
	{
		dateToBeFormatted = date;
	}

	public override string Format(object obj, CultureInfo culture)
	{
		return Format((DateTime)obj, new StringBuilder(), culture).ToString();
	}

	public StringBuilder Format(DateTime date, StringBuilder paramStringBuilder, CultureInfo culture)
	{
		string empty = string.Empty;
		empty = ((!Regex.IsMatch(base.Pattern, "[yYmMdDhHsS\\-/,. :\"\\\\]+0?[ampAMP/]*")) ? base.Pattern : date.ToString(base.Pattern, culture));
		if (empty.IndexOf('\ue009') != -1)
		{
			empty = empty.Replace('\ue009', '"');
		}
		if (empty.IndexOf('\ue001') != -1)
		{
			Regex regex = new Regex("\ue001(\\w)\\w+\ue002", RegexOptions.IgnoreCase);
			Match match = regex.Match(empty);
			if (match.Success)
			{
				empty = regex.Replace(empty, match.Groups[1].Value);
			}
		}
		if (empty.IndexOf('\ue010') != -1 || empty.IndexOf('\ue011') != -1)
		{
			double d = dateToBeFormatted * 24.0 + 0.01;
			d = Math.Floor(d);
			empty = empty.Replace('\ue010'.ToString(), format1digit.Format(d, culture));
			empty = empty.Replace('\ue011'.ToString(), format2digits.Format(d, culture));
		}
		if (empty.IndexOf('\ue012') != -1 || empty.IndexOf('\ue013') != -1)
		{
			double d2 = dateToBeFormatted * 24.0 * 60.0 + 0.01;
			d2 = Math.Floor(d2);
			empty = empty.Replace('\ue012'.ToString(), format1digit.Format(d2, culture));
			empty = empty.Replace('\ue013'.ToString(), format2digits.Format(d2, culture));
		}
		if (empty.IndexOf('\ue014') != -1 || empty.IndexOf('\ue015') != -1)
		{
			double num = dateToBeFormatted * 24.0 * 60.0 * 60.0 + 0.01;
			empty = empty.Replace('\ue014'.ToString(), format1digit.Format(num, culture));
			empty = empty.Replace('\ue015'.ToString(), format2digits.Format(num, culture));
		}
		if (empty.IndexOf('\ue016') != -1 || empty.IndexOf('\ue017') != -1)
		{
			float num2 = (float)((dateToBeFormatted - Math.Floor(dateToBeFormatted)) * 24.0 * 60.0 * 60.0);
			float num3 = num2 - (float)(int)num2;
			empty = empty.Replace('\ue016'.ToString(), format3digit.Format(num3 * 10f, culture));
			empty = empty.Replace('\ue017'.ToString(), format4digits.Format(num3 * 100f, culture));
		}
		return new StringBuilder(empty);
	}

	public override bool Equals(object o)
	{
		if (!(o is ExcelStyleDateFormatter))
		{
			return false;
		}
		ExcelStyleDateFormatter excelStyleDateFormatter = (ExcelStyleDateFormatter)o;
		return dateToBeFormatted == excelStyleDateFormatter.dateToBeFormatted;
	}

	public override int GetHashCode()
	{
		return dateToBeFormatted.GetHashCode();
	}
}
