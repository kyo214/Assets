using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.SS.Formula;

public interface IFormulaParsingWorkbook
{
	IEvaluationName GetName(string name, int sheetIndex);

	IName CreateName();

	ITable GetTable(string name);

	Ptg GetNameXPtg(string name, SheetIdentifier sheet);

	Ptg Get3DReferencePtg(CellReference cell, SheetIdentifier sheet);

	Ptg Get3DReferencePtg(AreaReference area, SheetIdentifier sheet);

	int GetExternalSheetIndex(string sheetName);

	int GetExternalSheetIndex(string workbookName, string sheetName);

	SpreadsheetVersion GetSpreadsheetVersion();
}
