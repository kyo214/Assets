using System;
using System.Collections;
using NPOI.HSSF.Record;
using NPOI.HSSF.Record.Chart;

namespace NPOI.HSSF.Model;

public class RecordStream
{
	private IList _list;

	private int _nextIndex;

	private int _endIx;

	private int _countRead;

	public RecordStream(IList inputList, int startIndex, int endIx)
	{
		_list = inputList;
		_nextIndex = startIndex;
		_endIx = endIx;
		_countRead = 0;
	}

	public RecordStream(IList records, int startIx)
		: this(records, startIx, records.Count)
	{
	}

	public bool HasNext()
	{
		return _nextIndex < _endIx;
	}

	public NPOI.HSSF.Record.Record GetNext()
	{
		if (_nextIndex >= _list.Count)
		{
			throw new Exception("Attempt to Read past end of record stream");
		}
		_countRead++;
		return (NPOI.HSSF.Record.Record)_list[_nextIndex++];
	}

	public int PeekNextSid()
	{
		if (!HasNext())
		{
			return -1;
		}
		return ((NPOI.HSSF.Record.Record)_list[_nextIndex]).Sid;
	}

	public Type PeekNextClass()
	{
		if (_nextIndex >= _list.Count)
		{
			return null;
		}
		return _list[_nextIndex].GetType();
	}

	public int GetCountRead()
	{
		return _countRead;
	}

	public int PeekNextChartSid()
	{
		if (!HasNext())
		{
			return -1;
		}
		while (PeekNextSid() == StartBlockRecord.sid || PeekNextSid() == 2131)
		{
			GetNext();
		}
		return PeekNextSid();
	}

	public void FindChartSubStream()
	{
		while (PeekNextSid() > -1)
		{
			NPOI.HSSF.Record.Record next = GetNext();
			if (next.Sid == 2057 && (next as BOFRecord).Type == BOFRecordType.Chart)
			{
				_nextIndex--;
				_countRead--;
				break;
			}
		}
	}
}
