using System.Globalization;
using NPOI.SS.UserModel;

namespace NPOI.HSSF.UserModel;

public class HSSFDataFormatter : DataFormatter
{
	public HSSFDataFormatter(CultureInfo locale)
		: base(locale)
	{
	}

	public HSSFDataFormatter()
		: this(CultureInfo.CurrentCulture)
	{
	}
}
