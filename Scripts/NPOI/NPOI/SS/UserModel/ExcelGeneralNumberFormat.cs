using System;
using System.Globalization;
using System.Text;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.SS.UserModel;

public class ExcelGeneralNumberFormat : FormatBase
{
	private static long serialVersionUID = 1L;

	private NumberFormatInfo decimalSymbols;

	private DecimalFormat integerFormat;

	private DecimalFormat decimalFormat;

	private DecimalFormat scientificFormat;

	private CultureInfo culture;

	public ExcelGeneralNumberFormat(CultureInfo culture)
	{
		decimalSymbols = culture.NumberFormat;
		scientificFormat = new DecimalFormat("0.#####E0", decimalSymbols);
		integerFormat = new DecimalFormat("#", decimalSymbols);
		decimalFormat = new DecimalFormat("#.##########", decimalSymbols);
		this.culture = culture;
	}

	protected override StringBuilder Format(object obj, StringBuilder toAppendTo, int pos)
	{
		return Format(obj, toAppendTo, culture);
	}

	public override StringBuilder Format(object number, StringBuilder toAppendTo, CultureInfo culture)
	{
		if (Number.IsNumber(number))
		{
			double num = double.Parse(number.ToString());
			if (double.IsInfinity(num) || double.IsNaN(num))
			{
				return integerFormat.Format(number, toAppendTo, culture);
			}
			double num2 = Math.Abs(num);
			if (num2 >= 100000000000.0 || (num2 <= 1E-10 && num2 > 0.0))
			{
				return scientificFormat.Format(number, toAppendTo, culture);
			}
			if (Math.Floor(num) == num || num2 >= 10000000000.0)
			{
				return integerFormat.Format(number, toAppendTo, culture);
			}
			int num3 = 10;
			if (Math.Abs(num) > 1.0)
			{
				int num4 = (int)Math.Log10((int)Math.Abs(num)) + 1;
				num3 -= num4;
			}
			double num5 = Math.Round(num, num3, MidpointRounding.AwayFromZero);
			return decimalFormat.Format(num5, toAppendTo, culture);
		}
		return integerFormat.Format(number, toAppendTo, culture);
	}

	public override object ParseObject(string source, int pos)
	{
		throw new InvalidOperationException();
	}
}
