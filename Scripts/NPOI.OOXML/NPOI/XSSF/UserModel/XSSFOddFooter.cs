using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel.Extensions;

namespace NPOI.XSSF.UserModel;

public class XSSFOddFooter : XSSFHeaderFooter, IFooter, IHeaderFooter
{
	public override string Text
	{
		get
		{
			return GetHeaderFooter().oddFooter;
		}
		set
		{
			GetHeaderFooter().oddFooter = value;
		}
	}

	public XSSFOddFooter(CT_HeaderFooter headerFooter)
		: base(headerFooter)
	{
	}
}
