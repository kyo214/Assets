using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;

namespace NPOI.XSSF.UserModel.Helpers;

public class XSSFFormulaUtils
{
	private XSSFWorkbook _wb;

	private XSSFEvaluationWorkbook _fpwb;

	public XSSFFormulaUtils(XSSFWorkbook wb)
	{
		_wb = wb;
		_fpwb = XSSFEvaluationWorkbook.Create(_wb);
	}

	public void UpdateSheetName(int sheetIndex, string oldName, string newName)
	{
		_ = _wb.NumberOfNames;
		foreach (IName allName in _wb.GetAllNames())
		{
			if (allName.SheetIndex == -1 || allName.SheetIndex == sheetIndex)
			{
				UpdateName(allName, oldName, newName);
			}
		}
		foreach (ISheet item in _wb)
		{
			foreach (IRow item2 in item)
			{
				foreach (ICell item3 in item2)
				{
					if (item3.CellType == CellType.Formula)
					{
						UpdateFormula((XSSFCell)item3, oldName, newName);
					}
				}
			}
		}
	}

	private void UpdateFormula(XSSFCell cell, string oldName, string newName)
	{
		CT_CellFormula f = cell.GetCTCell().f;
		if (f == null)
		{
			return;
		}
		string value = f.Value;
		if (value != null && value.Length > 0)
		{
			int sheetIndex = _wb.GetSheetIndex(cell.Sheet);
			Ptg[] array = FormulaParser.Parse(value, _fpwb, FormulaType.Cell, sheetIndex, -1);
			Ptg[] array2 = array;
			foreach (Ptg ptg in array2)
			{
				UpdatePtg(ptg, oldName, newName);
			}
			string value2 = FormulaRenderer.ToFormulaString(_fpwb, array);
			if (!value.Equals(value2))
			{
				f.Value = value2;
			}
		}
	}

	private void UpdateName(IName name, string oldName, string newName)
	{
		string refersToFormula = name.RefersToFormula;
		if (refersToFormula != null)
		{
			int sheetIndex = name.SheetIndex;
			Ptg[] array = FormulaParser.Parse(refersToFormula, _fpwb, FormulaType.NamedRange, sheetIndex, -1);
			Ptg[] array2 = array;
			foreach (Ptg ptg in array2)
			{
				UpdatePtg(ptg, oldName, newName);
			}
			string text = FormulaRenderer.ToFormulaString(_fpwb, array);
			if (!refersToFormula.Equals(text))
			{
				name.RefersToFormula = text;
			}
		}
	}

	private void UpdatePtg(Ptg ptg, string oldName, string newName)
	{
		if (!(ptg is Pxg))
		{
			return;
		}
		Pxg pxg = (Pxg)ptg;
		if (pxg.ExternalWorkbookNumber >= 1)
		{
			return;
		}
		if (pxg.SheetName != null && pxg.SheetName.Equals(oldName))
		{
			pxg.SheetName = newName;
		}
		if (pxg is Pxg3D)
		{
			Pxg3D pxg3D = (Pxg3D)pxg;
			if (pxg3D.LastSheetName != null && pxg3D.LastSheetName.Equals(oldName))
			{
				pxg3D.LastSheetName = newName;
			}
		}
	}
}
