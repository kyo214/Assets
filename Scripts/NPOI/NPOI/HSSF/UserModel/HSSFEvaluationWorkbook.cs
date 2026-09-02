using System;
using NPOI.HSSF.Model;
using NPOI.HSSF.Record;
using NPOI.HSSF.Record.Aggregates;
using NPOI.SS;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.SS.Formula.UDF;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.Util;

namespace NPOI.HSSF.UserModel;

public class HSSFEvaluationWorkbook : IFormulaRenderingWorkbook, IEvaluationWorkbook, IFormulaParsingWorkbook
{
	private class Name : IEvaluationName
	{
		private NameRecord _nameRecord;

		private int _index;

		public Ptg[] NameDefinition => _nameRecord.NameDefinition;

		public string NameText => _nameRecord.NameText;

		public bool HasFormula => _nameRecord.HasFormula;

		public bool IsFunctionName => _nameRecord.IsFunctionName;

		public bool IsRange => _nameRecord.HasFormula;

		public Name(NameRecord nameRecord, int index)
		{
			_nameRecord = nameRecord;
			_index = index;
		}

		public NamePtg CreatePtg()
		{
			return new NamePtg(_index);
		}
	}

	private static POILogger logger = POILogFactory.GetLogger(typeof(HSSFEvaluationWorkbook));

	private HSSFWorkbook _uBook;

	private InternalWorkbook _iBook;

	public static HSSFEvaluationWorkbook Create(IWorkbook book)
	{
		if (book == null)
		{
			return null;
		}
		return new HSSFEvaluationWorkbook((HSSFWorkbook)book);
	}

	private HSSFEvaluationWorkbook(HSSFWorkbook book)
	{
		_uBook = book;
		_iBook = book.Workbook;
	}

	public void ClearAllCachedResultValues()
	{
	}

	public IName CreateName()
	{
		return _uBook.CreateName();
	}

	public int GetExternalSheetIndex(string sheetName)
	{
		int sheetIndex = _uBook.GetSheetIndex(sheetName);
		return _iBook.CheckExternSheet(sheetIndex);
	}

	public int GetExternalSheetIndex(string workbookName, string sheetName)
	{
		return _iBook.GetExternalSheetIndex(workbookName, sheetName);
	}

	public ExternalName GetExternalName(int externSheetIndex, int externNameIndex)
	{
		return _iBook.GetExternalName(externSheetIndex, externNameIndex);
	}

	public ExternalName GetExternalName(string nameName, string sheetName, int externalWorkbookNumber)
	{
		throw new InvalidOperationException("XSSF-style external names are not supported for HSSF");
	}

	public Ptg Get3DReferencePtg(CellReference cr, SheetIdentifier sheet)
	{
		int sheetExtIx = GetSheetExtIx(sheet);
		return new Ref3DPtg(cr, sheetExtIx);
	}

	public Ptg Get3DReferencePtg(AreaReference areaRef, SheetIdentifier sheet)
	{
		int sheetExtIx = GetSheetExtIx(sheet);
		return new Area3DPtg(areaRef, sheetExtIx);
	}

	public Ptg GetNameXPtg(string name, SheetIdentifier sheet)
	{
		int sheetExtIx = GetSheetExtIx(sheet);
		return _iBook.GetNameXPtg(name, sheetExtIx, _uBook.GetUDFFinder());
	}

	public IEvaluationName GetName(string name, int sheetIndex)
	{
		for (int i = 0; i < _iBook.NumNames; i++)
		{
			NameRecord nameRecord = _iBook.GetNameRecord(i);
			if (nameRecord.SheetNumber == sheetIndex + 1 && name.Equals(nameRecord.NameText, StringComparison.OrdinalIgnoreCase))
			{
				return new Name(nameRecord, i);
			}
		}
		if (sheetIndex != -1)
		{
			return GetName(name, -1);
		}
		return null;
	}

	public int GetSheetIndex(IEvaluationSheet evalSheet)
	{
		HSSFSheet hSSFSheet = ((HSSFEvaluationSheet)evalSheet).HSSFSheet;
		return _uBook.GetSheetIndex(hSSFSheet);
	}

