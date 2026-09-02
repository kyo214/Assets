namespace NPOI.SS.Util.CellWalk;

public interface ICellWalkContext
{
	long OrdinalNumber { get; }

	int RowNumber { get; }

	int ColumnNumber { get; }
}
