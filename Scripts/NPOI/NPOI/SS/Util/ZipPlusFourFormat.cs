using System.Globalization;
using System.Text;

namespace NPOI.SS.Util;

public class ZipPlusFourFormat : FormatBase
{
	public static readonly FormatBase Instance = new ZipPlusFourFormat();

	private static string df = "000000000";

	private ZipPlusFourFormat()
	{
	}

	public override string Format(object obj, CultureInfo culture)
	{
		string text = ((double)obj).ToString(df, culture);
		return text.Substring(0, 5) + "-" + text.Substring(5, 4);
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
