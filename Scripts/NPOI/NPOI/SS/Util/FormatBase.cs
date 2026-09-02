using System;
using System.Globalization;
using System.Text;

namespace NPOI.SS.Util;

public abstract class FormatBase
{
	public TimeZoneInfo TimeZone { get; set; }

	public FormatBase()
	{
	}

	public virtual string Format(object obj, CultureInfo culture)
	{
		return obj.ToString();
	}

	public virtual string Format(object obj)
	{
		return Format(obj, new StringBuilder(), 0).ToString();
	}

	protected virtual StringBuilder Format(object obj, StringBuilder sb, int pos)
	{
		return sb.Append(obj);
	}

	public abstract StringBuilder Format(object obj, StringBuilder toAppendTo, CultureInfo culture);

	public abstract object ParseObject(string source, int pos);
}
