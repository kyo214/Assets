using System.Collections;
using System.Collections.Generic;
using NPOI.HSSF.Model;
using NPOI.HSSF.Record;
using NPOI.HSSF.UserModel;

namespace NPOI.HSSF.EventUserModel;

public class EventWorkbookBuilder
{
	public class SheetRecordCollectingListener : IHSSFListener
	{
		private IHSSFListener childListener;

		private ArrayList boundSheetRecords = new ArrayList();

		private ArrayList externSheetRecords = new ArrayList();

		private SSTRecord sstRecord;

		public SheetRecordCollectingListener(IHSSFListener childListener)
		{
			this.childListener = childListener;
		}

		public BoundSheetRecord[] GetBoundSheetRecords()
		{
			return (BoundSheetRecord[])boundSheetRecords.ToArray(typeof(BoundSheetRecord));
		}

		public ExternSheetRecord[] GetExternSheetRecords()
		{
			return (ExternSheetRecord[])externSheetRecords.ToArray(typeof(ExternSheetRecord));
		}

		public SSTRecord GetSSTRecord()
		{
			return sstRecord;
		}

		public HSSFWorkbook GetStubHSSFWorkbook()
		{
			HSSFWorkbook hSSFWorkbook = HSSFWorkbook.Create(GetStubWorkbook());
			foreach (BoundSheetRecord boundSheetRecord in boundSheetRecords)
			{
				hSSFWorkbook.CreateSheet(boundSheetRecord.Sheetname);
			}
			return hSSFWorkbook;
		}

		public InternalWorkbook GetStubWorkbook()
		{
			return CreateStubWorkbook(GetExternSheetRecords(), GetBoundSheetRecords(), GetSSTRecord());
		}

		public void ProcessRecord(NPOI.HSSF.Record.Record record)
		{
			ProcessRecordInternally(record);
			childListener.ProcessRecord(record);
		}

		public void ProcessRecordInternally(NPOI.HSSF.Record.Record record)
		{
			if (record is BoundSheetRecord)
			{
				boundSheetRecords.Add(record);
			}
			else if (record is ExternSheetRecord)
			{
				externSheetRecords.Add(record);
			}
			else if (record is SSTRecord)
			{
				sstRecord = (SSTRecord)record;
			}
		}
	}

	public static InternalWorkbook CreateStubWorkbook(ExternSheetRecord[] externs, BoundSheetRecord[] bounds, SSTRecord sst)
	{
		List<NPOI.HSSF.Record.Record> list = new List<NPOI.HSSF.Record.Record>();
		if (bounds != null)
		{
			for (int i = 0; i < bounds.Length; i++)
			{
				list.Add(bounds[i]);
			}
		}
		if (sst != null)
		{
			list.Add(sst);
		}
		if (externs != null)
		{
			list.Add(SupBookRecord.CreateInternalReferences((short)externs.Length));
			for (int j = 0; j < externs.Length; j++)
			{
				list.Add(externs[j]);
			}
		}
		list.Add(EOFRecord.instance);
		return InternalWorkbook.CreateWorkbook(list);
	}

	public static InternalWorkbook CreateStubWorkbook(ExternSheetRecord[] externs, BoundSheetRecord[] bounds)
	{
		return CreateStubWorkbook(externs, bounds, null);
	}
}
