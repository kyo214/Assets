using System;
using System.Collections.Generic;
using NPOI.SS.Formula.PTG;
using NPOI.SS.Formula.UDF;
using NPOI.SS.UserModel;

namespace NPOI.SS.Formula.Eval.Forked;

internal class ForkedEvaluationWorkbook : IEvaluationWorkbook
{
	private IEvaluationWorkbook _masterBook;

	private Dictionary<string, ForkedEvaluationSheet> _sharedSheetsByName;

	public ForkedEvaluationWorkbook(IEvaluationWorkbook master)
	{
		_masterBook = master;
		_sharedSheetsByName = new Dictionary<string, ForkedEvaluationSheet>();
	}

	public ForkedEvaluationCell GetOrCreateUpdatableCell(string sheetName, int rowIndex, int columnIndex)
	{
		return GetSharedSheet(sheetName).GetOrCreateUpdatableCell(rowIndex, columnIndex);
	}

	public IEvaluationCell GetEvaluationCell(string sheetName, int rowIndex, int columnIndex)
	{
		return GetSharedSheet(sheetName).GetCell(rowIndex, columnIndex);
	}

	private ForkedEvaluationSheet GetSharedSheet(string sheetName)
	{
		ForkedEvaluationSheet forkedEvaluationSheet = null;
		if (_sharedSheetsByName.ContainsKey(sheetName))
		{
			forkedEvaluationSheet = _sharedSheetsByName[sheetName];
		}
		if (forkedEvaluationSheet == null)
		{
			forkedEvaluationSheet = new ForkedEvaluationSheet(_masterBook.GetSheet(_masterBook.GetSheetIndex(sheetName)));
			if (_sharedSheetsByName.ContainsKey(sheetName))
			{
				_sharedSheetsByName[sheetName] = forkedEvaluationSheet;
			}
			else
			{
				_sharedSheetsByName.Add(sheetName, forkedEvaluationSheet);
			}
		}
		return forkedEvaluationSheet;
	}

	public void CopyUpdatedCells(IWorkbook workbook)
	{
		string[] array = new string[_sharedSheetsByName.Count];
		_sharedSheetsByName.Keys.CopyTo(array, 0);
		string[] array2 = array;
		foreach (string text in array2)
		{
			_sharedSheetsByName[text].CopyUpdatedCells(workbook.GetSheet(text));
		}
	}

	public int ConvertFromExternSheetIndex(int externSheetIndex)
	{
		return _masterBook.ConvertFromExternSheetIndex(externSheetIndex);
	}

	public ExternalSheet GetExternalSheet(int externSheetIndex)
	{
		return _masterBook.GetExternalSheet(externSheetIndex);
	}

	public ExternalSheet GetExternalSheet(string firstSheetName, string lastSheetName, int externalWorkbookNumber)
	{
		return _masterBook.GetExternalSheet(firstSheetName, lastSheetName, externalWorkbookNumber);
	}

	public Ptg[] GetFormulaTokens(IEvaluationCell cell)
	{
		if (cell is ForkedEvaluationCell)
		{
			throw new Exception("Updated formulas not supported yet");
		}
		return _masterBook.GetFormulaTokens(cell);
	}

	public IEvaluationName GetName(NamePtg namePtg)
	{
		return _masterBook.GetName(namePtg);
	}

	public IEvaluationName GetName(string name, int sheetIndex)
	{
		return _masterBook.GetName(name, sheetIndex);
	}

	public IEvaluationSheet GetSheet(int sheetIndex)
	{
		return GetSharedSheet(GetSheetName(sheetIndex));
	}

	public ExternalName GetExternalName(int externSheetIndex, int externNameIndex)
	{
		return _masterBook.GetExternalName(externSheetIndex, externNameIndex);
	}

	public ExternalName GetExternalName(string nameName, string sheetName, int externalWorkbookNumber)
	{
		return _masterBook.GetExternalName(nameName, sheetName, externalWorkbookNumber);
	}

	public int GetSheetIndex(IEvaluationSheet sheet)
	{
		if (sheet is ForkedEvaluationSheet)
		{
			return ((ForkedEvaluationSheet)sheet).GetSheetIndex(_masterBook);
		}
		return _masterBook.GetSheetIndex(sheet);
	}

	public int GetSheetIndex(string sheetName)
	{
		return _masterBook.GetSheetIndex(sheetName);
	}

	public string GetSheetName(int sheetIndex)
	{
		return _masterBook.GetSheetName(sheetIndex);
	}

	public string ResolveNameXText(NameXPtg ptg)
	{
		return _masterBook.ResolveNameXText(ptg);
	}

	public UDFFinder GetUDFFinder()
	{
		return _masterBook.GetUDFFinder();
	}

	public void ClearAllCachedResultValues()
	{
		_masterBook.ClearAllCachedResultValues();
	}
}
