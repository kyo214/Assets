using System;
using System.Collections;
using System.Collections.Generic;
using NPOI.HSSF.Model;
using NPOI.SS;
using NPOI.SS.Formula;

namespace NPOI.HSSF.Record.Aggregates;

public class RowRecordsAggregate : RecordAggregate
{
	private int firstrow = -1;

	private int lastrow = -1;

	private SortedList _rowRecords;

	private ValueRecordsAggregate _valuesAgg;

	private List<Record> _unknownRecords;

	private SharedValueManager _sharedValueManager;

	private RowRecord[] _rowRecordValues;

	public int PhysicalNumberOfRows => _rowRecords.Count;

	public int FirstRowNum => firstrow;

	public int LastRowNum => lastrow;

	public int RowBlockCount
	{
		get
		{
			int num = _rowRecords.Count / 32;
			if (_rowRecords.Count % 32 != 0)
			{
				num++;
			}
			return num;
		}
	}

	public RowRecordsAggregate()
		: this(SharedValueManager.CreateEmpty())
	{
	}

	private RowRecordsAggregate(SharedValueManager svm)
	{
		_rowRecords = new SortedList();
		_valuesAgg = new ValueRecordsAggregate();
		_unknownRecords = new List<Record>();
		_sharedValueManager = svm;
	}

	private int VisitRowRecordsForBlock(int blockIndex, RecordVisitor rv)
	{
		int num = blockIndex * 32;
		int num2 = num + 32;
		IEnumerator enumerator = _rowRecords.Values.GetEnumerator();
		int i;
		for (i = 0; i < num; i++)
		{
			if (!enumerator.MoveNext())
			{
				break;
			}
		}
		int num3 = 0;
		while (enumerator.MoveNext() && i++ < num2)
		{
			Record record = (Record)enumerator.Current;
			num3 += record.RecordSize;
			rv.VisitRecord(record);
		}
		return num3;
	}

	public override void VisitContainedRecords(RecordVisitor rv)
	{
		PositionTrackingVisitor positionTrackingVisitor = new PositionTrackingVisitor(rv, 0);
		int rowBlockCount = RowBlockCount;
		for (int i = 0; i < rowBlockCount; i++)
		{
			int num = 0;
			int num2 = VisitRowRecordsForBlock(i, rv);
			num += num2;
			int startRowNumberForBlock = GetStartRowNumberForBlock(i);
			int endRowNumberForBlock = GetEndRowNumberForBlock(i);
			DBCellRecord dBCellRecord = new DBCellRecord();
			int num3 = num2 - 20;
			for (int j = startRowNumberForBlock; j <= endRowNumberForBlock; j++)
			{
				if (_valuesAgg.RowHasCells(j))
				{
					positionTrackingVisitor.Position = 0;
					_valuesAgg.VisitCellsForRow(j, positionTrackingVisitor);
					int position = positionTrackingVisitor.Position;
					num += position;
					dBCellRecord.AddCellOffset((short)num3);
					num3 = position;
				}
			}
			dBCellRecord.RowOffset = num;
			rv.VisitRecord(dBCellRecord);
		}
		foreach (Record unknownRecord in _unknownRecords)
		{
			rv.VisitRecord(unknownRecord);
		}
	}

	public RowRecordsAggregate(RecordStream rs, SharedValueManager svm)
		: this(svm)
	{
		while (rs.HasNext())
		{
			Record next = rs.GetNext();
			switch (next.Sid)
			{
			case 520:
				InsertRow((RowRecord)next);
				continue;
			case 81:
				AddUnknownRecord(next);
				continue;
			case 215:
				continue;
			}
			if (next is UnknownRecord)
			{
				AddUnknownRecord((UnknownRecord)next);
				while (rs.PeekNextSid() == 60)
				{
					AddUnknownRecord(rs.GetNext());
				}
			}
			else if (next is MulBlankRecord)
			{
				_valuesAgg.AddMultipleBlanks((MulBlankRecord)next);
			}
			else if (!(next is CellValueRecordInterface))
			{
				if (next.Sid != 4197)
				{
					throw new InvalidOperationException("Unexpected record type (" + next.GetType().Name + ")");
				}
				AddUnknownRecord(next);
			}
			else
			{
				_valuesAgg.Construct((CellValueRecordInterface)next, rs, svm);
			}
		}
	}

	private void AddUnknownRecord(Record rec)
	{
		_unknownRecords.Add(rec);
	}

