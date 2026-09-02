using NPOI.SS.UserModel;

namespace NPOI.SS.Util.CellWalk;

public class CellWalk
{
	private class SimpleCellWalkContext : ICellWalkContext
	{
		public long ordinalNumber;

		public int rowNumber;

		public int colNumber;

		public long OrdinalNumber => ordinalNumber;

		public int RowNumber => rowNumber;

		public int ColumnNumber => colNumber;
	}

	private ISheet sheet;

	private CellRangeAddress range;

	private bool traverseEmptyCells;

	public CellWalk(ISheet sheet, CellRangeAddress range)
	{
		this.sheet = sheet;
		this.range = range;
		traverseEmptyCells = false;
	}

	public bool IsTraverseEmptyCells()
	{
		return traverseEmptyCells;
	}

	public void SetTraverseEmptyCells(bool traverseEmptyCells)
	{
		this.traverseEmptyCells = traverseEmptyCells;
	}

	public void Traverse(ICellHandler handler)
	{
		int firstRow = range.FirstRow;
		int lastRow = range.LastRow;
		int firstColumn = range.FirstColumn;
		int lastColumn = range.LastColumn;
		int num = lastColumn - firstColumn + 1;
		SimpleCellWalkContext simpleCellWalkContext = new SimpleCellWalkContext();
		IRow row = null;
		ICell cell = null;
		simpleCellWalkContext.rowNumber = firstRow;
		while (simpleCellWalkContext.rowNumber <= lastRow)
		{
			row = sheet.GetRow(simpleCellWalkContext.rowNumber);
			if (row != null)
			{
				simpleCellWalkContext.colNumber = firstColumn;
				while (simpleCellWalkContext.colNumber <= lastColumn)
				{
					cell = row.GetCell(simpleCellWalkContext.colNumber);
					if (cell != null && (!IsEmpty(cell) || traverseEmptyCells))
					{
						simpleCellWalkContext.ordinalNumber = (simpleCellWalkContext.rowNumber - firstRow) * num + (simpleCellWalkContext.colNumber - firstColumn + 1);
						handler.OnCell(cell, simpleCellWalkContext);
					}
					simpleCellWalkContext.colNumber++;
				}
			}
			simpleCellWalkContext.rowNumber++;
		}
	}

	private bool IsEmpty(ICell cell)
	{
		return cell.CellType == CellType.Blank;
	}
}
