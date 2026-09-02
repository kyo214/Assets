using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel.Extensions;

namespace NPOI.XSSF.UserModel;

public class XSSFOddHeader : XSSFHeaderFooter, IHeader, IHeaderFooter
{
	public override string Text
	{
		get
		{
			return GetHeaderFooter().oddHeader;
		}
		set
		{
			GetHeaderFooter().oddHeader = value;
		}
	}

	public XSSFOddHeader(CT_HeaderFooter headerFooter)
		: base(headerFooter)
	{
	}
}
