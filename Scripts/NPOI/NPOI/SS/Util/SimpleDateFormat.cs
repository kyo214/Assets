using System;
using System.Globalization;
using System.Text;

namespace NPOI.SS.Util;

public class SimpleDateFormat : DateFormat
{
	private string _pattern;

	private DateTimeFormatInfo _formatData;

	private CultureInfo _culture;

	public string Pattern => _pattern;

	public SimpleDateFormat()
		: this("", CultureInfo.CurrentCulture)
	{
	}

	public SimpleDateFormat(string pattern, CultureInfo culture)
	{
		if (pattern == null || culture == null)
		{
			throw new ArgumentNullException();
		}
		_pattern = pattern;
		_formatData = (DateTimeFormatInfo)culture.DateTimeFormat.Clone();
		_culture = culture;
	}

	public SimpleDateFormat(string pattern, DateTimeFormatInfo formatSymbols)
	{
		if (pattern == null || formatSymbols == null)
		{
			throw new ArgumentNullException();
		}
		_pattern = pattern;
		_formatData = (DateTimeFormatInfo)formatSymbols.Clone();
		_culture = CultureInfo.CurrentCulture;
	}

	public SimpleDateFormat(string pattern)
	{
		_pattern = pattern;
	}

	public override string Format(object obj)
	{
		return Format(obj, CultureInfo.CurrentCulture);
	}

	public override string Format(object obj, CultureInfo culture)
	{
		DateTime dateTime = (DateTime)obj;
		if (base.TimeZone != null)
		{
			dateTime = TimeZoneInfo.ConvertTime(dateTime, base.TimeZone);
		}
		return dateTime.ToString(_pattern, culture);
	}

	public override StringBuilder Format(object obj, StringBuilder toAppendTo, CultureInfo culture)
	{
		return toAppendTo.Append(Format((DateTime)obj, culture));
	}

	public override object ParseObject(string source, int pos)
	{
		DateTime dateTime = DateTime.Parse(source.Substring(pos), CultureInfo.InvariantCulture);
		return (base.TimeZone != null) ? TimeZoneInfo.ConvertTime(dateTime, base.TimeZone) : dateTime;
	}

	public DateTime Parse(string source)
	{
		DateTime dateTime = DateTime.Parse(source, CultureInfo.InvariantCulture);
		if (base.TimeZone == null)
		{
			return dateTime;
		}
		return TimeZoneInfo.ConvertTime(dateTime, base.TimeZone);
	}
}
