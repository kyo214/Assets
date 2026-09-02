using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel.Extensions;

namespace NPOI.XSSF.UserModel;

public class XSSFEvenHeader : XSSFHeaderFooter, IHeader, IHeaderFooter
{
	public override string Text
	{
		get
		{
			return GetHeaderFooter().evenHeader;
		}
		set
		{
			GetHeaderFooter().evenHeader = value;
		}
	}

	public XSSFEvenHeader(CT_HeaderFooter headerFooter)
		: base(headerFooter)
	{
		headerFooter.differentOddEven = true;
	}
}
