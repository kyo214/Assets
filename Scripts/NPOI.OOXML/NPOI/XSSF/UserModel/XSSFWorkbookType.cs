namespace NPOI.XSSF.UserModel;

public class XSSFWorkbookType
{
	public static XSSFWorkbookType XLSX = new XSSFWorkbookType(XSSFRelation.WORKBOOK.ContentType, "xlsx");

	public static XSSFWorkbookType XLSM = new XSSFWorkbookType(XSSFRelation.MACROS_WORKBOOK.ContentType, "xlsm");

	private string _contentType;

	private string _extension;

	public string ContentType => _contentType;

	public string Extension => _extension;

	private XSSFWorkbookType(string contentType, string extension)
	{
		_contentType = contentType;
		_extension = extension;
	}
}
