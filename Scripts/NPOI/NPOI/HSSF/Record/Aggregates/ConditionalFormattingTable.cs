using System;
using System.Collections.Generic;
using NPOI.HSSF.Model;
using NPOI.SS.Formula;

namespace NPOI.HSSF.Record.Aggregates;

public class ConditionalFormattingTable : RecordAggregate
{
	private IList<CFRecordsAggregate> _cfHeaders;

	public int Count => _cfHeaders.Count;

	public ConditionalFormattingTable()
	{
		_cfHeaders = new List<CFRecordsAggregate>();
	}

	public ConditionalFormattingTable(RecordStream rs)
	{
		List<CFRecordsAggregate> list = new List<CFRecordsAggregate>();
		while (rs.PeekNextClass() == typeof(CFHeaderRecord) || rs.PeekNextClass() == typeof(CFHeader12Record))
		{
			list.Add(CFRecordsAggregate.CreateCFAggregate(rs));
		}
		_cfHeaders = list;
	}

	public override void VisitContainedRecords(RecordVisitor rv)
	{
		foreach (CFRecordsAggregate cfHeader in _cfHeaders)
		{
			cfHeader.VisitContainedRecords(rv);
		}
	}

	public int Add(CFRecordsAggregate cfAggregate)
	{
		cfAggregate.Header.ID = _cfHeaders.Count;
		_cfHeaders.Add(cfAggregate);
		return _cfHeaders.Count - 1;
	}

	public CFRecordsAggregate Get(int index)
	{
		CheckIndex(index);
		return _cfHeaders[index];
	}

	public void Remove(int index)
	{
		CheckIndex(index);
		_cfHeaders.RemoveAt(index);
	}

	private void CheckIndex(int index)
	{
		if (index < 0 || index >= _cfHeaders.Count)
		{
			throw new ArgumentException("Specified CF index " + index + " is outside the allowable range (0.." + (_cfHeaders.Count - 1) + ")");
		}
	}

	public void UpdateFormulasAfterCellShift(FormulaShifter shifter, int externSheetIndex)
	{
		for (int i = 0; i < _cfHeaders.Count; i++)
		{
			if (!_cfHeaders[i].UpdateFormulasAfterCellShift(shifter, externSheetIndex))
			{
				_cfHeaders.RemoveAt(i);
				i--;
			}
		}
	}
}
