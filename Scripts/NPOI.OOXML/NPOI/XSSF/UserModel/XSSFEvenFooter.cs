using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel.Extensions;

namespace NPOI.XSSF.UserModel;

public class XSSFEvenFooter : XSSFHeaderFooter, IFooter, IHeaderFooter
{
	public override string Text
	{
		get
		{
			return GetHeaderFooter().evenFooter;
		}
		set
		{
			GetHeaderFooter().evenFooter = value;
		}
	}

	public XSSFEvenFooter(CT_HeaderFooter headerFooter)
		: base(headerFooter)
	{
		headerFooter.differentOddEven = true;
	}
}
