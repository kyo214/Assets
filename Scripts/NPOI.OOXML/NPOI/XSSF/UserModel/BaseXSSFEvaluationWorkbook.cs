using System;
using System.Collections.Generic;
using System.Globalization;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.SS.Formula.UDF;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.Model;

namespace NPOI.XSSF.UserModel;

public abstract class BaseXSSFEvaluationWorkbook : IFormulaRenderingWorkbook, IEvaluationWorkbook, IFormulaParsingWorkbook
{
	private class FakeExternalLinksTable : ExternalLinksTable
	{
		private string fileName;

		public override string LinkedFileName => fileName;

		internal FakeExternalLinksTable(string fileName)
		{
			this.fileName = fileName;
		}
	}

	private class Name : IEvaluationName
	{
		private XSSFName _nameRecord;

		private int _index;

		private IFormulaParsingWorkbook _fpBook;

		public Ptg[] NameDefinition => FormulaParser.Parse(_nameRecord.RefersToFormula, _fpBook, FormulaType.NamedRange, _nameRecord.SheetIndex);

		public string NameText => _nameRecord.NameName;

		public bool HasFormula
		{
			get
			{
				CT_DefinedName cTName = _nameRecord.GetCTName();
				string value = cTName.Value;
				if (!cTName.function && value != null)
				{
					return value.Length > 0;
				}
				return false;
			}
		}

		public bool IsFunctionName => _nameRecord.IsFunctionName;

		public bool IsRange => HasFormula;

		public Name(XSSFName name, int index, IFormulaParsingWorkbook fpBook)
		{
			_nameRecord = name;
			_index = index;
			_fpBook = fpBook;
		}

		public NamePtg CreatePtg()
		{
			return new NamePtg(_index);
		}
	}

	protected XSSFWorkbook _uBook;

	private Dictionary<string, XSSFTable> _tableCache;

	public virtual void ClearAllCachedResultValues()
	{
		_tableCache = null;
	}

	protected BaseXSSFEvaluationWorkbook(XSSFWorkbook book)
	{
		_uBook = book;
	}

	private int ConvertFromExternalSheetIndex(int externSheetIndex)
	{
		return externSheetIndex;
	}

	public int ConvertFromExternSheetIndex(int externSheetIndex)
	{
		return externSheetIndex;
	}

	private int ConvertToExternalSheetIndex(int sheetIndex)
	{
		return sheetIndex;
	}

	public int GetExternalSheetIndex(string sheetName)
	{
		int sheetIndex = _uBook.GetSheetIndex(sheetName);
		return ConvertToExternalSheetIndex(sheetIndex);
	}

	private int ResolveBookIndex(string bookName)
	{
		if (bookName.StartsWith("[") && bookName.EndsWith("]"))
		{
			bookName = bookName.Substring(1, bookName.Length - 2);
		}
		try
		{
			return int.Parse(bookName);
		}
		catch (FormatException)
		{
		}
		List<ExternalLinksTable> externalLinksTable = _uBook.ExternalLinksTable;
		int num = FindExternalLinkIndex(bookName, externalLinksTable);
		if (num != -1)
		{
			return num;
		}
		if (bookName.StartsWith("'file:///") && bookName.EndsWith("'"))
		{
			string text = bookName.Substring(bookName.LastIndexOf('/') + 1);
			text = text.Substring(0, text.Length - 1);
			num = FindExternalLinkIndex(text, externalLinksTable);
			if (num != -1)
			{
				return num;
			}
			ExternalLinksTable item = new FakeExternalLinksTable(text);
			externalLinksTable.Add(item);
			return externalLinksTable.Count;
		}
		throw new Exception("Book not linked for filename " + bookName);
	}

	private int FindExternalLinkIndex(string bookName, List<ExternalLinksTable> tables)
	{
		int num = 0;
		foreach (ExternalLinksTable table in tables)
		{
			if (table.LinkedFileName.Equals(bookName))
			{
				return num + 1;
			}
			num++;
		}
		return -1;
	}

	public IEvaluationName GetName(string name, int sheetIndex)
	{
		for (int i = 0; i < _uBook.NumberOfNames; i++)
		{
			XSSFName xSSFName = _uBook.GetNameAt(i) as XSSFName;
			string nameName = xSSFName.NameName;
			int sheetIndex2 = xSSFName.SheetIndex;
			if (name.Equals(nameName, StringComparison.CurrentCultureIgnoreCase) && (sheetIndex2 == -1 || sheetIndex2 == sheetIndex))
			{
				return new Name(xSSFName, i, this);
			}
		}
		if (sheetIndex != -1)
		{
			return GetName(name, -1);
		}
		return null;
	}

	public string GetSheetName(int sheetIndex)
	{
		return _uBook.GetSheetName(sheetIndex);
	}

	public ExternalName GetExternalName(int externSheetIndex, int externNameIndex)
	{
		throw new InvalidOperationException("HSSF-style external references are not supported for XSSF");
	}

