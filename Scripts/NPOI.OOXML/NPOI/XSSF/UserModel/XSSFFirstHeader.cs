using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel.Extensions;

namespace NPOI.XSSF.UserModel;

public class XSSFFirstHeader : XSSFHeaderFooter, IHeader, IHeaderFooter
{
	public override string Text
	{
		get
		{
			return GetHeaderFooter().firstHeader;
		}
		set
		{
			if (value == null)
			{
				GetHeaderFooter().firstHeader = null;
			}
			else
			{
				GetHeaderFooter().firstHeader = value;
			}
		}
	}

	public XSSFFirstHeader(CT_HeaderFooter headerFooter)
		: base(headerFooter)
	{
		headerFooter.differentFirst = true;
	}
}
