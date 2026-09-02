using NPOI.SS.UserModel;

namespace NPOI.SS.Formula;

public interface IEvaluationCell
{
	IEvaluationSheet Sheet { get; }

	int RowIndex { get; }

	int ColumnIndex { get; }

	CellType CellType { get; }

	double NumericCellValue { get; }

	string StringCellValue { get; }

	bool BooleanCellValue { get; }

	int ErrorCellValue { get; }

	object IdentityKey { get; }

	CellType CachedFormulaResultType { get; }
}
