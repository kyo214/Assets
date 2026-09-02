using NPOI.SS.Formula;
using NPOI.SS.UserModel;

namespace NPOI.XSSF.UserModel;

public class XSSFEvaluationCell : IEvaluationCell
{
	private IEvaluationSheet _evalSheet;

	private XSSFCell _cell;

	public object IdentityKey => _cell;

	public virtual bool BooleanCellValue => _cell.BooleanCellValue;

	public virtual CellType CellType => _cell.CellType;

	public virtual int ColumnIndex => _cell.ColumnIndex;

	public virtual int ErrorCellValue => _cell.ErrorCellValue;

	public virtual double NumericCellValue => _cell.NumericCellValue;

	public virtual int RowIndex => _cell.RowIndex;

	public virtual IEvaluationSheet Sheet => _evalSheet;

	public virtual string StringCellValue => _cell.RichStringCellValue.String;

	public virtual CellType CachedFormulaResultType => _cell.CachedFormulaResultType;

	public XSSFEvaluationCell(ICell cell, XSSFEvaluationSheet EvaluationSheet)
	{
		_cell = (XSSFCell)cell;
		_evalSheet = EvaluationSheet;
	}

	public XSSFEvaluationCell(ICell cell)
		: this(cell, new XSSFEvaluationSheet(cell.Sheet))
	{
	}

	public XSSFEvaluationCell()
	{
	}

	public XSSFCell GetXSSFCell()
	{
		return _cell;
	}
}
