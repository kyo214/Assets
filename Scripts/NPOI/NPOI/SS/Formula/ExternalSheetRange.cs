namespace NPOI.SS.Formula;

public class ExternalSheetRange : ExternalSheet
{
	private string _lastSheetName;

	public string FirstSheetName => base.SheetName;

	public string LastSheetName => _lastSheetName;

	public ExternalSheetRange(string workbookName, string firstSheetName, string lastSheetName)
		: base(workbookName, firstSheetName)
	{
		_lastSheetName = lastSheetName;
	}
}
