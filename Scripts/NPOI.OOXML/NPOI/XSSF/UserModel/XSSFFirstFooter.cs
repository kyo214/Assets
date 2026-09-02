using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel.Extensions;

namespace NPOI.XSSF.UserModel;

public class XSSFFirstFooter : XSSFHeaderFooter, IFooter, IHeaderFooter
{
	public override string Text
	{
		get
		{
			return GetHeaderFooter().firstFooter;
		}
		set
		{
			if (value == null)
			{
				GetHeaderFooter().firstFooter = null;
			}
			else
			{
				GetHeaderFooter().firstFooter = value;
			}
		}
	}

	public XSSFFirstFooter(CT_HeaderFooter headerFooter)
		: base(headerFooter)
	{
		headerFooter.differentFirst = true;
	}
}
