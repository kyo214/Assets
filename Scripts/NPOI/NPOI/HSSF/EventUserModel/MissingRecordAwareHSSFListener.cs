using NPOI.HSSF.EventUserModel.DummyRecord;
using NPOI.HSSF.Record;

namespace NPOI.HSSF.EventUserModel;

public class MissingRecordAwareHSSFListener : IHSSFListener
{
	private IHSSFListener childListener;

	private int lastRowRow;

	private int lastCellRow;

	private int lastCellColumn;

	public MissingRecordAwareHSSFListener(IHSSFListener listener)
	{
		ResetCounts();
		childListener = listener;
	}

	public void ProcessRecord(NPOI.HSSF.Record.Record record)
	{
		CellValueRecordInterface[] array = null;
		int num;
		int num2;
		if (record is CellValueRecordInterface)
		{
			CellValueRecordInterface obj = (CellValueRecordInterface)record;
			num = obj.Row;
			num2 = obj.Column;
		}
		else
		{
			if (record is StringRecord)
			{
				childListener.ProcessRecord(record);
				return;
			}
			num = -1;
			num2 = -1;
			switch (record.Sid)
			{
			case 2057:
			{
				BOFRecord bOFRecord = (BOFRecord)record;
				if (bOFRecord.Type == BOFRecordType.Workbook || bOFRecord.Type == BOFRecordType.Worksheet)
				{
					ResetCounts();
				}
				break;
			}
			case 520:
			{
				RowRecord rowRecord = (RowRecord)record;
				if (lastRowRow + 1 < rowRecord.RowNumber)
				{
					for (int i = lastRowRow + 1; i < rowRecord.RowNumber; i++)
					{
						MissingRowDummyRecord record2 = new MissingRowDummyRecord(i);
						childListener.ProcessRecord(record2);
					}
				}
				lastRowRow = rowRecord.RowNumber;
				break;
			}
			case 1212:
				childListener.ProcessRecord(record);
				return;
			case 190:
			{
				CellValueRecordInterface[] array2 = RecordFactory.ConvertBlankRecords((MulBlankRecord)record);
				array = array2;
				break;
			}
			case 189:
			{
				CellValueRecordInterface[] array2 = RecordFactory.ConvertRKRecords((MulRKRecord)record);
				array = array2;
				break;
			}
			case 28:
			{
				NoteRecord obj2 = (NoteRecord)record;
				num = obj2.Row;
				num2 = obj2.Column;
				break;
			}
			}
		}
		if (array != null && array.Length != 0)
		{
			num = array[0].Row;
			num2 = array[0].Column;
		}
		if (num != lastCellRow && lastCellRow > -1)
		{
			for (int j = lastCellRow; j < num; j++)
			{
				int lastColumnNumber = -1;
				if (j == lastCellRow)
				{
					lastColumnNumber = lastCellColumn;
				}
				childListener.ProcessRecord(new LastCellOfRowDummyRecord(j, lastColumnNumber));
			}
		}
		if (lastCellRow != -1 && lastCellColumn != -1 && num == -1)
		{
			childListener.ProcessRecord(new LastCellOfRowDummyRecord(lastCellRow, lastCellColumn));
			lastCellRow = -1;
			lastCellColumn = -1;
		}
		if (num != lastCellRow)
		{
			lastCellColumn = -1;
		}
		if (lastCellColumn != num2 - 1)
		{
			for (int k = lastCellColumn + 1; k < num2; k++)
			{
				childListener.ProcessRecord(new MissingCellDummyRecord(num, k));
			}
		}
		if (array != null && array.Length != 0)
		{
			num2 = array[^1].Column;
		}
		if (num2 != -1)
		{
			lastCellColumn = num2;
			lastCellRow = num;
		}
		if (array != null && array.Length != 0)
		{
			CellValueRecordInterface[] array2 = array;
			foreach (CellValueRecordInterface cellValueRecordInterface in array2)
			{
				childListener.ProcessRecord((NPOI.HSSF.Record.Record)cellValueRecordInterface);
			}
		}
		else
		{
			childListener.ProcessRecord(record);
		}
	}

	private void ResetCounts()
	{
		lastRowRow = -1;
		lastCellRow = -1;
		lastCellColumn = -1;
	}
}
