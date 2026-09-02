using System;
using System.Text;
using NPOI.SS.Formula.PTG;

namespace NPOI.SS.Formula;

public class FormulaShifter
{
	private enum ShiftMode
	{
		RowMove = 0,
		RowCopy = 1,
		SheetMove = 2
	}

	private int _externSheetIndex;

	private string _sheetName;

	private int _firstMovedIndex;

	private int _lastMovedIndex;

	private int _amountToMove;

	private int _srcSheetIndex;

	private int _dstSheetIndex;

	private SpreadsheetVersion _version;

	private ShiftMode _mode;

	private FormulaShifter(int externSheetIndex, string sheetName, int firstMovedIndex, int lastMovedIndex, int amountToMove, ShiftMode mode, SpreadsheetVersion version)
	{
		if (amountToMove == 0)
		{
			throw new ArgumentException("amountToMove must not be zero");
		}
		if (firstMovedIndex > lastMovedIndex)
		{
			throw new ArgumentException("firstMovedIndex, lastMovedIndex out of order");
		}
		_externSheetIndex = externSheetIndex;
		_sheetName = sheetName;
		_firstMovedIndex = firstMovedIndex;
		_lastMovedIndex = lastMovedIndex;
		_amountToMove = amountToMove;
		_mode = mode;
		_version = version;
		_srcSheetIndex = (_dstSheetIndex = -1);
	}

	private FormulaShifter(int srcSheetIndex, int dstSheetIndex)
	{
		_externSheetIndex = (_firstMovedIndex = (_lastMovedIndex = (_amountToMove = -1)));
		_sheetName = null;
		_version = null;
		_srcSheetIndex = srcSheetIndex;
		_dstSheetIndex = dstSheetIndex;
		_mode = ShiftMode.SheetMove;
	}

	[Obsolete("deprecated As of 3.14 beta 1 (November 2015), replaced by CreateForRowShift(int, String, int, int, int, SpreadsheetVersion)")]
	public static FormulaShifter CreateForRowShift(int externSheetIndex, string sheetName, int firstMovedRowIndex, int lastMovedRowIndex, int numberOfRowsToMove)
	{
		return CreateForRowShift(externSheetIndex, sheetName, firstMovedRowIndex, lastMovedRowIndex, numberOfRowsToMove, SpreadsheetVersion.EXCEL97);
	}

	public static FormulaShifter CreateForRowShift(int externSheetIndex, string sheetName, int firstMovedRowIndex, int lastMovedRowIndex, int numberOfRowsToMove, SpreadsheetVersion version)
	{
		return new FormulaShifter(externSheetIndex, sheetName, firstMovedRowIndex, lastMovedRowIndex, numberOfRowsToMove, ShiftMode.RowMove, version);
	}

	public static FormulaShifter CreateForRowCopy(int externSheetIndex, string sheetName, int firstMovedRowIndex, int lastMovedRowIndex, int numberOfRowsToMove, SpreadsheetVersion version)
	{
		return new FormulaShifter(externSheetIndex, sheetName, firstMovedRowIndex, lastMovedRowIndex, numberOfRowsToMove, ShiftMode.RowCopy, version);
	}

	public static FormulaShifter CreateForSheetShift(int srcSheetIndex, int dstSheetIndex)
	{
		return new FormulaShifter(srcSheetIndex, dstSheetIndex);
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(GetType().Name);
		stringBuilder.Append(" [");
		stringBuilder.Append(_firstMovedIndex);
		stringBuilder.Append(_lastMovedIndex);
		stringBuilder.Append(_amountToMove);
		return stringBuilder.ToString();
	}

	public bool AdjustFormula(Ptg[] ptgs, int currentExternSheetIx)
	{
		bool result = false;
		for (int i = 0; i < ptgs.Length; i++)
		{
			Ptg ptg = AdjustPtg(ptgs[i], currentExternSheetIx);
			if (ptg != null)
			{
				result = true;
				ptgs[i] = ptg;
			}
		}
		return result;
	}

	private Ptg AdjustPtg(Ptg ptg, int currentExternSheetIx)
	{
		return _mode switch
		{
			ShiftMode.RowMove => AdjustPtgDueToRowMove(ptg, currentExternSheetIx), 
			ShiftMode.RowCopy => AdjustPtgDueToRowCopy(ptg), 
			ShiftMode.SheetMove => AdjustPtgDueToSheetMove(ptg), 
			_ => throw new InvalidOperationException("Unsupported shift mode: " + _mode), 
		};
	}

