using System;
using System.Collections;
using System.Collections.Generic;
using NPOI.HSSF.Model;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;

namespace NPOI.HSSF.Record.Aggregates;

public class ValueRecordsAggregate : IEnumerable<CellValueRecordInterface>, IEnumerable
{
	private class ValueEnumerator : IEnumerator<CellValueRecordInterface>, IDisposable, IEnumerator
	{
		private short nextColumn = -1;

		private int nextRow;

		private int lastRow;

		private CellValueRecordInterface[][] records;

		public CellValueRecordInterface Current => records[nextRow][nextColumn];

		object IEnumerator.Current => Current;

		public ValueEnumerator(ref CellValueRecordInterface[][] _records)
		{
			records = _records;
			nextRow = 0;
			lastRow = _records.Length - 1;
		}

		public ValueEnumerator(ref CellValueRecordInterface[][] _records, int firstRow, int lastRow)
		{
			records = _records;
			nextRow = firstRow;
			this.lastRow = lastRow;
		}

		public bool MoveNext()
		{
			FindNext();
			return nextRow <= lastRow;
		}

		public void Remove()
		{
			throw new InvalidOperationException("gibt's noch nicht");
		}

		private void FindNext()
		{
			nextColumn++;
			while (nextRow <= lastRow)
			{
				CellValueRecordInterface[] array = ((nextRow < records.Length) ? records[nextRow] : null);
				if (array == null)
				{
					nextColumn = 0;
				}
				else
				{
					while (nextColumn < array.Length)
					{
						if (array[nextColumn] != null)
						{
							return;
						}
						nextColumn++;
					}
					nextColumn = 0;
				}
				nextRow++;
			}
		}

		public void Reset()
		{
			nextColumn = -1;
			nextRow = 0;
		}

		public void Dispose()
		{
			records = null;
		}
	}

	private const int MAX_ROW_INDEX = 65535;

	private const int INDEX_NOT_SET = -1;

	public const short sid = -1001;

	private int firstcell = -1;

	private int lastcell = -1;

	private CellValueRecordInterface[][] records;

	public int PhysicalNumberOfCells
	{
		get
		{
			int num = 0;
			for (int i = 0; i < records.Length; i++)
			{
				CellValueRecordInterface[] array = records[i];
				if (array == null)
				{
					continue;
				}
				for (short num2 = 0; num2 < array.Length; num2++)
				{
					if (array[num2] != null)
					{
						num++;
					}
				}
			}
			return num;
		}
	}

	public int FirstCellNum => firstcell;

	public int LastCellNum => lastcell;

	public ValueRecordsAggregate()
		: this(-1, -1, new CellValueRecordInterface[30][])
	{
	}

	private ValueRecordsAggregate(int firstCellIx, int lastCellIx, CellValueRecordInterface[][] pRecords)
	{
		firstcell = firstCellIx;
		lastcell = lastCellIx;
		records = pRecords;
	}

	public void InsertCell(CellValueRecordInterface cell)
	{
		int column = cell.Column;
		int row = cell.Row;
		if (row >= records.Length)
		{
			CellValueRecordInterface[][] array = records;
			int num = array.Length * 2;
			if (num < row + 1)
			{
				num = row + 1;
			}
			records = new CellValueRecordInterface[num][];
			Array.Copy(array, 0, records, 0, array.Length);
		}
		object obj = records[row];
		if (obj == null)
		{
			int num2 = column + 1;
			if (num2 < 10)
			{
				num2 = 10;
			}
			obj = new CellValueRecordInterface[num2];
			records[row] = (CellValueRecordInterface[])obj;
		}
		CellValueRecordInterface[] array2 = (CellValueRecordInterface[])obj;
		if (column >= array2.Length)
		{
			CellValueRecordInterface[] array3 = array2;
			int num3 = array3.Length * 2;
			if (num3 < column + 1)
			{
				num3 = column + 1;
			}
			array2 = new CellValueRecordInterface[num3];
			Array.Copy(array3, 0, array2, 0, array3.Length);
			records[row] = array2;
		}
		array2[column] = cell;
		if (column < firstcell || firstcell == -1)
		{
			firstcell = column;
		}
		if (column > lastcell || lastcell == -1)
		{
			lastcell = column;
		}
	}

	public void RemoveCell(CellValueRecordInterface cell)
	{
		if (cell == null)
		{
			throw new ArgumentException("cell must not be null");
		}
		int row = cell.Row;
		if (row >= records.Length)
		{
			throw new Exception("cell row is out of range");
		}
		CellValueRecordInterface[] array = records[row];
		if (array == null)
		{
			throw new Exception("cell row is already empty");
		}
		int column = cell.Column;
		if (column >= array.Length)
		{
			throw new Exception("cell column is out of range");
		}
		array[column] = null;
	}

