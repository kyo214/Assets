using NPOI.SS.Formula.PTG;
using NPOI.SS.Formula.UDF;

namespace NPOI.SS.Formula;

public interface IEvaluationWorkbook
{
	string GetSheetName(int sheetIndex);

	int GetSheetIndex(IEvaluationSheet sheet);

	int GetSheetIndex(string sheetName);

	IEvaluationSheet GetSheet(int sheetIndex);

	ExternalSheet GetExternalSheet(int externSheetIndex);

	ExternalSheet GetExternalSheet(string firstSheetName, string lastSheetName, int externalWorkbookNumber);

	int ConvertFromExternSheetIndex(int externSheetIndex);

	ExternalName GetExternalName(int externSheetIndex, int externNameIndex);

	ExternalName GetExternalName(string nameName, string sheetName, int externalWorkbookNumber);

	IEvaluationName GetName(NamePtg namePtg);

	IEvaluationName GetName(string name, int sheetIndex);

	string ResolveNameXText(NameXPtg ptg);

	Ptg[] GetFormulaTokens(IEvaluationCell cell);

	UDFFinder GetUDFFinder();

	void ClearAllCachedResultValues();
}
