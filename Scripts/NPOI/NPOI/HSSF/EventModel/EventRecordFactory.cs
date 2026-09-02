using System.Collections;
using System.IO;
using NPOI.HSSF.Record;

namespace NPOI.HSSF.EventModel;

public class EventRecordFactory
{
	private IERFListener _listener;

	private ArrayList _sids;

	public EventRecordFactory(IERFListener listener, ArrayList sids)
	{
		_listener = listener;
		_sids = sids;
		if (_sids == null)
		{
			_sids = null;
			return;
		}
		if (_sids == null)
		{
			_sids = new ArrayList();
		}
		_sids.Sort();
	}

	private bool IsSidIncluded(int sid)
	{
		if (_sids == null)
		{
			return true;
		}
		return _sids.BinarySearch((short)sid) >= 0;
	}

	private bool ProcessRecord(NPOI.HSSF.Record.Record record)
	{
		if (!IsSidIncluded(record.Sid))
		{
			return true;
		}
		return _listener.ProcessRecord(record);
	}

	public void ProcessRecords(Stream in1)
	{
		NPOI.HSSF.Record.Record record = null;
		RecordInputStream recordInputStream = new RecordInputStream(in1);
		while (recordInputStream.HasNextRecord)
		{
			recordInputStream.NextRecord();
			NPOI.HSSF.Record.Record[] array = RecordFactory.CreateRecord(recordInputStream);
			if (array.Length > 1)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (record != null && !ProcessRecord(record))
					{
						return;
					}
					record = array[i];
				}
				continue;
			}
			NPOI.HSSF.Record.Record record2 = array[0];
			if (record2 != null)
			{
				if (record != null && !ProcessRecord(record))
				{
					return;
				}
				record = record2;
			}
		}
		if (record != null)
		{
			ProcessRecord(record);
		}
	}
}