	public void RemoveAllCellsValuesForRow(int rowIndex)
	{
		if (rowIndex < 0 || rowIndex > 65535)
		{
			throw new ArgumentException("Specified rowIndex " + rowIndex + " is outside the allowable range (0.." + 65535 + ")");
		}
		if (rowIndex < records.Length)
		{
			records[rowIndex] = null;
		}
	}

	public void AddMultipleBlanks(MulBlankRecord mbr)
	{
		for (int i = 0; i < mbr.NumColumns; i++)
		{
			BlankRecord blankRecord = new BlankRecord();
			blankRecord.Column = i + mbr.FirstColumn;
			blankRecord.Row = mbr.Row;
			blankRecord.XFIndex = mbr.GetXFAt(i);
			InsertCell(blankRecord);
		}
	}

	private MulBlankRecord CreateMBR(CellValueRecordInterface[] cellValues, int startIx, int nBlank)
	{
		short[] array = new short[nBlank];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = ((BlankRecord)cellValues[startIx + i]).XFIndex;
		}
		return new MulBlankRecord(cellValues[startIx].Row, startIx, array);
	}

	public void Construct(CellValueRecordInterface rec, RecordStream rs, SharedValueManager sfh)
	{
		if (rec is FormulaRecord)
		{
			FormulaRecord formulaRec = (FormulaRecord)rec;
			StringRecord stringRecord = null;
			stringRecord = ((!(rs.PeekNextClass() == typeof(StringRecord))) ? null : ((StringRecord)rs.GetNext()));
			InsertCell(new FormulaRecordAggregate(formulaRec, stringRecord, sfh));
		}
		else
		{
			InsertCell(rec);
		}
	}

	private static void HandleMissingSharedFormulaRecord(FormulaRecord formula)
	{
	}

	public int GetRowCellBlockSize(int startRow, int endRow)
	{
		ValueEnumerator valueEnumerator = new ValueEnumerator(ref records, startRow, endRow);
		int num = 0;
		while (valueEnumerator.MoveNext())
		{
			CellValueRecordInterface current = valueEnumerator.Current;
			int row = current.Row;
			if (row > endRow)
			{
				break;
			}
			if (row >= startRow && row <= endRow)
			{
				num += ((RecordBase)current).RecordSize;
			}
		}
		return num;
	}

	public bool RowHasCells(int row)
	{
		if (row > records.Length - 1)
		{
			return false;
		}
		CellValueRecordInterface[] array = records[row];
		if (array == null)
		{
			return false;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null)
			{
				return true;
			}
		}
		return false;
	}

	public void UpdateFormulasAfterRowShift(FormulaShifter shifter, int currentExternSheetIndex)
	{
		for (int i = 0; i < records.Length; i++)
		{
			CellValueRecordInterface[] array = records[i];
			if (array == null)
			{
				continue;
			}
			foreach (CellValueRecordInterface cellValueRecordInterface in array)
			{
				if (cellValueRecordInterface is FormulaRecordAggregate)
				{
					FormulaRecordAggregate formulaRecordAggregate = (FormulaRecordAggregate)cellValueRecordInterface;
					Ptg[] formulaTokens = formulaRecordAggregate.FormulaTokens;
					_ = ((FormulaRecordAggregate)cellValueRecordInterface).FormulaRecord.ParsedExpression;
					if (shifter.AdjustFormula(formulaTokens, currentExternSheetIndex))
					{
						formulaRecordAggregate.SetParsedExpression(formulaTokens);
					}
				}
			}
		}
	}

	public void VisitCellsForRow(int rowIndex, RecordVisitor rv)
	{
		CellValueRecordInterface[] array = records[rowIndex];
		if (array == null)
		{
			throw new ArgumentException("Row [" + rowIndex + "] is empty");
		}
		for (int i = 0; i < array.Length; i++)
		{
			RecordBase recordBase = (RecordBase)array[i];
			if (recordBase != null)
			{
				int num = CountBlanks(array, i);
				if (num > 1)
				{
					rv.VisitRecord(CreateMBR(array, i, num));
					i += num - 1;
				}
				else if (recordBase is RecordAggregate)
				{
					((RecordAggregate)recordBase).VisitContainedRecords(rv);
				}
				else
				{
					rv.VisitRecord((Record)recordBase);
				}
			}
		}
	}

	private static int CountBlanks(CellValueRecordInterface[] rowCellValues, int startIx)
	{
		int i;
		for (i = startIx; i < rowCellValues.Length && rowCellValues[i] is BlankRecord; i++)
		{
		}
		return i - startIx;
	}

	public int SerializeCellRow(int row, int offset, byte[] data)
	{
		ValueEnumerator valueEnumerator = new ValueEnumerator(ref records, row, row);
		int num = offset;
		while (valueEnumerator.MoveNext())
		{
			CellValueRecordInterface current = valueEnumerator.Current;
			if (current.Row != row)
			{
				break;
			}
			num += ((RecordBase)current).Serialize(num, data);
		}
		return num - offset;
	}

	public IEnumerator<CellValueRecordInterface> GetEnumerator()
	{
		return new ValueEnumerator(ref records);
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}
}
