using NPOI.SS.UserModel;

namespace NPOI.XSSF.UserModel;

public class XSSFAutoFilter : IAutoFilter
{
	private XSSFSheet _sheet;

	public XSSFAutoFilter(XSSFSheet sheet)
	{
		_sheet = sheet;
	}
}
