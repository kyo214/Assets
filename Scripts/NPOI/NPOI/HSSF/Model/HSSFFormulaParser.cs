using NPOI.HSSF.UserModel;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;

namespace NPOI.HSSF.Model;

public class HSSFFormulaParser
{
	private static IFormulaParsingWorkbook CreateParsingWorkbook(HSSFWorkbook book)
	{
		return HSSFEvaluationWorkbook.Create(book);
	}

	private HSSFFormulaParser()
	{
	}

	public static Ptg[] Parse(string formula, HSSFWorkbook workbook)
	{
		return Parse(formula, workbook, FormulaType.Cell);
	}

	public static Ptg[] Parse(string formula, HSSFWorkbook workbook, FormulaType formulaType)
	{
		return FormulaParser.Parse(formula, CreateParsingWorkbook(workbook), formulaType, -1);
	}

	public static Ptg[] Parse(string formula, HSSFWorkbook workbook, FormulaType formulaType, int sheetIndex)
	{
		return FormulaParser.Parse(formula, CreateParsingWorkbook(workbook), formulaType, sheetIndex, -1);
	}

	public static string ToFormulaString(HSSFWorkbook book, Ptg[] ptgs)
	{
		return FormulaRenderer.ToFormulaString(HSSFEvaluationWorkbook.Create(book), ptgs);
	}
}