	public ExternalName GetExternalName(string nameName, string sheetName, int externalWorkbookNumber)
	{
		if (externalWorkbookNumber > 0)
		{
			int index = externalWorkbookNumber - 1;
			ExternalLinksTable externalLinksTable = _uBook.ExternalLinksTable[index];
			foreach (IName definedName in externalLinksTable.DefinedNames)
			{
				if (definedName.NameName.Equals(nameName))
				{
					int ix = definedName.SheetIndex + 1;
					return new ExternalName(nameName, -1, ix);
				}
			}
			throw new ArgumentException("Name '" + nameName + "' not found in reference to " + externalLinksTable.LinkedFileName);
		}
		int nameIndex = _uBook.GetNameIndex(nameName);
		return new ExternalName(nameName, nameIndex, 0);
	}

	public Ptg GetNameXPtg(string name, SheetIdentifier sheet)
	{
		if (((IndexedUDFFinder)GetUDFFinder()).FindFunction(name) != null)
		{
			return new NameXPxg(null, name);
		}
		if (sheet == null)
		{
			if (_uBook.GetNames(name).Count > 0)
			{
				return new NameXPxg(null, name);
			}
			return null;
		}
		if (sheet._sheetIdentifier == null)
		{
			return new NameXPxg(ResolveBookIndex(sheet._bookName), null, name);
		}
		string name2 = sheet._sheetIdentifier.Name;
		if (sheet._bookName != null)
		{
			return new NameXPxg(ResolveBookIndex(sheet._bookName), name2, name);
		}
		return new NameXPxg(name2, name);
	}

	public Ptg Get3DReferencePtg(CellReference cell, SheetIdentifier sheet)
	{
		if (sheet._bookName != null)
		{
			return new Ref3DPxg(ResolveBookIndex(sheet._bookName), sheet, cell);
		}
		return new Ref3DPxg(sheet, cell);
	}

	public Ptg Get3DReferencePtg(AreaReference area, SheetIdentifier sheet)
	{
		if (sheet._bookName != null)
		{
			return new Area3DPxg(ResolveBookIndex(sheet._bookName), sheet, area);
		}
		return new Area3DPxg(sheet, area);
	}

	public string ResolveNameXText(NameXPtg n)
	{
		int nameIndex = n.NameIndex;
		string text = null;
		text = ((IndexedUDFFinder)GetUDFFinder()).GetFunctionName(nameIndex);
		if (text != null)
		{
			return text;
		}
		if (_uBook.GetNameAt(nameIndex) is XSSFName xSSFName)
		{
			text = xSSFName.NameName;
		}
		return text;
	}

	public ExternalSheet GetExternalSheet(int externSheetIndex)
	{
		throw new InvalidOperationException("HSSF-style external references are not supported for XSSF");
	}

	public ExternalSheet GetExternalSheet(string firstSheetName, string lastSheetName, int externalWorkbookNumber)
	{
		string workbookName;
		if (externalWorkbookNumber > 0)
		{
			int index = externalWorkbookNumber - 1;
			workbookName = _uBook.ExternalLinksTable[index].LinkedFileName;
		}
		else
		{
			workbookName = null;
		}
		if (lastSheetName == null || firstSheetName.Equals(lastSheetName))
		{
			return new ExternalSheet(workbookName, firstSheetName);
		}
		return new ExternalSheetRange(workbookName, firstSheetName, lastSheetName);
	}

	public int GetExternalSheetIndex(string workbookName, string sheetName)
	{
		throw new Exception("not implemented yet");
	}

	public int GetSheetIndex(string sheetName)
	{
		return _uBook.GetSheetIndex(sheetName);
	}

	public string GetSheetFirstNameByExternSheet(int externSheetIndex)
	{
		int sheetIx = ConvertFromExternalSheetIndex(externSheetIndex);
		return _uBook.GetSheetName(sheetIx);
	}

	public string GetSheetLastNameByExternSheet(int externSheetIndex)
	{
		return GetSheetFirstNameByExternSheet(externSheetIndex);
	}

	public string GetNameText(NamePtg namePtg)
	{
		return _uBook.GetNameAt(namePtg.Index).NameName;
	}

	public IEvaluationName GetName(NamePtg namePtg)
	{
		int index = namePtg.Index;
		return new Name(_uBook.GetNameAt(index) as XSSFName, index, this);
	}

	public IName CreateName()
	{
		return _uBook.CreateName();
	}

	private Dictionary<string, XSSFTable> GetTableCache()
	{
		if (_tableCache != null)
		{
			return _tableCache;
		}
		_tableCache = new Dictionary<string, XSSFTable>();
		foreach (XSSFSheet item in _uBook)
		{
			foreach (XSSFTable table in item.GetTables())
			{
				string key = table.Name.ToLower(CultureInfo.CurrentCulture);
				_tableCache.Add(key, table);
			}
		}
		return _tableCache;
	}

	public ITable GetTable(string name)
	{
		if (name == null)
		{
			return null;
		}
		string key = name.ToLower(CultureInfo.CurrentCulture);
		return GetTableCache()[key];
	}

	public UDFFinder GetUDFFinder()
	{
		return _uBook.GetUDFFinder();
	}

	public SpreadsheetVersion GetSpreadsheetVersion()
	{
		return SpreadsheetVersion.EXCEL2007;
	}

	public abstract int GetSheetIndex(IEvaluationSheet sheet);

	public abstract IEvaluationSheet GetSheet(int sheetIndex);

	public abstract Ptg[] GetFormulaTokens(IEvaluationCell cell);
}
