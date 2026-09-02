using System.Globalization;
using System.Text;

namespace NPOI.SS.Util;

public class SSNFormat : FormatBase
{
	public static readonly FormatBase Instance = new SSNFormat();

	private static string df = "000000000";

	private SSNFormat()
	{
	}

	public override string Format(object obj, CultureInfo culture)
	{
		string text = ((double)obj).ToString(df, culture);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(text.Substring(0, 3)).Append('-');
		stringBuilder.Append(text.Substring(3, 2)).Append('-');
		stringBuilder.Append(text.Substring(5, 4));
		return stringBuilder.ToString();
	}

	protected override StringBuilder Format(object obj, StringBuilder toAppendTo, int pos)
	{
		return toAppendTo.Append(Format(obj, CultureInfo.CurrentCulture));
	}

	public override StringBuilder Format(object obj, StringBuilder toAppendTo, CultureInfo culture)
	{
		return toAppendTo.Append(Format((long)obj, culture));
	}

	public override object ParseObject(string source, int pos)
	{
		return long.Parse(source.Substring(pos), CultureInfo.InvariantCulture);
	}
}
