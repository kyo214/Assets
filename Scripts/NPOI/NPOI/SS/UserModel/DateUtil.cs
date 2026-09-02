using System;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace NPOI.SS.UserModel;

public class DateUtil
{
	public const int SECONDS_PER_MINUTE = 60;

	public const int MINUTES_PER_HOUR = 60;

	public const int HOURS_PER_DAY = 24;

	public const int SECONDS_PER_DAY = 86400;

	private const int BAD_DATE = -1;

	public const long DAY_MILLISECONDS = 86400000L;

	private static readonly char[] TIME_SEPARATOR_PATTERN = new char[1] { ':' };

	private static Regex date_ptrn1 = new Regex("^\\[\\$\\-.*?\\]", RegexOptions.Compiled);

	private static Regex date_ptrn2 = new Regex("^\\[[a-zA-Z]+\\]", RegexOptions.Compiled);

	private static Regex date_ptrn3a = new Regex("[yYmMdDhHsS]", RegexOptions.Compiled);

	private static Regex date_ptrn3b = new Regex("^[\\[\\]yYmMdDhHsS\\-T/,. :\"\\\\]+0*[ampAMP/]*$", RegexOptions.Compiled);

	private static Regex date_ptrn4 = new Regex("^\\[([hH]+|[mM]+|[sS]+)\\]$", RegexOptions.Compiled);

	private static int lastFormatIndex = -1;

	private static string lastFormatString = null;

	private static bool cached = false;

	private static string syncIsADateFormat = "IsADateFormat";

	public static int absoluteDay(DateTime cal, bool use1904windowing)
	{
		int num = (cal - new DateTime(1899, 12, 31)).Days;
		if ((cal > new DateTime(1900, 3, 1)) & use1904windowing)
		{
			num++;
		}
		return num;
	}

	public static int AbsoluteDay(DateTime cal, bool use1904windowing)
	{
		return cal.DayOfYear + DaysInPriorYears(cal.Year, use1904windowing);
	}

	private static int DaysInPriorYears(int yr, bool use1904windowing)
	{
		if ((!use1904windowing && yr < 1900) || (use1904windowing && yr < 1904))
		{
			throw new ArgumentException("'year' must be 1900 or greater");
		}
		int num = yr - 1;
		int num2 = num / 4 - num / 100 + num / 400 - 460;
		return 365 * (yr - (use1904windowing ? 1904 : 1900)) + num2;
	}

	public static double GetExcelDate(DateTime date)
	{
		return GetExcelDate(date, use1904windowing: false);
	}

	public static double GetExcelDate(int year, int month, int day, int hour, int minute, int second, bool use1904windowing)
	{
		if ((!use1904windowing && year < 1900) || (use1904windowing && year < 1904))
		{
			return -1.0;
		}
		DateTime dateTime = ((!use1904windowing) ? new DateTime(1900, 1, 1) : new DateTime(1904, 1, 1));
		int months = 0;
		if (month > 12)
		{
			months = month - 12;
			month = 12;
		}
		int num = 0;
		switch (month)
		{
		case 1:
		case 3:
		case 5:
		case 7:
		case 8:
		case 10:
		case 12:
			if (day > 31)
			{
				num = day - 31;
				day = 31;
			}
			break;
		case 4:
		case 6:
		case 9:
		case 11:
			if (day > 30)
			{
				num = day - 30;
				day = 30;
			}
			break;
		default:
			if (DateTime.IsLeapYear(year))
			{
				if (day > 29)
				{
					num = day - 29;
					day = 29;
				}
			}
			else if (day > 28)
			{
				num = day - 28;
				day = 28;
			}
			break;
		}
		if (day <= 0)
		{
			num = day - 1;
			day = 1;
		}
		DateTime dateTime2 = new DateTime(year, month, day, hour, minute, second).AddMonths(months).AddDays(num);
		double num2 = (dateTime2 - dateTime).TotalDays + 1.0;
		if (!use1904windowing && num2 >= 60.0)
		{
			num2++;
		}
		else if (use1904windowing)
		{
			num2--;
		}
		return num2;
	}

	public static double GetExcelDate(DateTime date, bool use1904windowing)
	{
		if ((!use1904windowing && date.Year < 1900) || (use1904windowing && date.Year < 1904))
		{
			return -1.0;
		}
		DateTime dateTime = ((!use1904windowing) ? new DateTime(1900, 1, 1) : new DateTime(1904, 1, 1));
		double num = (date - dateTime).TotalDays + 1.0;
		if (!use1904windowing && num >= 60.0)
		{
			num++;
		}
		else if (use1904windowing)
		{
			num--;
		}
		return num;
	}

	public static DateTime GetJavaDate(double date)
	{
		return GetJavaDate(date, use1904windowing: false);
	}

	public static DateTime GetJavaDate(double date, TimeZoneInfo tz)
	{
		return GetJavaDate(date, use1904windowing: false, tz, roundSeconds: false);
	}

	[Obsolete("The class TimeZone was marked obsolete, Use the Overload using TimeZoneInfo instead.")]
	public static DateTime GetJavaDate(double date, TimeZone tz)
	{
		return GetJavaDate(date, use1904windowing: false, tz, roundSeconds: false);
	}

