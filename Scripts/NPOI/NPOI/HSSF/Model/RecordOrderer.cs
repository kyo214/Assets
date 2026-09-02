using System;
using System.Collections.Generic;
using NPOI.HSSF.Record;
using NPOI.HSSF.Record.Aggregates;

namespace NPOI.HSSF.Model;

public class RecordOrderer
{
	private RecordOrderer()
	{
	}

	public static void AddNewSheetRecord(List<RecordBase> sheetRecords, RecordBase newRecord)
	{
		int index = FindSheetInsertPos(sheetRecords, newRecord.GetType());
		sheetRecords.Insert(index, newRecord);
	}

	private static int FindSheetInsertPos(List<RecordBase> records, Type recClass)
	{
		if (recClass == typeof(DataValidityTable))
		{
			return FindDataValidationTableInsertPos(records);
		}
		if (recClass == typeof(MergedCellsTable))
		{
			return FindInsertPosForNewMergedRecordTable(records);
		}
		if (recClass == typeof(ConditionalFormattingTable))
		{
			return FindInsertPosForNewCondFormatTable(records);
		}
		if (recClass == typeof(GutsRecord))
		{
			return GetGutsRecordInsertPos(records);
		}
		if (recClass == typeof(PageSettingsBlock))
		{
			return GetPageBreakRecordInsertPos(records);
		}
		if (recClass == typeof(WorksheetProtectionBlock))
		{
			return GetWorksheetProtectionBlockInsertPos(records);
		}
		throw new InvalidOperationException("Unexpected record class (" + recClass.Name + ")");
	}

	private static int GetWorksheetProtectionBlockInsertPos(List<RecordBase> records)
	{
		int num = GetDimensionsIndex(records);
		while (num > 0)
		{
			num--;
			if (!IsProtectionSubsequentRecord(records[num]))
			{
				return num + 1;
			}
		}
		throw new InvalidOperationException("did not find insert pos for protection block");
	}

	private static bool IsProtectionSubsequentRecord(object rb)
	{
		if (rb is ColumnInfoRecordsAggregate)
		{
			return true;
		}
		if (rb is NPOI.HSSF.Record.Record)
		{
			short sid = ((NPOI.HSSF.Record.Record)rb).Sid;
			if (sid == 85 || sid == 144)
			{
				return true;
			}
		}
		return false;
	}

	private static int GetPageBreakRecordInsertPos(List<RecordBase> records)
	{
		int num = GetDimensionsIndex(records) - 1;
		while (num > 0)
		{
			num--;
			if (IsPageBreakPriorRecord(records[num]))
			{
				return num + 1;
			}
		}
		throw new InvalidOperationException("Did not Find insert point for GUTS");
	}

	private static bool IsPageBreakPriorRecord(RecordBase rb)
	{
		if (rb is NPOI.HSSF.Record.Record)
		{
			switch (((NPOI.HSSF.Record.Record)rb).Sid)
			{
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
			case 17:
			case 34:
			case 42:
			case 43:
			case 94:
			case 95:
			case 129:
			case 130:
			case 523:
			case 549:
			case 2057:
				return true;
			}
		}
		return false;
	}

	private static int FindInsertPosForNewCondFormatTable(List<RecordBase> records)
	{
		for (int num = records.Count - 2; num >= 0; num--)
		{
			object obj = records[num];
			if (obj is MergedCellsTable)
			{
				return num + 1;
			}
			if (!(obj is DataValidityTable))
			{
				switch (((NPOI.HSSF.Record.Record)obj).Sid)
				{
				case 29:
				case 65:
				case 153:
				case 160:
				case 239:
				case 351:
				case 574:
					return num + 1;
				}
			}
		}
		throw new InvalidOperationException("Did not Find Window2 record");
	}

