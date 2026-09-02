using System.Text;

namespace NPOI.SS.Formula.PTG;

public class ExternSheetNameResolver
{
	public static string PrependSheetName(IFormulaRenderingWorkbook book, int field_1_index_extern_sheet, string cellRefText)
	{
		ExternalSheet externalSheet = book.GetExternalSheet(field_1_index_extern_sheet);
		StringBuilder stringBuilder;
		if (externalSheet != null)
		{
			string workbookName = externalSheet.WorkbookName;
			string sheetName = externalSheet.SheetName;
			if (workbookName != null)
			{
				stringBuilder = new StringBuilder(workbookName.Length + sheetName.Length + cellRefText.Length + 4);
				SheetNameFormatter.AppendFormat(stringBuilder, workbookName, sheetName);
			}
			else
			{
				stringBuilder = new StringBuilder(sheetName.Length + cellRefText.Length + 4);
				SheetNameFormatter.AppendFormat(stringBuilder, sheetName);
			}
			if (externalSheet is ExternalSheetRange)
			{
				ExternalSheetRange externalSheetRange = (ExternalSheetRange)externalSheet;
				if (!externalSheetRange.FirstSheetName.Equals(externalSheetRange.LastSheetName))
				{
					stringBuilder.Append(':');
					SheetNameFormatter.AppendFormat(stringBuilder, externalSheetRange.LastSheetName);
				}
			}
		}
		else
		{
			string sheetFirstNameByExternSheet = book.GetSheetFirstNameByExternSheet(field_1_index_extern_sheet);
			string sheetLastNameByExternSheet = book.GetSheetLastNameByExternSheet(field_1_index_extern_sheet);
			stringBuilder = new StringBuilder(sheetFirstNameByExternSheet.Length + cellRefText.Length + 4);
			if (sheetFirstNameByExternSheet.Length < 1)
			{
				stringBuilder.Append("#REF");
			}
			else
			{
				SheetNameFormatter.AppendFormat(stringBuilder, sheetFirstNameByExternSheet);
				if (!sheetFirstNameByExternSheet.Equals(sheetLastNameByExternSheet))
				{
					stringBuilder.Append(':');
					stringBuilder.Append(sheetLastNameByExternSheet);
				}
			}
		}
		stringBuilder.Append('!');
		stringBuilder.Append(cellRefText);
		return stringBuilder.ToString();
	}
}