	private Ptg AdjustPtgDueToRowMove(Ptg ptg, int currentExternSheetIx)
	{
		if (ptg is RefPtg)
		{
			if (currentExternSheetIx != _externSheetIndex)
			{
				return null;
			}
			RefPtg rptg = (RefPtg)ptg;
			return RowMoveRefPtg(rptg);
		}
		if (ptg is Ref3DPtg)
		{
			Ref3DPtg ref3DPtg = (Ref3DPtg)ptg;
			if (_externSheetIndex != ref3DPtg.ExternSheetIndex)
			{
				return null;
			}
			return RowMoveRefPtg(ref3DPtg);
		}
		if (ptg is Ref3DPxg)
		{
			Ref3DPxg ref3DPxg = (Ref3DPxg)ptg;
			if (ref3DPxg.ExternalWorkbookNumber > 0 || !_sheetName.Equals(ref3DPxg.SheetName))
			{
				return null;
			}
			return RowMoveRefPtg(ref3DPxg);
		}
		if (ptg is Area2DPtgBase)
		{
			if (currentExternSheetIx != _externSheetIndex)
			{
				return ptg;
			}
			return RowMoveAreaPtg((Area2DPtgBase)ptg);
		}
		if (ptg is Area3DPtg)
		{
			Area3DPtg area3DPtg = (Area3DPtg)ptg;
			if (_externSheetIndex != area3DPtg.ExternSheetIndex)
			{
				return null;
			}
			return RowMoveAreaPtg(area3DPtg);
		}
		if (ptg is Area3DPxg)
		{
			Area3DPxg area3DPxg = (Area3DPxg)ptg;
			if (area3DPxg.ExternalWorkbookNumber > 0 || !_sheetName.Equals(area3DPxg.SheetName))
			{
				return null;
			}
			return RowMoveAreaPtg(area3DPxg);
		}
		return null;
	}

	private Ptg AdjustPtgDueToRowCopy(Ptg ptg)
	{
		if (ptg is RefPtg)
		{
			RefPtg rptg = (RefPtg)ptg;
			return RowCopyRefPtg(rptg);
		}
		if (ptg is Ref3DPtg)
		{
			Ref3DPtg rptg2 = (Ref3DPtg)ptg;
			return RowCopyRefPtg(rptg2);
		}
		if (ptg is Ref3DPxg)
		{
			Ref3DPxg rptg3 = (Ref3DPxg)ptg;
			return RowCopyRefPtg(rptg3);
		}
		if (ptg is Area2DPtgBase)
		{
			return RowCopyAreaPtg((Area2DPtgBase)ptg);
		}
		if (ptg is Area3DPtg)
		{
			Area3DPtg aptg = (Area3DPtg)ptg;
			return RowCopyAreaPtg(aptg);
		}
		if (ptg is Area3DPxg)
		{
			Area3DPxg aptg2 = (Area3DPxg)ptg;
			return RowCopyAreaPtg(aptg2);
		}
		return null;
	}

	private Ptg AdjustPtgDueToSheetMove(Ptg ptg)
	{
		if (ptg is Ref3DPtg)
		{
			Ref3DPtg ref3DPtg = (Ref3DPtg)ptg;
			int externSheetIndex = ref3DPtg.ExternSheetIndex;
			if (externSheetIndex < _srcSheetIndex && externSheetIndex < _dstSheetIndex)
			{
				return null;
			}
			if (externSheetIndex > _srcSheetIndex && externSheetIndex > _dstSheetIndex)
			{
				return null;
			}
			if (externSheetIndex == _srcSheetIndex)
			{
				ref3DPtg.ExternSheetIndex = _dstSheetIndex;
				return ref3DPtg;
			}
			if (_dstSheetIndex < _srcSheetIndex)
			{
				ref3DPtg.ExternSheetIndex = externSheetIndex + 1;
				return ref3DPtg;
			}
			if (_dstSheetIndex > _srcSheetIndex)
			{
				ref3DPtg.ExternSheetIndex = externSheetIndex - 1;
				return ref3DPtg;
			}
		}
		return null;
	}

	private Ptg RowMoveRefPtg(RefPtgBase rptg)
	{
		int row = rptg.Row;
		if (_firstMovedIndex <= row && row <= _lastMovedIndex)
		{
			rptg.Row = row + _amountToMove;
			return rptg;
		}
		int num = _firstMovedIndex + _amountToMove;
		int num2 = _lastMovedIndex + _amountToMove;
		if (num2 < row || row < num)
		{
			return null;
		}
		if (num <= row && row <= num2)
		{
			return CreateDeletedRef(rptg);
		}
		throw new InvalidOperationException("Situation not covered: (" + _firstMovedIndex + ", " + _lastMovedIndex + ", " + _amountToMove + ", " + row + ", " + row + ")");
	}

