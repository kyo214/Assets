using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;

namespace NPOI.XSSF.UserModel;

public class XSSFEvaluationWorkbook : BaseXSSFEvaluationWorkbook
{
	private XSSFEvaluationSheet[] _sheetCache;

	public static XSSFEvaluationWorkbook Create(IWorkbook book)
	{
		if (book == null)
		{
			return null;
		}
		return new XSSFEvaluationWorkbook(book as XSSFWorkbook);
	}

	protected XSSFEvaluationWorkbook(XSSFWorkbook book)
		: base(book)
	{
	}

	public override void ClearAllCachedResultValues()
	{
		base.ClearAllCachedResultValues();
		_sheetCache = null;
	}

	public override int GetSheetIndex(IEvaluationSheet evalSheet)
	{
		XSSFSheet xSSFSheet = ((XSSFEvaluationSheet)evalSheet).GetXSSFSheet();
		return _uBook.GetSheetIndex(xSSFSheet);
	}

	public override IEvaluationSheet GetSheet(int sheetIndex)
	{
		if (_sheetCache == null)
		{
			int numberOfSheets = _uBook.NumberOfSheets;
			_sheetCache = new XSSFEvaluationSheet[numberOfSheets];
			for (int i = 0; i < numberOfSheets; i++)
			{
				_sheetCache[i] = new XSSFEvaluationSheet(_uBook.GetSheetAt(i));
			}
		}
		if (sheetIndex < 0 || sheetIndex >= _sheetCache.Length)
		{
			_uBook.GetSheetAt(sheetIndex);
		}
		return _sheetCache[sheetIndex];
	}

	public override Ptg[] GetFormulaTokens(IEvaluationCell evalCell)
	{
		XSSFCell xSSFCell = ((XSSFEvaluationCell)evalCell).GetXSSFCell();
		int sheetIndex = _uBook.GetSheetIndex(xSSFCell.Sheet);
		int rowIndex = xSSFCell.RowIndex;
		return FormulaParser.Parse(xSSFCell.GetCellFormula(this), this, FormulaType.Cell, sheetIndex, rowIndex);
	}
}