	private static int FindInsertPosForNewMergedRecordTable(List<RecordBase> records)
	{
		for (int num = records.Count - 2; num >= 0; num--)
		{
			object obj = records[num];
			if (obj is NPOI.HSSF.Record.Record)
			{
				switch (((NPOI.HSSF.Record.Record)obj).Sid)
				{
				case 29:
				case 65:
				case 153:
				case 160:
				case 574:
					return num + 1;
				}
			}
		}
		throw new InvalidOperationException("Did not Find Window2 record");
	}

	private static int FindDataValidationTableInsertPos(List<RecordBase> records)
	{
		int num = records.Count - 1;
		if (!(records[num] is EOFRecord))
		{
			throw new InvalidOperationException("Last sheet record should be EOFRecord");
		}
		while (num > 0)
		{
			num--;
			RecordBase recordBase = records[num];
			if (IsDVTPriorRecord(recordBase))
			{
				NPOI.HSSF.Record.Record record = (NPOI.HSSF.Record.Record)records[num + 1];
				if (!IsDVTSubsequentRecord(record.Sid))
				{
					throw new InvalidOperationException("Unexpected (" + record.GetType().Name + ") found after (" + recordBase.GetType().Name + ")");
				}
				return num + 1;
			}
			NPOI.HSSF.Record.Record record2 = (NPOI.HSSF.Record.Record)recordBase;
			if (!IsDVTSubsequentRecord(record2.Sid))
			{
				throw new InvalidOperationException("Unexpected (" + record2.GetType().Name + ") while looking for DV Table insert pos");
			}
		}
		return 0;
	}

	private static bool IsDVTPriorRecord(RecordBase rb)
	{
		if (rb is MergedCellsTable || rb is ConditionalFormattingTable)
		{
			return true;
		}
		switch (((NPOI.HSSF.Record.Record)rb).Sid)
		{
		case 29:
		case 65:
		case 153:
		case 160:
		case 239:
		case 351:
		case 440:
		case 574:
		case 2048:
			return true;
		default:
			return false;
		}
	}

	private static bool IsDVTSubsequentRecord(short sid)
	{
		switch (sid)
		{
		case 10:
		case 2146:
		case 2151:
		case 2152:
		case 2248:
			return true;
		default:
			return false;
		}
	}

	private static int GetDimensionsIndex(List<RecordBase> records)
	{
		int count = records.Count;
		for (int i = 0; i < count; i++)
		{
			if (records[i] is DimensionsRecord)
			{
				return i;
			}
		}
		throw new InvalidOperationException("DimensionsRecord not found");
	}

	private static int GetGutsRecordInsertPos(List<RecordBase> records)
	{
		int num = GetDimensionsIndex(records) - 1;
		while (num > 0)
		{
			num--;
			if (IsGutsPriorRecord(records[num]))
			{
				return num + 1;
			}
		}
		throw new InvalidOperationException("Did not Find insert point for GUTS");
	}

	private static bool IsGutsPriorRecord(RecordBase rb)
	{
		if (rb is NPOI.HSSF.Record.Record)
		{
			switch (((NPOI.HSSF.Record.Record)rb).Sid)
			{
			case 12:
			case 13:
			case 14:
			case 15:
			case 16:
			case 17:
			case 34:
			case 42:
			case 43:
			case 94:
			case 95:
			case 130:
			case 523:
			case 2057:
				return true;
			}
		}
		return false;
	}

	public static bool IsEndOfRowBlock(int sid)
	{
		switch (sid)
		{
		case 61:
		case 93:
		case 125:
		case 128:
		case 176:
		case 236:
		case 237:
		case 438:
		case 574:
			return true;
		case 434:
			return true;
		case 10:
			throw new InvalidOperationException("Found EOFRecord before WindowTwoRecord was encountered");
		default:
			return PageSettingsBlock.IsComponentRecord(sid);
		}
	}

	public static bool IsRowBlockRecord(int sid)
	{
		switch (sid)
		{
		case 6:
		case 253:
		case 513:
		case 515:
		case 516:
		case 517:
		case 520:
		case 545:
		case 566:
		case 638:
		case 1212:
			return true;
		default:
			return false;
		}
	}
}
