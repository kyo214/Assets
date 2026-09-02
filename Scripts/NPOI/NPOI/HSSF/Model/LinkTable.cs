using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NPOI.HSSF.Record;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;
using NPOI.Util;

namespace NPOI.HSSF.Model;

public class LinkTable
{
	private class CRNBlock
	{
		private CRNCountRecord _countRecord;

		private CRNRecord[] _crns;

		public CRNBlock(RecordStream rs)
		{
			_countRecord = (CRNCountRecord)rs.GetNext();
			CRNRecord[] array = new CRNRecord[_countRecord.NumberOfCRNs];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (CRNRecord)rs.GetNext();
			}
			_crns = array;
		}

		public CRNRecord[] GetCrns()
		{
			return (CRNRecord[])_crns.Clone();
		}
	}

	private class ExternalBookBlock
	{
		private SupBookRecord _externalBookRecord;

		internal ExternalNameRecord[] _externalNameRecords;

		private CRNBlock[] _crnBlocks;

		public int NumberOfNames => _externalNameRecords.Length;

		public ExternalBookBlock()
		{
			_externalBookRecord = SupBookRecord.CreateAddInFunctions();
			_externalNameRecords = new ExternalNameRecord[0];
			_crnBlocks = new CRNBlock[0];
		}

		public ExternalBookBlock(RecordStream rs)
		{
			_externalBookRecord = (SupBookRecord)rs.GetNext();
			ArrayList arrayList = new ArrayList();
			while (rs.PeekNextClass() == typeof(ExternalNameRecord))
			{
				arrayList.Add(rs.GetNext());
			}
			_externalNameRecords = (ExternalNameRecord[])arrayList.ToArray(typeof(ExternalNameRecord));
			arrayList.Clear();
			while (rs.PeekNextClass() == typeof(CRNCountRecord))
			{
				arrayList.Add(new CRNBlock(rs));
			}
			_crnBlocks = (CRNBlock[])arrayList.ToArray(typeof(CRNBlock));
		}

		public ExternalBookBlock(string url, string[] sheetNames)
		{
			_externalBookRecord = SupBookRecord.CreateExternalReferences(url, sheetNames);
			_crnBlocks = new CRNBlock[0];
		}

		public ExternalBookBlock(int numberOfSheets)
		{
			_externalBookRecord = SupBookRecord.CreateInternalReferences((short)numberOfSheets);
			_externalNameRecords = new ExternalNameRecord[0];
			_crnBlocks = new CRNBlock[0];
		}

		public int AddExternalName(ExternalNameRecord rec)
		{
			ExternalNameRecord[] array = new ExternalNameRecord[_externalNameRecords.Length + 1];
			Array.Copy(_externalNameRecords, 0, array, 0, _externalNameRecords.Length);
			array[^1] = rec;
			_externalNameRecords = array;
			return _externalNameRecords.Length - 1;
		}

		public SupBookRecord GetExternalBookRecord()
		{
			return _externalBookRecord;
		}

		public string GetNameText(int definedNameIndex)
		{
			return _externalNameRecords[definedNameIndex].Text;
		}

		public int GetIndexOfName(string name)
		{
			for (int i = 0; i < _externalNameRecords.Length; i++)
			{
				if (_externalNameRecords[i].Text.Equals(name, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
			return -1;
		}

		public int GetNameIx(int definedNameIndex)
		{
			return _externalNameRecords[definedNameIndex].Ix;
		}
	}

	private ExternalBookBlock[] _externalBookBlocks;

	private ExternSheetRecord _externSheetRecord;

	private List<NameRecord> _definedNames;

	private int _recordCount;

	private WorkbookRecordList _workbookRecordList;

	public int RecordCount => _recordCount;

	public int NumNames => _definedNames.Count;

	private static ExternSheetRecord ReadExtSheetRecord(RecordStream rs)
	{
		List<ExternSheetRecord> list = new List<ExternSheetRecord>(2);
		while (rs.PeekNextClass() == typeof(ExternSheetRecord))
		{
			list.Add((ExternSheetRecord)rs.GetNext());
		}
		int count = list.Count;
		if (count < 1)
		{
			throw new Exception("Expected an EXTERNSHEET record but got (" + rs.PeekNextClass().Name + ")");
		}
		if (count == 1)
		{
			return list[0];
		}
		_ = new ExternSheetRecord[count];
		return ExternSheetRecord.Combine(list.ToArray());
	}

	public LinkTable(List<NPOI.HSSF.Record.Record> inputList, int startIndex, WorkbookRecordList workbookRecordList, Dictionary<string, NameCommentRecord> commentRecords)
	{
		_workbookRecordList = workbookRecordList;
		RecordStream recordStream = new RecordStream(inputList, startIndex);
		ArrayList arrayList = new ArrayList();
		while (recordStream.PeekNextClass() == typeof(SupBookRecord))
		{
			arrayList.Add(new ExternalBookBlock(recordStream));
		}
		_externalBookBlocks = (ExternalBookBlock[])arrayList.ToArray(typeof(ExternalBookBlock));
		arrayList.Clear();
		if (_externalBookBlocks.Length != 0)
		{
			if (recordStream.PeekNextClass() != typeof(ExternSheetRecord))
			{
				_externSheetRecord = null;
			}
			else
			{
				_externSheetRecord = ReadExtSheetRecord(recordStream);
			}
		}
		else
		{
			_externSheetRecord = null;
		}
		_definedNames = new List<NameRecord>();
		while (true)
		{
			Type type = recordStream.PeekNextClass();
			if (type == typeof(NameRecord))
			{
				NameRecord item = (NameRecord)recordStream.GetNext();
				_definedNames.Add(item);
				continue;
			}
			if (!(type == typeof(NameCommentRecord)))
			{
				break;
			}
			NameCommentRecord nameCommentRecord = (NameCommentRecord)recordStream.GetNext();
			commentRecords[nameCommentRecord.NameText] = nameCommentRecord;
		}
		_recordCount = recordStream.GetCountRead();
		for (int i = startIndex; i < startIndex + _recordCount; i++)
		{
			_workbookRecordList.Records.Add(inputList[i]);
		}
	}

	public LinkTable(int numberOfSheets, WorkbookRecordList workbookRecordList)
	{
		_workbookRecordList = workbookRecordList;
		_definedNames = new List<NameRecord>();
		_externalBookBlocks = new ExternalBookBlock[1]
		{
			new ExternalBookBlock(numberOfSheets)
		};
		_externSheetRecord = new ExternSheetRecord();
		_recordCount = 2;
		SupBookRecord externalBookRecord = _externalBookBlocks[0].GetExternalBookRecord();
		int num = FindFirstRecordLocBySid(140);
		if (num < 0)
		{
			throw new Exception("CountryRecord not found");
		}
		_workbookRecordList.Add(num + 1, _externSheetRecord);
		_workbookRecordList.Add(num + 1, externalBookRecord);
	}

	public NameRecord GetSpecificBuiltinRecord(byte builtInCode, int sheetNumber)
	{
		IEnumerator<NameRecord> enumerator = _definedNames.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NameRecord current = enumerator.Current;
			if (current.BuiltInName == builtInCode && current.SheetNumber == sheetNumber)
			{
				return current;
			}
		}
		return null;
	}

	public void RemoveBuiltinRecord(byte name, int sheetIndex)
	{
		NameRecord specificBuiltinRecord = GetSpecificBuiltinRecord(name, sheetIndex);
		if (specificBuiltinRecord != null)
		{
			_definedNames.Remove(specificBuiltinRecord);
		}
	}

	public int GetFirstInternalSheetIndexForExtIndex(int extRefIndex)
	{
		if (extRefIndex >= _externSheetRecord.NumOfRefs || extRefIndex < 0)
		{
			return -1;
		}
		return _externSheetRecord.GetFirstSheetIndexFromRefIndex(extRefIndex);
	}

	public int GetLastInternalSheetIndexForExtIndex(int extRefIndex)
	{
		if (extRefIndex >= _externSheetRecord.NumOfRefs || extRefIndex < 0)
		{
			return -1;
		}
		return _externSheetRecord.GetLastSheetIndexFromRefIndex(extRefIndex);
	}

	public void RemoveSheet(int sheetIdx)
	{
		_externSheetRecord.RemoveSheet(sheetIdx);
	}

	private int ExtendExternalBookBlocks(ExternalBookBlock newBlock)
	{
		ExternalBookBlock[] array = new ExternalBookBlock[_externalBookBlocks.Length + 1];
		Array.Copy(_externalBookBlocks, 0, array, 0, _externalBookBlocks.Length);
		array[^1] = newBlock;
		_externalBookBlocks = array;
		return _externalBookBlocks.Length - 1;
	}

	private int FindRefIndexFromExtBookIndex(int extBookIndex)
	{
		return _externSheetRecord.FindRefIndexFromExtBookIndex(extBookIndex);
	}

	public NameXPtg GetNameXPtg(string name, int sheetRefIndex)
	{
		for (int i = 0; i < _externalBookBlocks.Length; i++)
		{
			int indexOfName = _externalBookBlocks[i].GetIndexOfName(name);
			if (indexOfName >= 0)
			{
				int num = FindRefIndexFromExtBookIndex(i);
				if (num >= 0 && (sheetRefIndex == -1 || num == sheetRefIndex))
				{
					return new NameXPtg(num, indexOfName);
				}
			}
		}
		return null;
	}

	public NameRecord GetNameRecord(int index)
	{
		return _definedNames[index];
	}

	public void AddName(NameRecord name)
	{
		_definedNames.Add(name);
		int num = FindFirstRecordLocBySid(23);
		if (num == -1)
		{
			num = FindFirstRecordLocBySid(430);
		}
		if (num == -1)
		{
			num = FindFirstRecordLocBySid(140);
		}
		int count = _definedNames.Count;
		_workbookRecordList.Add(num + count, name);
	}

	public NameXPtg AddNameXPtg(string name)
	{
		int externalBookIndex = -1;
		ExternalBookBlock externalBookBlock = null;
		for (int i = 0; i < _externalBookBlocks.Length; i++)
		{
			if (_externalBookBlocks[i].GetExternalBookRecord().IsAddInFunctions)
			{
				externalBookBlock = _externalBookBlocks[i];
				externalBookIndex = i;
				break;
			}
		}
		if (externalBookBlock == null)
		{
			externalBookBlock = new ExternalBookBlock();
			externalBookIndex = ExtendExternalBookBlocks(externalBookBlock);
			int pos = FindFirstRecordLocBySid(23);
			_workbookRecordList.Add(pos, externalBookBlock.GetExternalBookRecord());
			_externSheetRecord.AddRef(_externalBookBlocks.Length - 1, -2, -2);
		}
		ExternalNameRecord externalNameRecord = new ExternalNameRecord();
		externalNameRecord.Text = name;
		externalNameRecord.SetParsedExpression(new Ptg[1] { ErrPtg.REF_INVALID });
		int nameIndex = externalBookBlock.AddExternalName(externalNameRecord);
		int num = 0;
		IEnumerator enumerator = _workbookRecordList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			NPOI.HSSF.Record.Record record = (NPOI.HSSF.Record.Record)enumerator.Current;
			if (record is SupBookRecord && ((SupBookRecord)record).IsAddInFunctions)
			{
				break;
			}
			num++;
		}
		int numberOfNames = externalBookBlock.NumberOfNames;
		_workbookRecordList.Add(num + numberOfNames, externalNameRecord);
		int num2 = -2;
		return new NameXPtg(_externSheetRecord.GetRefIxForSheet(externalBookIndex, num2, num2), nameIndex);
	}

	public void RemoveName(int namenum)
	{
		_definedNames.RemoveAt(namenum);
	}

	private static int GetSheetIndex(string[] sheetNames, string sheetName)
	{
		for (int i = 0; i < sheetNames.Length; i++)
		{
			if (sheetNames[i].Equals(sheetName))
			{
				return i;
			}
		}
		throw new InvalidOperationException("External workbook does not contain sheet '" + sheetName + "'");
	}

	private int GetExternalWorkbookIndex(string workbookName)
	{
		for (int i = 0; i < _externalBookBlocks.Length; i++)
		{
			SupBookRecord externalBookRecord = _externalBookBlocks[i].GetExternalBookRecord();
			if (externalBookRecord.IsExternalReferences && workbookName.Equals(externalBookRecord.URL))
			{
				return i;
			}
		}
		return -1;
	}

	public int LinkExternalWorkbook(string name, IWorkbook externalWorkbook)
	{
		int externalWorkbookIndex = GetExternalWorkbookIndex(name);
		if (externalWorkbookIndex != -1)
		{
			return externalWorkbookIndex;
		}
		string[] array = new string[externalWorkbook.NumberOfSheets];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = externalWorkbook.GetSheetName(i);
		}
		ExternalBookBlock externalBookBlock = new ExternalBookBlock("\0" + name, array);
		externalWorkbookIndex = ExtendExternalBookBlocks(externalBookBlock);
		int num = FindFirstRecordLocBySid(23);
		if (num == -1)
		{
			num = _workbookRecordList.Count;
		}
		_workbookRecordList.Add(num, externalBookBlock.GetExternalBookRecord());
		for (int j = 0; j < array.Length; j++)
		{
			_externSheetRecord.AddRef(externalWorkbookIndex, j, j);
		}
		return externalWorkbookIndex;
	}

	public int GetExternalSheetIndex(string workbookName, string firstSheetName, string lastSheetName)
	{
		int externalWorkbookIndex = GetExternalWorkbookIndex(workbookName);
		if (externalWorkbookIndex == -1)
		{
			throw new RuntimeException("No external workbook with name '" + workbookName + "'");
		}
		SupBookRecord externalBookRecord = _externalBookBlocks[externalWorkbookIndex].GetExternalBookRecord();
		int sheetIndex = GetSheetIndex(externalBookRecord.SheetNames, firstSheetName);
		int sheetIndex2 = GetSheetIndex(externalBookRecord.SheetNames, lastSheetName);
		int num = _externSheetRecord.GetRefIxForSheet(externalWorkbookIndex, sheetIndex, sheetIndex2);
		if (num < 0)
		{
			num = _externSheetRecord.AddRef(externalWorkbookIndex, sheetIndex, sheetIndex2);
		}
		return num;
	}

	public string[] GetExternalBookAndSheetName(int extRefIndex)
	{
		int extbookIndexFromRefIndex = _externSheetRecord.GetExtbookIndexFromRefIndex(extRefIndex);
		SupBookRecord externalBookRecord = _externalBookBlocks[extbookIndexFromRefIndex].GetExternalBookRecord();
		if (!externalBookRecord.IsExternalReferences)
		{
			return null;
		}
		int firstSheetIndexFromRefIndex = _externSheetRecord.GetFirstSheetIndexFromRefIndex(extRefIndex);
		int lastSheetIndexFromRefIndex = _externSheetRecord.GetLastSheetIndexFromRefIndex(extRefIndex);
		string text = null;
		string text2 = null;
		if (firstSheetIndexFromRefIndex >= 0)
		{
			text = externalBookRecord.SheetNames[firstSheetIndexFromRefIndex];
		}
		if (lastSheetIndexFromRefIndex >= 0)
		{
			text2 = externalBookRecord.SheetNames[lastSheetIndexFromRefIndex];
		}
		if (firstSheetIndexFromRefIndex != lastSheetIndexFromRefIndex)
		{
			return new string[3] { externalBookRecord.URL, text, text2 };
		}
		return new string[2] { externalBookRecord.URL, text };
	}

	public int CheckExternSheet(int sheetIndex)
	{
		return CheckExternSheet(sheetIndex, sheetIndex);
	}

	public int CheckExternSheet(int firstSheetIndex, int lastSheetIndex)
	{
		int num = -1;
		for (int i = 0; i < _externalBookBlocks.Length; i++)
		{
			if (_externalBookBlocks[i].GetExternalBookRecord().IsInternalReferences)
			{
				num = i;
				break;
			}
		}
		if (num < 0)
		{
			throw new InvalidOperationException("Could not find 'internal references' EXTERNALBOOK");
		}
		int refIxForSheet = _externSheetRecord.GetRefIxForSheet(num, firstSheetIndex, lastSheetIndex);
		if (refIxForSheet >= 0)
		{
			return refIxForSheet;
		}
		return _externSheetRecord.AddRef(num, firstSheetIndex, lastSheetIndex);
	}

	private int FindFirstRecordLocBySid(short sid)
	{
		int num = 0;
		IEnumerator<NPOI.HSSF.Record.Record> enumerator = _workbookRecordList.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Current.Sid == sid)
			{
				return num;
			}
			num++;
		}
		return -1;
	}

	public string ResolveNameXText(int refIndex, int definedNameIndex, InternalWorkbook workbook)
	{
		int extbookIndexFromRefIndex = _externSheetRecord.GetExtbookIndexFromRefIndex(refIndex);
		int firstSheetIndexFromRefIndex = _externSheetRecord.GetFirstSheetIndexFromRefIndex(refIndex);
		if (firstSheetIndexFromRefIndex == -1)
		{
			throw new RuntimeException("Referenced sheet could not be found");
		}
		if (_externalBookBlocks[extbookIndexFromRefIndex]._externalNameRecords.Length > definedNameIndex)
		{
			return _externalBookBlocks[extbookIndexFromRefIndex].GetNameText(definedNameIndex);
		}
		if (firstSheetIndexFromRefIndex == -2)
		{
			NameRecord nameRecord = GetNameRecord(definedNameIndex);
			int sheetNumber = nameRecord.SheetNumber;
			StringBuilder stringBuilder = new StringBuilder();
			if (sheetNumber > 0)
			{
				string sheetName = workbook.GetSheetName(sheetNumber - 1);
				SheetNameFormatter.AppendFormat(stringBuilder, sheetName);
				stringBuilder.Append("!");
			}
			stringBuilder.Append(nameRecord.NameText);
			return stringBuilder.ToString();
		}
		throw new IndexOutOfRangeException("Ext Book Index relative but beyond the supported length, was " + extbookIndexFromRefIndex + " but maximum is " + _externalBookBlocks.Length);
	}

	public int ResolveNameXIx(int refIndex, int definedNameIndex)
	{
		int extbookIndexFromRefIndex = _externSheetRecord.GetExtbookIndexFromRefIndex(refIndex);
		return _externalBookBlocks[extbookIndexFromRefIndex].GetNameIx(definedNameIndex);
	}

	public bool ChangeExternalReference(string oldUrl, string newUrl)
	{
		ExternalBookBlock[] externalBookBlocks = _externalBookBlocks;
		for (int i = 0; i < externalBookBlocks.Length; i++)
		{
			SupBookRecord externalBookRecord = externalBookBlocks[i].GetExternalBookRecord();
			if (externalBookRecord.IsExternalReferences && externalBookRecord.URL.Equals(oldUrl))
			{
				externalBookRecord.URL = newUrl;
				return true;
			}
		}
		return false;
	}
}
