using System.Collections;
using System.Collections.Generic;

namespace NPOI.SS.UserModel;

public interface IRow : IEnumerable<ICell>, IEnumerable
{
	int RowNum { get; set; }

	short FirstCellNum { get; }

	short LastCellNum { get; }

	int PhysicalNumberOfCells { get; }

	bool ZeroHeight { get; set; }

	short Height { get; set; }

	float HeightInPoints { get; set; }

	bool IsFormatted { get; }

	ISheet Sheet { get; }

	ICellStyle RowStyle { get; set; }

	List<ICell> Cells { get; }

	int OutlineLevel { get; }

	bool? Hidden { get; set; }

	bool? Collapsed { get; set; }

	ICell CreateCell(int column);

	ICell CreateCell(int column, CellType type);

	void RemoveCell(ICell cell);

	ICell GetCell(int cellnum);

	ICell GetCell(int cellnum, MissingCellPolicy policy);

	void MoveCell(ICell cell, int newColumn);

	IRow CopyRowTo(int targetIndex);

	ICell CopyCell(int sourceIndex, int targetIndex);

	bool HasCustomHeight();
}