	public int GetSheetIndex(string sheetName)
	{
		return _uBook.GetSheetIndex(sheetName);
	}

	public string GetSheetName(int sheetIndex)
	{
		return _uBook.GetSheetName(sheetIndex);
	}

	public IEvaluationSheet GetSheet(int sheetIndex)
	{
		return new HSSFEvaluationSheet((HSSFSheet)_uBook.GetSheetAt(sheetIndex));
	}

	public int ConvertFromExternSheetIndex(int externSheetIndex)
	{
		return _iBook.GetFirstSheetIndexFromExternSheetIndex(externSheetIndex);
	}

	public ExternalSheet GetExternalSheet(int externSheetIndex)
	{
		ExternalSheet externalSheet = _iBook.GetExternalSheet(externSheetIndex);
		if (externalSheet == null)
		{
			int num = ConvertFromExternSheetIndex(externSheetIndex);
			switch (num)
			{
			case -1:
				return null;
			case -2:
				return null;
			}
			string sheetName = GetSheetName(num);
			int lastSheetIndexFromExternSheetIndex = _iBook.GetLastSheetIndexFromExternSheetIndex(externSheetIndex);
			if (lastSheetIndexFromExternSheetIndex == num)
			{
				externalSheet = new ExternalSheet(null, sheetName);
			}
			else
			{
				string sheetName2 = GetSheetName(lastSheetIndexFromExternSheetIndex);
				externalSheet = new ExternalSheetRange(null, sheetName, sheetName2);
			}
		}
		return externalSheet;
	}

	public ExternalSheet GetExternalSheet(string firstSheetName, string lastSheetName, int externalWorkbookNumber)
	{
		throw new InvalidOperationException("XSSF-style external references are not supported for HSSF");
	}

	public string ResolveNameXText(NameXPtg n)
	{
		return _iBook.ResolveNameXText(n.SheetRefIndex, n.NameIndex);
	}

	public string GetSheetFirstNameByExternSheet(int externSheetIndex)
	{
		return _iBook.FindSheetFirstNameFromExternSheet(externSheetIndex);
	}

	public string GetSheetLastNameByExternSheet(int externSheetIndex)
	{
		return _iBook.FindSheetLastNameFromExternSheet(externSheetIndex);
	}

	public string GetNameText(NamePtg namePtg)
	{
		return _iBook.GetNameRecord(namePtg.Index).NameText;
	}

	public IEvaluationName GetName(NamePtg namePtg)
	{
		int index = namePtg.Index;
		return new Name(_iBook.GetNameRecord(index), index);
	}

	public Ptg[] GetFormulaTokens(IEvaluationCell evalCell)
	{
		return ((FormulaRecordAggregate)((HSSFCell)((HSSFEvaluationCell)evalCell).HSSFCell).CellValueRecord).FormulaTokens;
	}

	public UDFFinder GetUDFFinder()
	{
		return _uBook.GetUDFFinder();
	}

	private int GetSheetExtIx(SheetIdentifier sheetIden)
	{
		if (sheetIden == null)
		{
			return -1;
		}
		string bookName = sheetIden.BookName;
		string name = sheetIden.SheetId.Name;
		string text = name;
		if (sheetIden is SheetRangeIdentifier)
		{
			text = ((SheetRangeIdentifier)sheetIden).LastSheetIdentifier.Name;
		}
		if (bookName == null)
		{
			int sheetIndex = _uBook.GetSheetIndex(name);
			int sheetIndex2 = _uBook.GetSheetIndex(text);
			return _iBook.checkExternSheet(sheetIndex, sheetIndex2);
		}
		return _iBook.GetExternalSheetIndex(bookName, name, text);
	}

	public SpreadsheetVersion GetSpreadsheetVersion()
	{
		return SpreadsheetVersion.EXCEL97;
	}

	public ITable GetTable(string name)
	{
		throw new InvalidOperationException("XSSF-style tables are not supported for HSSF");
	}
}