	public static DateTime GetJavaDate(double date, bool use1904windowing)
	{
		return GetJavaCalendar(date, use1904windowing, (TimeZoneInfo)null, false);
	}

	public static DateTime GetJavaDate(double date, bool use1904windowing, TimeZoneInfo tz)
	{
		return GetJavaCalendar(date, use1904windowing, tz, roundSeconds: false);
	}

	[Obsolete("The class TimeZone was marked obsolete, Use the Overload using TimeZoneInfo instead.")]
	public static DateTime GetJavaDate(double date, bool use1904windowing, TimeZone tz)
	{
		return GetJavaCalendar(date, use1904windowing, tz, roundSeconds: false);
	}

	public static DateTime GetJavaDate(double date, bool use1904windowing, TimeZoneInfo tz, bool roundSeconds)
	{
		return GetJavaCalendar(date, use1904windowing, tz, roundSeconds);
	}

	[Obsolete("The class TimeZone was marked obsolete, Use the Overload using TimeZoneInfo instead.")]
	public static DateTime GetJavaDate(double date, bool use1904windowing, TimeZone tz, bool roundSeconds)
	{
		return GetJavaCalendar(date, use1904windowing, tz, roundSeconds);
	}

	public static DateTime SetCalendar(int wholeDays, int millisecondsInDay, bool use1904windowing, bool roundSeconds)
	{
		int year = 1900;
		int num = -1;
		if (use1904windowing)
		{
			year = 1904;
			num = 1;
		}
		else if (wholeDays < 61)
		{
			num = 0;
		}
		DateTime result = new DateTime(year, 1, 1).AddDays(wholeDays + num - 1).AddMilliseconds(millisecondsInDay);
		if (roundSeconds)
		{
			result = result.AddMilliseconds(500.0);
			result = result.AddMilliseconds(-result.Millisecond);
		}
		return result;
	}

	public static DateTime GetJavaCalendar(double date)
	{
		return GetJavaCalendar(date, false, (TimeZoneInfo)null, false);
	}

	public static DateTime GetJavaCalendar(double date, bool use1904windowing)
	{
		return GetJavaCalendar(date, use1904windowing, (TimeZoneInfo)null, false);
	}

	public static DateTime GetJavaCalendarUTC(double date, bool use1904windowing)
	{
		return TimeZoneInfo.ConvertTimeToUtc(GetJavaCalendar(date, use1904windowing, (TimeZoneInfo)null, false));
	}

	public static DateTime GetJavaCalendar(double date, bool use1904windowing, TimeZoneInfo timeZone)
	{
		return GetJavaCalendar(date, use1904windowing, timeZone, roundSeconds: false);
	}

	public static DateTime GetJavaCalendar(double date, bool use1904windowing, TimeZoneInfo timeZone, bool roundSeconds)
	{
		if (!IsValidExcelDate(date))
		{
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid Excel date double value: {0}", new object[1] { date }));
		}
		int num = (int)Math.Floor(date);
		int millisecondsInDay = (int)((date - (double)num) * 86400000.0 + 0.5);
		_ = DateTime.Now;
		return SetCalendar(num, millisecondsInDay, use1904windowing, roundSeconds);
	}

	[Obsolete("The class TimeZone was marked obsolete, Use the Overload using TimeZoneInfo instead.")]
	public static DateTime GetJavaCalendar(double date, bool use1904windowing, TimeZone timeZone)
	{
		return GetJavaCalendar(date, use1904windowing, timeZone, roundSeconds: false);
	}

	[Obsolete("The class TimeZone was marked obsolete, Use the Overload using TimeZoneInfo instead.")]
	public static DateTime GetJavaCalendar(double date, bool use1904windowing, TimeZone timeZone, bool roundSeconds)
	{
		if (!IsValidExcelDate(date))
		{
			throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Invalid Excel date double value: {0}", new object[1] { date }));
		}
		int num = (int)Math.Floor(date);
		int millisecondsInDay = (int)((date - (double)num) * 86400000.0 + 0.5);
		_ = DateTime.Now;
		return SetCalendar(num, millisecondsInDay, use1904windowing, roundSeconds);
	}

	public static double ConvertTime(string timeStr)
	{
		try
		{
			return ConvertTimeInternal(timeStr);
		}
		catch (FormatException ex)
		{
			throw new ArgumentException("Bad time format '" + timeStr + "' expected 'HH:MM' or 'HH:MM:SS' - " + ex.Message);
		}
	}

	private static double ConvertTimeInternal(string timeStr)
	{
		int length = timeStr.Length;
		if (length < 4 || length > 8)
		{
			throw new FormatException("Bad length");
		}
		string[] array = timeStr.Split(TIME_SEPARATOR_PATTERN);
		string strVal = array.Length switch
		{
			2 => "00", 
			3 => array[2], 
			_ => throw new FormatException("Expected 2 or 3 fields but got (" + array.Length + ")"), 
		};
		string strVal2 = array[0];
		string strVal3 = array[1];
		int num = ParseInt(strVal2, "hour", 24);
		int num2 = ParseInt(strVal3, "minute", 60);
		return (double)(ParseInt(strVal, "second", 60) + (num2 + num * 60) * 60) / 86400.0;
	}