	public void InsertRow(RowRecord row)
	{
		_rowRecords[row.RowNumber] = row;
		_rowRecordValues = null;
		if (row.RowNumber < firstrow || firstrow == -1)
		{
			firstrow = row.RowNumber;
		}
		if (row.RowNumber > lastrow || lastrow == -1)
		{
			lastrow = row.RowNumber;
		}
	}

	public void RemoveRow(RowRecord row)
	{
		int rowNumber = row.RowNumber;
		_valuesAgg.RemoveAllCellsValuesForRow(rowNumber);
		int num = rowNumber;
		RowRecord rowRecord = (RowRecord)_rowRecords[num];
		_rowRecords.Remove(num);
		if (rowRecord == null)
		{
			throw new Exception("Invalid row index (" + num + ")");
		}
		if (row != rowRecord)
		{
			_rowRecords[num] = rowRecord;
			throw new Exception("Attempt to remove row that does not belong to this sheet");
		}
		_rowRecordValues = null;
	}

	public void InsertCell(CellValueRecordInterface cvRec)
	{
		_valuesAgg.InsertCell(cvRec);
	}

	public void RemoveCell(CellValueRecordInterface cvRec)
	{
		if (cvRec is FormulaRecordAggregate)
		{
			((FormulaRecordAggregate)cvRec).NotifyFormulaChanging();
		}
		_valuesAgg.RemoveCell(cvRec);
	}

	public RowRecord GetRow(int rowIndex)
	{
		int lastRowIndex = SpreadsheetVersion.EXCEL97.LastRowIndex;
		if (rowIndex < 0 || rowIndex > lastRowIndex)
		{
			throw new ArgumentException("The row number must be between 0 and " + lastRowIndex + ", but had: " + rowIndex);
		}
		return (RowRecord)_rowRecords[rowIndex];
	}

	public FormulaRecordAggregate CreateFormula(int row, int col)
	{
		return new FormulaRecordAggregate(new FormulaRecord
		{
			Row = row,
			Column = (short)col
		}, null, _sharedValueManager);
	}

	public int GetRowBlockSize(int block)
	{
		return 20 * GetRowCountForBlock(block);
	}

	public int GetRowCountForBlock(int block)
	{
		int num = block * 32;
		int num2 = num + 32 - 1;
		if (num2 >= _rowRecords.Count)
		{
			num2 = _rowRecords.Count - 1;
		}
		return num2 - num + 1;
	}

	public int GetStartRowNumberForBlock(int block)
	{
		int num = block * 32;
		if (_rowRecordValues == null)
		{
			_rowRecordValues = new RowRecord[_rowRecords.Count];
			_rowRecords.Values.CopyTo(_rowRecordValues, 0);
		}
		try
		{
			return _rowRecordValues[num].RowNumber;
		}
		catch (IndexOutOfRangeException)
		{
			throw new Exception("Did not find start row for block " + block);
		}
	}

	public int GetEndRowNumberForBlock(int block)
	{
		int num = (block + 1) * 32 - 1;
		if (num >= _rowRecords.Count)
		{
			num = _rowRecords.Count - 1;
		}
		if (_rowRecordValues == null)
		{
			_rowRecordValues = new RowRecord[_rowRecords.Count];
			_rowRecords.Values.CopyTo(_rowRecordValues, 0);
		}
		try
		{
			return _rowRecordValues[num].RowNumber;
		}
		catch (IndexOutOfRangeException)
		{
			throw new Exception("Did not find end row for block " + block);
		}
	}

	public IEnumerator GetEnumerator()
	{
		return _rowRecords.Values.GetEnumerator();
	}

	public int FindStartOfRowOutlineGroup(int row)
	{
		int outlineLevel = GetRow(row).OutlineLevel;
		int num = row;
		while (num >= 0 && GetRow(num) != null)
		{
			if (GetRow(num).OutlineLevel < outlineLevel)
			{
				return num + 1;
			}
			num--;
		}
		return num + 1;
	}

	public int FindEndOfRowOutlineGroup(int row)
	{
		int outlineLevel = GetRow(row).OutlineLevel;
		int i;
		for (i = row; i < LastRowNum && GetRow(i) != null && GetRow(i).OutlineLevel >= outlineLevel; i++)
		{
		}
		return i - 1;
	}

