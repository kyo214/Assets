using System.Globalization;
using System.Text;

namespace NPOI.SS.Format;

public abstract class CellFormatter
{
	protected string format;

	public static readonly CultureInfo LOCALE = CultureInfo.GetCultureInfo("en-US");

	public CellFormatter(string format)
	{
		this.format = format;
	}

	public abstract void FormatValue(StringBuilder toAppendTo, object value);

	public abstract void SimpleValue(StringBuilder toAppendTo, object value);

	public string Format(object value)
	{
		StringBuilder stringBuilder = new StringBuilder();
		FormatValue(stringBuilder, value);
		return stringBuilder.ToString();
	}

	public string SimpleFormat(object value)
	{
		StringBuilder stringBuilder = new StringBuilder();
		SimpleValue(stringBuilder, value);
		return stringBuilder.ToString();
	}

	private static string Quote(string str)
	{
		return "\"" + str + "\"";
	}

	public override string ToString()
	{
		return format;
	}
}
