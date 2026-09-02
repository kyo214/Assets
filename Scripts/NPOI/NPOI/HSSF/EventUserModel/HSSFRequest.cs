using System.Collections;
using NPOI.HSSF.Record;

namespace NPOI.HSSF.EventUserModel;

public class HSSFRequest
{
	private Hashtable records;

	public HSSFRequest()
	{
		records = new Hashtable(50);
	}

	public void AddListener(IHSSFListener lsnr, short sid)
	{
		IList list = null;
		object obj = records[sid];
		if (obj != null)
		{
			list = (IList)obj;
			return;
		}
		list = new ArrayList(1);
		list.Add(lsnr);
		records[sid] = list;
	}

	public void AddListenerForAllRecords(IHSSFListener lsnr)
	{
		short[] allKnownRecordSIDs = RecordFactory.GetAllKnownRecordSIDs();
		for (int i = 0; i < allKnownRecordSIDs.Length; i++)
		{
			AddListener(lsnr, allKnownRecordSIDs[i]);
		}
	}

	public short ProcessRecord(NPOI.HSSF.Record.Record rec)
	{
		object obj = records[rec.Sid];
		short num = 0;
		if (obj != null)
		{
			IList list = (IList)obj;
			for (int i = 0; i < list.Count; i++)
			{
				object obj2 = list[i];
				if (obj2 is AbortableHSSFListener)
				{
					num = ((AbortableHSSFListener)obj2).AbortableProcessRecord(rec);
					if (num != 0)
					{
						break;
					}
				}
				else
				{
					((IHSSFListener)obj2).ProcessRecord(rec);
				}
			}
		}
		return num;
	}
}
