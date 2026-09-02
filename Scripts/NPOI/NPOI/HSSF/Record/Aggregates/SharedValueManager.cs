using System;
using System.Collections.Generic;
using System.Text;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.HSSF.Record.Aggregates;

[Serializable]
public class SharedValueManager
{
	private class SharedFormulaGroup
	{
		private SharedFormulaRecord _sfr;

		private FormulaRecordAggregate[] _frAggs;

		private int _numberOfFormulas;

		private CellReference _firstCell;

		internal CellReference FirstCell => _firstCell;

		public SharedFormulaRecord SFR => _sfr;

		public SharedFormulaGroup(SharedFormulaRecord sfr, CellReference firstCell)
		{
			if (!sfr.IsInRange(firstCell.Row, firstCell.Col))
			{
				throw new ArgumentException("First formula cell " + firstCell.FormatAsString() + " is not shared formula range " + sfr.Range.ToString() + ".");
			}
			_sfr = sfr;
			_firstCell = firstCell;
			int num = sfr.LastColumn - sfr.FirstColumn + 1;
			int num2 = sfr.LastRow - sfr.FirstRow + 1;
			_frAggs = new FormulaRecordAggregate[num * num2];
			_numberOfFormulas = 0;
		}

		public void Add(FormulaRecordAggregate agg)
		{
			if (_numberOfFormulas == 0 && (_firstCell.Row != agg.Row || _firstCell.Col != agg.Column))
			{
				throw new InvalidOperationException("shared formula coding error");
			}
			if (_numberOfFormulas >= _frAggs.Length)
			{
				throw new Exception("Too many formula records for shared formula group");
			}
			_frAggs[_numberOfFormulas++] = agg;
		}

