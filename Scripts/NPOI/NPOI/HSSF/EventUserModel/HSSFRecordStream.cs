using System.Collections;
using NPOI.HSSF.Record;
using NPOI.Util;

namespace NPOI.HSSF.EventUserModel;

public class HSSFRecordStream
{
	private RecordInputStream in1;

	private bool hitEOS;

	private bool complete;

	private ArrayList bonusRecords;

	private NPOI.HSSF.Record.Record rec;

	private NPOI.HSSF.Record.Record lastRec;

	private DrawingRecord lastDrawingRecord = new DrawingRecord();

	public HSSFRecordStream(RecordInputStream inp)
	{
		in1 = inp;
	}

	public NPOI.HSSF.Record.Record NextRecord()
	{
		NPOI.HSSF.Record.Record record = null;
		while (record == null && !complete)
		{
			record = GetBonusRecord();
			if (record == null)
			{
				record = GetNextRecord();
			}
		}
		return record;
	}

	private NPOI.HSSF.Record.Record GetBonusRecord()
	{
		if (bonusRecords != null)
		{
			NPOI.HSSF.Record.Record result = (NPOI.HSSF.Record.Record)bonusRecords[0];
			bonusRecords.RemoveAt(0);
			if (bonusRecords.Count == 0)
			{
				bonusRecords = null;
			}
			return result;
		}
		return null;
	}

	private NPOI.HSSF.Record.Record GetNextRecord()
	{
		NPOI.HSSF.Record.Record result = null;
		if (in1.HasNextRecord)
		{
			in1.NextRecord();
			short sid = in1.Sid;
			if (sid == 0)
			{
				return null;
			}
			if (rec != null && sid != 60)
			{
				result = rec;
			}
			if (sid != 60)
			{
				NPOI.HSSF.Record.Record[] array = RecordFactory.CreateRecord(in1);
				if (array.Length > 1)
				{
					bonusRecords = new ArrayList(array.Length - 1);
					for (int i = 0; i < array.Length - 1; i++)
					{
						bonusRecords.Add(array[i]);
					}
				}
				rec = array[^1];
			}
			else
			{
				ContinueRecord continueRecord = (ContinueRecord)RecordFactory.CreateRecord(in1)[0];
				if (lastRec is ObjRecord || lastRec is TextObjectRecord)
				{
					lastDrawingRecord.ProcessContinueRecord(continueRecord.Data);
					rec = lastDrawingRecord;
				}
				else if (lastRec is DrawingGroupRecord)
				{
					((DrawingGroupRecord)lastRec).ProcessContinueRecord(continueRecord.Data);
					rec = lastRec;
				}
				else if (!(rec is UnknownRecord))
				{
					throw new RecordFormatException("Records should handle ContinueRecord internally. Should not see this exception");
				}
			}
			lastRec = rec;
			if (rec is DrawingRecord)
			{
				lastDrawingRecord = (DrawingRecord)rec;
			}
		}
		else
		{
			hitEOS = true;
		}
		if (hitEOS)
		{
			complete = true;
			if (rec != null)
			{
				result = rec;
				rec = null;
			}
		}
		return result;
	}
}
