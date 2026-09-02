using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.XSSF.UserModel;

namespace NPOI.XSSF.Streaming;

public class SXSSFEvaluationWorkbook : BaseXSSFEvaluationWorkbook
{
	private SXSSFWorkbook _xBook;

	public static SXSSFEvaluationWorkbook Create(SXSSFWorkbook book)
	{
		if (book == null)
		{
			return null;
		}
		return new SXSSFEvaluationWorkbook(book);
	}

	private SXSSFEvaluationWorkbook(SXSSFWorkbook book)
		: base(book.XssfWorkbook)
	{
		_xBook = book;
	}

	public override int GetSheetIndex(IEvaluationSheet evalSheet)
	{
		SXSSFSheet sXSSFSheet = ((SXSSFEvaluationSheet)evalSheet).GetSXSSFSheet();
		return _xBook.GetSheetIndex(sXSSFSheet);
	}

	public override IEvaluationSheet GetSheet(int sheetIndex)
	{
		return new SXSSFEvaluationSheet(_xBook.GetSheetAt(sheetIndex) as SXSSFSheet);
	}

	public override Ptg[] GetFormulaTokens(IEvaluationCell evalCell)
	{
		SXSSFCell sXSSFCell = ((SXSSFEvaluationCell)evalCell).GetSXSSFCell();
		return FormulaParser.Parse(sXSSFCell.CellFormula, this, FormulaType.Cell, _xBook.GetSheetIndex(sXSSFCell.Sheet));
	}
}
