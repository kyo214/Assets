namespace NPOI.SS.Formula;

public class ExternalSheet
{
	private string _workbookName;

	private string _sheetName;

	public string WorkbookName => _workbookName;

	public string SheetName => _sheetName;

	public ExternalSheet(string workbookName, string sheetName)
	{
		_workbookName = workbookName;
		_sheetName = sheetName;
	}
}
