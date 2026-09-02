namespace NPOI.SS.Formula;

public interface WorkbookDependentFormula
{
	string ToFormulaString(IFormulaRenderingWorkbook book);
}
