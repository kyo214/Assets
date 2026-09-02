using System.Collections.Generic;
using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class ExternSheetRecord : StandardRecord
{
	public const short sid = 23;

	private IList<RefSubRecord> _list;

	public int NumOfREFRecords => _list.Count;

	public int NumOfRefs => _list.Count;

	protected override int DataSize => 2 + _list.Count * 6;

	public override short Sid => 23;

	public ExternSheetRecord()
	{
		_list = new List<RefSubRecord>();
	}

	public ExternSheetRecord(RecordInputStream in1)
	{
		_list = new List<RefSubRecord>();
		int num = in1.ReadShort();
		for (int i = 0; i < num; i++)
		{
			RefSubRecord item = new RefSubRecord(in1);
			_list.Add(item);
		}
	}

	public int AddRef(int extBookIndex, int firstSheetIndex, int lastSheetIndex)
	{
		_list.Add(new RefSubRecord(extBookIndex, firstSheetIndex, lastSheetIndex));
		return _list.Count - 1;
	}

	public int GetRefIxForSheet(int externalBookIndex, int firstSheetIndex, int lastSheetIndex)
	{
		int count = _list.Count;
		for (int i = 0; i < count; i++)
		{
			RefSubRecord refSubRecord = GetRef(i);
			if (refSubRecord.ExtBookIndex == externalBookIndex && refSubRecord.FirstSheetIndex == firstSheetIndex && refSubRecord.LastSheetIndex == lastSheetIndex)
			{
				return i;
			}
		}
		return -1;
	}

	public void AddREFRecord(RefSubRecord rec)
	{
		_list.Add(rec);
	}

	private RefSubRecord GetRef(int i)
	{
		return _list[i];
	}

	public void RemoveSheet(int sheetIdx)
	{
		int count = _list.Count;
		for (int i = 0; i < count; i++)
		{
			RefSubRecord refSubRecord = _list[i];
			if (refSubRecord.FirstSheetIndex == sheetIdx && refSubRecord.LastSheetIndex == sheetIdx)
			{
				_list[i] = new RefSubRecord(refSubRecord.ExtBookIndex, -1, -1);
			}
			else if (refSubRecord.FirstSheetIndex > sheetIdx && refSubRecord.LastSheetIndex > sheetIdx)
			{
				_list[i] = new RefSubRecord(refSubRecord.ExtBookIndex, refSubRecord.FirstSheetIndex - 1, refSubRecord.LastSheetIndex - 1);
			}
		}
	}

	public int GetExtbookIndexFromRefIndex(int refIndex)
	{
		return GetRef(refIndex).ExtBookIndex;
	}

	public int FindRefIndexFromExtBookIndex(int extBookIndex)
	{
		int count = _list.Count;
		for (int i = 0; i < count; i++)
		{
			if (GetRef(i).ExtBookIndex == extBookIndex)
			{
				return i;
			}
		}
		return -1;
	}

	public static ExternSheetRecord Combine(ExternSheetRecord[] esrs)
	{
		ExternSheetRecord externSheetRecord = new ExternSheetRecord();
		foreach (ExternSheetRecord externSheetRecord2 in esrs)
		{
			int numOfREFRecords = externSheetRecord2.NumOfREFRecords;
			for (int j = 0; j < numOfREFRecords; j++)
			{
				externSheetRecord.AddREFRecord(externSheetRecord2.GetRef(j));
			}
		}
		return externSheetRecord;
	}

	public int GetFirstSheetIndexFromRefIndex(int extRefIndex)
	{
		return GetRef(extRefIndex).FirstSheetIndex;
	}

	public int GetLastSheetIndexFromRefIndex(int extRefIndex)
	{
		return GetRef(extRefIndex).LastSheetIndex;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		int count = _list.Count;
		stringBuilder.Append("[EXTERNSHEET]\n");
		stringBuilder.Append("   numOfRefs     = ").Append(count).Append("\n");
		for (int i = 0; i < count; i++)
		{
			stringBuilder.Append("refrec         #").Append(i).Append(": ");
			stringBuilder.Append(GetRef(i).ToString());
			stringBuilder.Append('\n');
		}
		stringBuilder.Append("[/EXTERNSHEET]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		int count = _list.Count;
		out1.WriteShort(count);
		for (int i = 0; i < count; i++)
		{
			GetRef(i).Serialize(out1);
		}
	}
}
