using System;
using System.Globalization;
using System.Text;

namespace NPOI.SS.Util;

public class PhoneFormat : FormatBase
{
	public static readonly FormatBase Instance = new PhoneFormat();

	private static string df = "##########";

	private PhoneFormat()
	{
	}

	public override string Format(object obj, CultureInfo culture)
	{
		string text = ((double)obj).ToString(df, culture);
		StringBuilder stringBuilder = new StringBuilder();
		int length = text.Length;
		if (length <= 4)
		{
			return text;
		}
		string value = text.Substring(length - 4);
		int num = Math.Max(0, length - 7);
		string text2 = text.Substring(Math.Max(0, length - 7), length - 4 - num);
		num = Math.Max(0, length - 10);
		string text3 = text.Substring(num, Math.Max(0, length - 7) - num);
		if (text3 != null && text3.Trim().Length > 0)
		{
			stringBuilder.Append('(').Append(text3).Append(") ");
		}
		if (text2 != null && text2.Trim().Length > 0)
		{
			stringBuilder.Append(text2).Append('-');
		}
		stringBuilder.Append(value);
		return stringBuilder.ToString();
	}

	protected override StringBuilder Format(object obj, StringBuilder toAppendTo, int pos)
	{
		return toAppendTo.Append(Format(obj, CultureInfo.CurrentCulture));
	}

	public override StringBuilder Format(object obj, StringBuilder toAppendTo, CultureInfo culture)
	{
		return toAppendTo.Append(Format(obj, culture));
	}

	public override object ParseObject(string source, int pos)
	{
		return long.Parse(source.Substring(pos), CultureInfo.InvariantCulture);
	}
}