	public int WriteHidden(RowRecord rowRecord, int row, bool hidden)
	{
		int outlineLevel = rowRecord.OutlineLevel;
		while (rowRecord != null && GetRow(row).OutlineLevel >= outlineLevel)
		{
			rowRecord.ZeroHeight = hidden;
			row++;
			rowRecord = GetRow(row);
		}
		return row - 1;
	}

	public void CollapseRow(int rowNumber)
	{
		int num = FindStartOfRowOutlineGroup(rowNumber);
		RowRecord row = GetRow(num);
		int num2 = WriteHidden(row, num, hidden: true);
		if (GetRow(num2 + 1) != null)
		{
			GetRow(num2 + 1).Colapsed = true;
			return;
		}
		RowRecord rowRecord = CreateRow(num2 + 1);
		rowRecord.Colapsed = true;
		InsertRow(rowRecord);
	}

	public DimensionsRecord CreateDimensions()
	{
		return new DimensionsRecord
		{
			FirstRow = firstrow,
			LastRow = lastrow,
			FirstCol = _valuesAgg.FirstCellNum,
			LastCol = _valuesAgg.LastCellNum
		};
	}

	public static RowRecord CreateRow(int rowNumber)
	{
		return new RowRecord(rowNumber);
	}

	public IEnumerator<CellValueRecordInterface> GetCellValueEnumerator()
	{
		return _valuesAgg.GetEnumerator();
	}

	public IndexRecord CreateIndexRecord(int indexRecordOffset, int sizeOfInitialSheetRecords, int offsetDefaultColWidth)
	{
		IndexRecord indexRecord = new IndexRecord();
		indexRecord.FirstRow = firstrow;
		indexRecord.LastRowAdd1 = lastrow + 1;
		int rowBlockCount = RowBlockCount;
		int recordSizeForBlockCount = IndexRecord.GetRecordSizeForBlockCount(rowBlockCount);
		int num = indexRecordOffset + recordSizeForBlockCount + sizeOfInitialSheetRecords;
		for (int i = 0; i < rowBlockCount; i++)
		{
			num += GetRowBlockSize(i);
			num += _valuesAgg.GetRowCellBlockSize(GetStartRowNumberForBlock(i), GetEndRowNumberForBlock(i));
			indexRecord.AddDbcell(num);
			num += 8 + GetRowCountForBlock(i) * 2;
		}
		return indexRecord;
	}

	public bool IsRowGroupCollapsed(int row)
	{
		int rowIndex = FindEndOfRowOutlineGroup(row) + 1;
		if (GetRow(rowIndex) != null)
		{
			return GetRow(rowIndex).Colapsed;
		}
		return false;
	}

	public void ExpandRow(int rowNumber)
	{
		if (rowNumber == -1 || !IsRowGroupCollapsed(rowNumber))
		{
			return;
		}
		int num = FindStartOfRowOutlineGroup(rowNumber);
		RowRecord row = GetRow(num);
		int num2 = FindEndOfRowOutlineGroup(rowNumber);
		if (!IsRowGroupHiddenByParent(rowNumber))
		{
			for (int i = num; i <= num2; i++)
			{
				if (row.OutlineLevel == GetRow(i).OutlineLevel)
				{
					GetRow(i).ZeroHeight = false;
				}
				else if (!IsRowGroupCollapsed(i))
				{
					GetRow(i).ZeroHeight = false;
				}
			}
		}
		GetRow(num2 + 1).Colapsed = false;
	}

	public void UpdateFormulasAfterRowShift(FormulaShifter formulaShifter, int currentExternSheetIndex)
	{
		_valuesAgg.UpdateFormulasAfterRowShift(formulaShifter, currentExternSheetIndex);
	}

	public bool IsRowGroupHiddenByParent(int row)
	{
		int num = FindEndOfRowOutlineGroup(row);
		int num2;
		bool result;
		if (GetRow(num + 1) == null)
		{
			num2 = 0;
			result = false;
		}
		else
		{
			num2 = GetRow(num + 1).OutlineLevel;
			result = GetRow(num + 1).ZeroHeight;
		}
		int num3 = FindStartOfRowOutlineGroup(row);
		int num4;
		bool result2;
		if (num3 - 1 < 0 || GetRow(num3 - 1) == null)
		{
			num4 = 0;
			result2 = false;
		}
		else
		{
			num4 = GetRow(num3 - 1).OutlineLevel;
			result2 = GetRow(num3 - 1).ZeroHeight;
		}
		if (num2 > num4)
		{
			return result;
		}
		return result2;
	}
}
