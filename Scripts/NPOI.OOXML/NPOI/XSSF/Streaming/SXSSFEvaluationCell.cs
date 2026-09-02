using NPOI.SS.Formula;
using NPOI.SS.UserModel;

namespace NPOI.XSSF.Streaming;

public class SXSSFEvaluationCell : IEvaluationCell
{
	private SXSSFEvaluationSheet _evalSheet;

	private SXSSFCell _cell;

	public object IdentityKey => _cell;

	public bool BooleanCellValue => _cell.BooleanCellValue;

	public CellType CellType => _cell.CellType;

	public CellType CellTypeEnum => _cell.CellType;

	public int ColumnIndex => _cell.ColumnIndex;

	public int ErrorCellValue => _cell.ErrorCellValue;

	public double NumericCellValue => _cell.NumericCellValue;

	public int RowIndex => _cell.RowIndex;

	public IEvaluationSheet Sheet => _evalSheet;

	public string StringCellValue => _cell.RichStringCellValue.String;

	public CellType CachedFormulaResultType => _cell.CachedFormulaResultType;

	public SXSSFEvaluationCell(SXSSFCell cell, SXSSFEvaluationSheet evaluationSheet)
	{
		_cell = cell;
		_evalSheet = evaluationSheet;
	}

	public SXSSFEvaluationCell(SXSSFCell cell)
		: this(cell, new SXSSFEvaluationSheet(cell.Sheet as SXSSFSheet))
	{
	}

	public SXSSFCell GetSXSSFCell()
	{
		return _cell;
	}

	public CellType GetCachedFormulaResultTypeEnum()
	{
		return _cell.GetCachedFormulaResultTypeEnum();
	}
}
