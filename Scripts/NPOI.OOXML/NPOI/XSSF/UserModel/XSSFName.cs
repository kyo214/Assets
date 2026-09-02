using System;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.XSSF.UserModel;

public class XSSFName : IName
{
	public static string BUILTIN_PRINT_AREA = "_xlnm.Print_Area";

	public static string BUILTIN_PRINT_TITLE = "_xlnm.Print_Titles";

	public static string BUILTIN_CRITERIA = "_xlnm.Criteria:";

	public static string BUILTIN_EXTRACT = "_xlnm.Extract:";

	public static string BUILTIN_FILTER_DB = "_xlnm._FilterDatabase";

	public static string BUILTIN_CONSOLIDATE_AREA = "_xlnm.Consolidate_Area";

	public static string BUILTIN_DATABASE = "_xlnm.Database";

	public static string BUILTIN_SHEET_TITLE = "_xlnm.Sheet_Title";

	private XSSFWorkbook _workbook;

	private CT_DefinedName _ctName;

	public string NameName
	{
		get
		{
			return _ctName.name;
		}
		set
		{
			ValidateName(value);
			string nameName = NameName;
			int sheetIndex = SheetIndex;
			foreach (XSSFName name in _workbook.GetNames(value))
			{
				if (name != this && sheetIndex == name.SheetIndex)
				{
					throw new ArgumentException("The " + ((sheetIndex == -1) ? "workbook" : "sheet") + " already contains this name: " + value);
				}
			}
			_ctName.name = value;
			_workbook.UpdateName(this, nameName);
		}
	}

	public string RefersToFormula
	{
		get
		{
			string value = _ctName.Value;
			if (value == null || value.Length < 1)
			{
				return null;
			}
			return value;
		}
		set
		{
			XSSFEvaluationWorkbook workbook = XSSFEvaluationWorkbook.Create(_workbook);
			FormulaParser.Parse(value, workbook, FormulaType.NamedRange, SheetIndex, -1);
			_ctName.Value = value;
		}
	}

	public bool IsDeleted
	{
		get
		{
			string refersToFormula = RefersToFormula;
			if (refersToFormula == null)
			{
				return false;
			}
			XSSFEvaluationWorkbook workbook = XSSFEvaluationWorkbook.Create(_workbook);
			return Ptg.DoesFormulaReferToDeletedCell(FormulaParser.Parse(refersToFormula, workbook, FormulaType.NamedRange, SheetIndex, -1));
		}
	}

	public int SheetIndex
	{
		get
		{
			if (!_ctName.IsSetLocalSheetId())
			{
				return -1;
			}
			return (int)_ctName.localSheetId;
		}
		set
		{
			int num = _workbook.NumberOfSheets - 1;
			if (value < -1 || value > num)
			{
				throw new ArgumentException("Sheet index (" + value + ") is out of range" + ((num == -1) ? "" : (" (0.." + num + ")")));
			}
			if (value == -1)
			{
				if (_ctName.IsSetLocalSheetId())
				{
					_ctName.UnsetLocalSheetId();
				}
			}
			else
			{
				_ctName.localSheetId = (uint)value;
				_ctName.localSheetIdSpecified = true;
			}
		}
	}

	public bool Function
	{
		get
		{
			return _ctName.function;
		}
		set
		{
			_ctName.function = value;
		}
	}

	public int FunctionGroupId
	{
		get
		{
			return (int)_ctName.functionGroupId;
		}
		set
		{
			_ctName.functionGroupId = (uint)value;
		}
	}

	public string SheetName
	{
		get
		{
			if (_ctName.IsSetLocalSheetId())
			{
				int localSheetId = (int)_ctName.localSheetId;
				return _workbook.GetSheetName(localSheetId);
			}
			return new AreaReference(RefersToFormula).FirstCell.SheetName;
		}
	}

	public bool IsFunctionName => Function;

	public string Comment
	{
		get
		{
			return _ctName.comment;
		}
		set
		{
			_ctName.comment = value;
		}
	}

	public XSSFName(CT_DefinedName name, XSSFWorkbook workbook)
	{
		_workbook = workbook;
		_ctName = name;
	}

	internal CT_DefinedName GetCTName()
	{
		return _ctName;
	}

	public void SetFunction(bool value)
	{
		Function = value;
	}

	public override int GetHashCode()
	{
		return _ctName.ToString().GetHashCode();
	}

	public override bool Equals(object o)
	{
		if (o == this)
		{
			return true;
		}
		if (!(o is XSSFName))
		{
			return false;
		}
		XSSFName xSSFName = (XSSFName)o;
		if (_ctName.name == xSSFName.GetCTName().name && _ctName.localSheetId == xSSFName.GetCTName().localSheetId)
		{
			return _ctName.Value == xSSFName.RefersToFormula;
		}
		return false;
	}

	private static void ValidateName(string name)
	{
		if (name.Length == 0)
		{
			throw new ArgumentException("Name cannot be blank");
		}
		char c = name[0];
		string text = "_";
		if (!char.IsLetter(c) && text.IndexOf(c) == -1)
		{
			throw new ArgumentException("Invalid name: '" + name + "': first character must be underscore or a letter");
		}
		text = "_\\";
		char[] array = name.ToCharArray();
		foreach (char c2 in array)
		{
			if (!char.IsLetterOrDigit(c2) && text.IndexOf(c2) == -1)
			{
				throw new ArgumentException("Invalid name: '" + name + "'");
			}
		}
	}
}
