using System.Globalization;

namespace NPOI.SS.Util;

public abstract class DateFormat : FormatBase
{
	public const int FULL = 0;

	public const int LONG = 1;

	public const int MEDIUM = 2;

	public const int SHORT = 3;

	public const int DEFAULT = 2;

	public static string GetDateTimePattern(int dateStyle, int timeStyle, CultureInfo locale)
	{
		_ = locale.DateTimeFormat;
		string datePattern = GetDatePattern(dateStyle, locale);
		string timePattern = GetTimePattern(timeStyle, locale);
		if (locale.TextInfo.IsRightToLeft)
		{
			return timePattern + " " + datePattern;
		}
		return datePattern + " " + timePattern;
	}

	public static string GetDatePattern(int dateStyle, CultureInfo locale)
	{
		DateTimeFormatInfo dateTimeFormat = locale.DateTimeFormat;
		return dateStyle switch
		{
			3 => dateTimeFormat.ShortDatePattern.Replace("yyyy", "yy").Replace("YYYY", "YY"), 
			2 => dateTimeFormat.ShortDatePattern, 
			1 => dateTimeFormat.LongDatePattern.Replace("dddd,", "").Trim(), 
			0 => dateTimeFormat.LongDatePattern, 
			_ => dateTimeFormat.ShortDatePattern, 
		};
	}

	public static string GetTimePattern(int timeStyle, CultureInfo locale)
	{
		DateTimeFormatInfo dateTimeFormat = locale.DateTimeFormat;
		if ((uint)timeStyle > 2u && timeStyle == 3)
		{
			return dateTimeFormat.ShortTimePattern;
		}
		return dateTimeFormat.LongTimePattern;
	}
}