		public void UnlinkSharedFormulas()
		{
			for (int i = 0; i < _numberOfFormulas; i++)
			{
				_frAggs[i].UnlinkSharedFormula();
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.Append(GetType().Name).Append(" [");
			stringBuilder.Append(_sfr.Range.ToString());
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public bool IsFirstCell(int row, int column)
		{
			if (_firstCell.Row == row)
			{
				return _firstCell.Col == column;
			}
			return false;
		}
	}

	private class SharedFormulaGroupComparator : Comparer<SharedFormulaGroup>
	{
		public override int Compare(SharedFormulaGroup a, SharedFormulaGroup b)
		{
			CellRangeAddress8Bit range = a.SFR.Range;
			CellRangeAddress8Bit range2 = b.SFR.Range;
			int num = range.FirstRow - range2.FirstRow;
			if (num != 0)
			{
				return num;
			}
			num = range.FirstColumn - range2.FirstColumn;
			if (num != 0)
			{
				return num;
			}
			return 0;
		}
	}

	public static readonly SharedValueManager EMPTY = new SharedValueManager(new SharedFormulaRecord[0], new CellReference[0], new List<ArrayRecord>(), new List<TableRecord>());

	private List<ArrayRecord> _arrayRecords;

	private List<TableRecord> _tableRecords;

	private Dictionary<SharedFormulaRecord, SharedFormulaGroup> _groupsBySharedFormulaRecord;

	[NonSerialized]
	private Dictionary<int, SharedFormulaGroup> _groupsCache;

	[NonSerialized]
	private SharedFormulaGroupComparator SVGComparator = new SharedFormulaGroupComparator();

	private SharedValueManager(SharedFormulaRecord[] sharedFormulaRecords, CellReference[] firstCells, List<ArrayRecord> arrayRecords, List<TableRecord> tableRecords)
	{
		int num = sharedFormulaRecords.Length;
		if (num != firstCells.Length)
		{
			throw new ArgumentException("array sizes don't match: " + num + "!=" + firstCells.Length + ".");
		}
		_arrayRecords = new List<ArrayRecord>();
		_arrayRecords.AddRange(arrayRecords);
		_tableRecords = tableRecords;
		Dictionary<SharedFormulaRecord, SharedFormulaGroup> dictionary = new Dictionary<SharedFormulaRecord, SharedFormulaGroup>(num * 3 / 2);
		for (int i = 0; i < num; i++)
		{
			SharedFormulaRecord sharedFormulaRecord = sharedFormulaRecords[i];
			dictionary[sharedFormulaRecord] = new SharedFormulaGroup(sharedFormulaRecord, firstCells[i]);
		}
		_groupsBySharedFormulaRecord = dictionary;
	}

	public static SharedValueManager CreateEmpty()
	{
		return new SharedValueManager(new SharedFormulaRecord[0], new CellReference[0], new List<ArrayRecord>(), new List<TableRecord>());
	}

	public static SharedValueManager Create(SharedFormulaRecord[] sharedFormulaRecords, CellReference[] firstCells, List<ArrayRecord> arrayRecords, List<TableRecord> tableRecords)
	{
		if (sharedFormulaRecords.Length + firstCells.Length + arrayRecords.Count + tableRecords.Count < 1)
		{
			return EMPTY;
		}
		return new SharedValueManager(sharedFormulaRecords, firstCells, arrayRecords, tableRecords);
	}

	public SharedFormulaRecord LinkSharedFormulaRecord(CellReference firstCell, FormulaRecordAggregate agg)
	{
		SharedFormulaGroup sharedFormulaGroup = FindFormulaGroupForCell(firstCell);
		if (sharedFormulaGroup == null)
		{
			throw new RuntimeException("Failed to find a matching shared formula record");
		}
		sharedFormulaGroup.Add(agg);
		return sharedFormulaGroup.SFR;
	}

	private SharedFormulaGroup FindFormulaGroupForCell(CellReference cellRef)
	{
		if (_groupsCache == null)
		{
			_groupsCache = new Dictionary<int, SharedFormulaGroup>(_groupsBySharedFormulaRecord.Count);
			foreach (SharedFormulaGroup value in _groupsBySharedFormulaRecord.Values)
			{
				_groupsCache.Add(GetKeyForCache(value.FirstCell), value);
			}
		}
		int keyForCache = GetKeyForCache(cellRef);
		SharedFormulaGroup result = null;
		if (_groupsCache.ContainsKey(keyForCache))
		{
			result = _groupsCache[keyForCache];
		}
		return result;
	}

	private int GetKeyForCache(CellReference cellRef)
	{
		return (cellRef.Col + 1 << 16) | cellRef.Row;
	}

	public SharedValueRecordBase GetRecordForFirstCell(FormulaRecordAggregate agg)
	{
		CellReference expReference = agg.FormulaRecord.Formula.ExpReference;
		if (expReference == null)
		{
			return null;
		}
		int row = expReference.Row;
		int col = expReference.Col;
		if (agg.Row != row || agg.Column != col)
		{
			return null;
		}
		if (_groupsBySharedFormulaRecord.Count != 0)
		{
			SharedFormulaGroup sharedFormulaGroup = FindFormulaGroupForCell(expReference);
			if (sharedFormulaGroup != null)
			{
				return sharedFormulaGroup.SFR;
			}
		}
		for (int i = 0; i < _tableRecords.Count; i++)
		{
			TableRecord tableRecord = _tableRecords[i];
			if (tableRecord.IsFirstCell(row, col))
			{
				return tableRecord;
			}
		}
		foreach (ArrayRecord arrayRecord in _arrayRecords)
		{
			if (arrayRecord.IsFirstCell(row, col))
			{
				return arrayRecord;
			}
		}
		return null;
	}

	public void Unlink(SharedFormulaRecord sharedFormulaRecord)
	{
		SharedFormulaGroup sharedFormulaGroup = _groupsBySharedFormulaRecord[sharedFormulaRecord];
		_groupsBySharedFormulaRecord.Remove(sharedFormulaRecord);
		_groupsCache = null;
		if (sharedFormulaGroup == null)
		{
			throw new InvalidOperationException("Failed to find formulas for shared formula");
		}
		sharedFormulaGroup.UnlinkSharedFormulas();
	}

	public void AddArrayRecord(ArrayRecord ar)
	{
		_arrayRecords.Add(ar);
	}

	public CellRangeAddress8Bit RemoveArrayFormula(int rowIndex, int columnIndex)
	{
		foreach (ArrayRecord arrayRecord in _arrayRecords)
		{
			if (arrayRecord.IsInRange(rowIndex, columnIndex))
			{
				_arrayRecords.Remove(arrayRecord);
				return arrayRecord.Range;
			}
		}
		string text = new CellReference(rowIndex, columnIndex, pAbsRow: false, pAbsCol: false).FormatAsString();
		throw new ArgumentException("Specified cell " + text + " is not part of an array formula.");
	}

	public ArrayRecord GetArrayRecord(int firstRow, int firstColumn)
	{
		foreach (ArrayRecord arrayRecord in _arrayRecords)
		{
			if (arrayRecord.IsFirstCell(firstRow, firstColumn))
			{
				return arrayRecord;
			}
		}
		return null;
	}
}
