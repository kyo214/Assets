namespace NPOI.SS.Formula.PTG;

public interface Pxg
{
	int ExternalWorkbookNumber { get; }

	string SheetName { get; set; }

	string ToFormulaString();
}