	public static bool IsADateFormat(int formatIndex, string formatString)
	{
		lock (syncIsADateFormat)
		{
			if (formatString != null && formatIndex == lastFormatIndex && formatString.Equals(lastFormatString))
			{
				return cached;
			}
			if (IsInternalDateFormat(formatIndex))
			{
				lastFormatIndex = formatIndex;
				lastFormatString = formatString;
				cached = true;
				return true;
			}
			if (formatString == null || formatString.Length == 0)
			{
				lastFormatIndex = formatIndex;
				lastFormatString = formatString;
				cached = false;
				return false;
			}
			string input = formatString;
			input = Regex.Replace(input, ";@", "");
			int length = input.Length;
			StringBuilder stringBuilder = new StringBuilder(length);
			for (int i = 0; i < length; i++)
			{
				char c = input[i];
				if (i < length - 1)
				{
					char c2 = input[i + 1];
					switch (c)
					{
					case '\\':
						switch (c2)
						{
						case ' ':
						case ',':
						case '-':
						case '.':
						case '\\':
							continue;
						}
						break;
					case ';':
						if (c2 == '@')
						{
							i++;
							continue;
						}
						break;
					}
				}
				stringBuilder.Append(c);
			}
			input = stringBuilder.ToString();
			if (date_ptrn4.IsMatch(input))
			{
				lastFormatIndex = formatIndex;
				lastFormatString = formatString;
				cached = true;
				return true;
			}
			input = date_ptrn1.Replace(input, "");
			input = date_ptrn2.Replace(input, "");
			int num = input.IndexOf(';');
			if (num > 0 && num < input.Length - 1)
			{
				input = input.Substring(0, num);
			}
			if (!date_ptrn3a.Match(input).Success)
			{
				return false;
			}
			input = Regex.Replace(input, "\"[^\"\\\\]*(?:\\\\.[^\"\\\\]*)*\"", "");
			bool result = date_ptrn3b.IsMatch(input);
			lastFormatIndex = formatIndex;
			lastFormatString = formatString;
			cached = result;
			return result;
		}
	}

	public static DateTime ParseYYYYMMDDDate(string dateStr)
	{
		try
		{
			return ParseYYYYMMDDDateInternal(dateStr);
		}
		catch (FormatException ex)
		{
			throw new ArgumentException("Bad time format " + dateStr + " expected 'YYYY/MM/DD' - " + ex.Message);
		}
	}

	private static DateTime ParseYYYYMMDDDateInternal(string timeStr)
	{
		if (timeStr.Length != 10)
		{
			throw new FormatException("Bad length");
		}
		string strVal = timeStr.Substring(0, 4);
		string strVal2 = timeStr.Substring(5, 2);
		string strVal3 = timeStr.Substring(8, 2);
		int year = ParseInt(strVal, "year", -32768, 32767);
		int month = ParseInt(strVal2, "month", 1, 12);
		int day = ParseInt(strVal3, "day", 1, 31);
		return new DateTime(year, month, day, 0, 0, 0);
	}

	private static int ParseInt(string strVal, string fieldName, int rangeMax)
	{
		return ParseInt(strVal, fieldName, 0, rangeMax - 1);
	}

	private static int ParseInt(string strVal, string fieldName, int lowerLimit, int upperLimit)
	{
		int num;
		try
		{
			num = int.Parse(strVal, CultureInfo.InvariantCulture);
		}
		catch (FormatException)
		{
			throw new FormatException("Bad int format '" + strVal + "' for " + fieldName + " field");
		}
		if (num < lowerLimit || num > upperLimit)
		{
			throw new FormatException(fieldName + " value (" + num + ") is outside the allowable range(0.." + upperLimit + ")");
		}
		return num;
	}

	public static bool IsInternalDateFormat(int format)
	{
		bool flag = false;
		if ((uint)(format - 14) <= 8u || (uint)(format - 45) <= 2u)
		{
			return true;
		}
		return false;
	}

	public static bool IsCellDateFormatted(ICell cell)
	{
		if (cell == null)
		{
			return false;
		}
		bool result = false;
		if (IsValidExcelDate(cell.NumericCellValue))
		{
			ICellStyle cellStyle = cell.CellStyle;
			if (cellStyle == null)
			{
				return false;
			}
			int dataFormat = cellStyle.DataFormat;
			string dataFormatString = cellStyle.GetDataFormatString();
			result = IsADateFormat(dataFormat, dataFormatString);
		}
		return result;
	}

	public static bool IsCellInternalDateFormatted(ICell cell)
	{
		if (cell == null)
		{
			return false;
		}
		bool result = false;
		if (IsValidExcelDate(cell.NumericCellValue))
		{
			result = IsInternalDateFormat(cell.CellStyle.DataFormat);
		}
		return result;
	}

	public static bool IsValidExcelDate(double value)
	{
		return value > -double.Epsilon;
	}
}
