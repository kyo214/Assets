using NPOI.SS.Formula.PTG;

namespace NPOI.SS.Formula;

public interface IFormulaRenderingWorkbook
{
	ExternalSheet GetExternalSheet(int externSheetIndex);

	string GetSheetFirstNameByExternSheet(int externSheetIndex);

	string GetSheetLastNameByExternSheet(int externSheetIndex);

	string ResolveNameXText(NameXPtg nameXPtg);

	string GetNameText(NamePtg namePtg);
}