	private Ptg RowMoveAreaPtg(AreaPtgBase aptg)
	{
		int firstRow = aptg.FirstRow;
		int lastRow = aptg.LastRow;
		if (_firstMovedIndex <= firstRow && lastRow <= _lastMovedIndex)
		{
			aptg.FirstRow = firstRow + _amountToMove;
			aptg.LastRow = lastRow + _amountToMove;
			return aptg;
		}
		int num = _firstMovedIndex + _amountToMove;
		int num2 = _lastMovedIndex + _amountToMove;
		if (firstRow < _firstMovedIndex && _lastMovedIndex < lastRow)
		{
			if (num < firstRow && firstRow <= num2)
			{
				aptg.FirstRow = num2 + 1;
				return aptg;
			}
			if (num <= lastRow && lastRow < num2)
			{
				aptg.LastRow = num - 1;
				return aptg;
			}
			return null;
		}
		if (_firstMovedIndex <= firstRow && firstRow <= _lastMovedIndex)
		{
			if (_amountToMove < 0)
			{
				aptg.FirstRow = firstRow + _amountToMove;
				return aptg;
			}
			if (num > lastRow)
			{
				return null;
			}
			int firstRow2 = firstRow + _amountToMove;
			if (num2 < lastRow)
			{
				aptg.FirstRow = firstRow2;
				return aptg;
			}
			int num3 = _lastMovedIndex + 1;
			if (num > num3)
			{
				firstRow2 = num3;
			}
			aptg.FirstRow = firstRow2;
			aptg.LastRow = Math.Max(lastRow, num2);
			return aptg;
		}
		if (_firstMovedIndex <= lastRow && lastRow <= _lastMovedIndex)
		{
			if (_amountToMove > 0)
			{
				aptg.LastRow = lastRow + _amountToMove;
				return aptg;
			}
			if (num2 < firstRow)
			{
				return null;
			}
			int lastRow2 = lastRow + _amountToMove;
			if (num > firstRow)
			{
				aptg.LastRow = lastRow2;
				return aptg;
			}
			int num4 = _firstMovedIndex - 1;
			if (num2 < num4)
			{
				lastRow2 = num4;
			}
			aptg.FirstRow = Math.Min(firstRow, num);
			aptg.LastRow = lastRow2;
			return aptg;
		}
		if (num2 < firstRow || lastRow < num)
		{
			return null;
		}
		if (num <= firstRow && lastRow <= num2)
		{
			return CreateDeletedRef(aptg);
		}
		if (firstRow <= num && num2 <= lastRow)
		{
			return null;
		}
		if (num < firstRow && firstRow <= num2)
		{
			aptg.FirstRow = num2 + 1;
			return aptg;
		}
		if (num <= lastRow && lastRow < num2)
		{
			aptg.LastRow = num - 1;
			return aptg;
		}
		throw new InvalidOperationException("Situation not covered: (" + _firstMovedIndex + ", " + _lastMovedIndex + ", " + _amountToMove + ", " + firstRow + ", " + lastRow + ")");
	}

	private Ptg RowCopyRefPtg(RefPtgBase rptg)
	{
		int row = rptg.Row;
		if (rptg.IsRowRelative)
		{
			int num = _firstMovedIndex + _amountToMove;
			if (num < 0 || _version.LastRowIndex < num)
			{
				return CreateDeletedRef(rptg);
			}
			rptg.Row = row + _amountToMove;
			return rptg;
		}
		return null;
	}

	private Ptg RowCopyAreaPtg(AreaPtgBase aptg)
	{
		bool flag = false;
		int firstRow = aptg.FirstRow;
		int lastRow = aptg.LastRow;
		if (aptg.IsFirstRowRelative)
		{
			int num = firstRow + _amountToMove;
			if (num < 0 || _version.LastRowIndex < num)
			{
				return CreateDeletedRef(aptg);
			}
			aptg.FirstRow = num;
			flag = true;
		}
		if (aptg.IsLastRowRelative)
		{
			int num2 = lastRow + _amountToMove;
			if (num2 < 0 || _version.LastRowIndex < num2)
			{
				return CreateDeletedRef(aptg);
			}
			aptg.LastRow = num2;
			flag = true;
		}
		if (flag)
		{
			aptg.SortTopLeftToBottomRight();
		}
		if (!flag)
		{
			return null;
		}
		return aptg;
	}

	private static Ptg CreateDeletedRef(Ptg ptg)
	{
		if (ptg is RefPtg)
		{
			return new RefErrorPtg();
		}
		if (ptg is Ref3DPtg)
		{
			return new DeletedRef3DPtg(((Ref3DPtg)ptg).ExternSheetIndex);
		}
		if (ptg is AreaPtg)
		{
			return new AreaErrPtg();
		}
		if (ptg is Area3DPtg)
		{
			return new DeletedArea3DPtg(((Area3DPtg)ptg).ExternSheetIndex);
		}
		if (ptg is Ref3DPxg)
		{
			Ref3DPxg ref3DPxg = (Ref3DPxg)ptg;
			return new Deleted3DPxg(ref3DPxg.ExternalWorkbookNumber, ref3DPxg.SheetName);
		}
		if (ptg is Area3DPxg)
		{
			Area3DPxg area3DPxg = (Area3DPxg)ptg;
			return new Deleted3DPxg(area3DPxg.ExternalWorkbookNumber, area3DPxg.SheetName);
		}
		throw new ArgumentException("Unexpected ref ptg class (" + ptg.GetType().Name + ")");
	}
}
