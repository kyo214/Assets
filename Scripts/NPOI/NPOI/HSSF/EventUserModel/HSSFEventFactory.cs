using System.IO;
using NPOI.HSSF.Record;
using NPOI.POIFS.FileSystem;

namespace NPOI.HSSF.EventUserModel;

public class HSSFEventFactory
{
	public void ProcessWorkbookEvents(HSSFRequest req, POIFSFileSystem fs)
	{
		Stream @in = fs.CreateDocumentInputStream("Workbook");
		ProcessEvents(req, @in);
	}

	public short AbortableProcessWorkbookEvents(HSSFRequest req, POIFSFileSystem fs)
	{
		Stream @in = fs.CreateDocumentInputStream("Workbook");
		return AbortableProcessEvents(req, @in);
	}

	public void ProcessEvents(HSSFRequest req, Stream in1)
	{
		try
		{
			GenericProcessEvents(req, new RecordInputStream(in1));
		}
		catch (HSSFUserException)
		{
		}
	}

	public short AbortableProcessEvents(HSSFRequest req, Stream in1)
	{
		return GenericProcessEvents(req, new RecordInputStream(in1));
	}

	protected short GenericProcessEvents(HSSFRequest req, RecordInputStream in1)
	{
		bool flag = true;
		short num = 0;
		NPOI.HSSF.Record.Record record = null;
		HSSFRecordStream hSSFRecordStream = new HSSFRecordStream(in1);
		while (flag)
		{
			record = hSSFRecordStream.NextRecord();
			if (record != null)
			{
				num = req.ProcessRecord(record);
				if (num != 0)
				{
					break;
				}
			}
			else
			{
				flag = false;
			}
		}
		return num;
	}
}
