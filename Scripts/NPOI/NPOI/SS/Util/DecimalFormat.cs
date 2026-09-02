using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace NPOI.SS.Util;

public class DecimalFormat : FormatBase
{
	private string _pattern;

	private NumberFormatInfo _formatInfo;

	private static readonly Regex RegexFraction = new Regex("#+/#+", RegexOptions.Compiled);

	public string Pattern => _pattern;

	public bool ParseIntegerOnly => false;

	public DecimalFormat()
	{
	}

	public DecimalFormat(string pattern)
	{
		if (pattern.IndexOf("'", StringComparison.Ordinal) != -1)
		{
			throw new ArgumentException("invalid pattern");
		}
		_pattern = pattern;
	}

	public DecimalFormat(string pattern, NumberFormatInfo formatInfo)
		: this(pattern)
	{
		_formatInfo = formatInfo;
	}

	public override string Format(object obj)
	{
		return Format(obj, CultureInfo.CurrentCulture);
	}

	public override string Format(object obj, CultureInfo culture)
	{
		_pattern = RegexFraction.Replace(_pattern, "/");
		if (_formatInfo != null)
		{
			culture = (CultureInfo)culture.Clone();
			culture.NumberFormat = _formatInfo;
		}
		if (_pattern.IndexOf("'", StringComparison.Ordinal) != -1)
		{
			return Convert.ToDouble(obj, CultureInfo.InvariantCulture).ToString(culture);
		}
		string text = Convert.ToDouble(obj, CultureInfo.InvariantCulture).ToString(_pattern, culture);
		if (string.IsNullOrEmpty(text))
		{
			text = "0";
		}
		return text;
	}

	public override StringBuilder Format(object obj, StringBuilder toAppendTo, CultureInfo culture)
	{
		return toAppendTo.Append(Format(obj, culture));
	}

	public override object ParseObject(string source, int pos)
	{
		return decimal.Parse(source.Substring(pos), CultureInfo.CurrentCulture);
	}
}
