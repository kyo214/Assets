using System;
using System.Globalization;

namespace NPOI.Util;

public class LocaleUtil
{
	public static TimeZoneInfo TIMEZONE_UTC = TimeZoneInfo.Utc;

	public static string CHARSET_1252 = CodePageUtil.CodepageToEncoding(1252);

	[ThreadStatic]
	private static TimeZoneInfo userTimeZone;

	[ThreadStatic]
	private static CultureInfo userLocale;

	public static void SetUserTimeZone(TimeZoneInfo timezone)
	{
		userTimeZone = timezone;
	}

	public static TimeZoneInfo GetUserTimeZoneInfo()
	{
		return userTimeZone ?? (userTimeZone = TimeZoneInfo.Local);
	}

	public static void SetUserLocale(CultureInfo locale)
	{
		userLocale = locale;
	}

	public static CultureInfo GetUserLocale()
	{
		return userLocale ?? (userLocale = CultureInfo.CurrentCulture);
	}

	public static DateTime GetLocaleCalendar()
	{
		return GetLocaleCalendar(GetUserTimeZoneInfo());
	}

	public static DateTime GetLocaleCalendar(int year, int month, int day)
	{
		return GetLocaleCalendar(year, month, day, 0, 0, 0);
	}

	public static DateTime GetLocaleCalendar(int year, int month, int day, int hour, int minute, int second)
	{
		if (month < 0 || day < 0)
		{
			throw new ArgumentOutOfRangeException();
		}
		if (day == 0)
		{
			return new DateTime(year, month, 1, hour, minute, second).AddDays(-1.0);
		}
		return new DateTime(year, month, day, hour, minute, second);
	}

	public static DateTime GetLocaleCalendar(TimeZoneInfo timeZone)
	{
		return TimeZoneInfo.ConvertTime(DateTime.Now, timeZone);
	}

	[Obsolete("The class TimeZone was marked obsolete, Use the Overload using TimeZoneInfo instead.")]
	public static DateTime GetLocaleCalendar(TimeZone timeZone)
	{
		return timeZone.ToLocalTime(DateTime.Now);
	}
}
