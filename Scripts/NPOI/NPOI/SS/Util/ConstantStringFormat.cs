using System.Globalization;
using System.Text;

namespace NPOI.SS.Util;

public class ConstantStringFormat : FormatBase
{
	private static DecimalFormat df = new DecimalFormat("##########");

	private string str;

	public ConstantStringFormat(string s)
	{
		str = s;
	}

	public override string Format(object obj)
	{
		return str;
	}

	public override string Format(object obj, CultureInfo culture)
	{
		return str;
	}

	public override StringBuilder Format(object obj, StringBuilder toAppendTo, CultureInfo culture)
	{
		return toAppendTo.Append(str);
	}

	public override object ParseObject(string source, int pos)
	{
		return df.ParseObject(source, pos);
	}
}
