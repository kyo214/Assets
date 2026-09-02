using System;
using System.Collections.Generic;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.Formula;
using NPOI.SS.Formula.PTG;
using NPOI.SS.UserModel;
using NPOI.SS.UserModel.Helpers;
using NPOI.SS.Util;

namespace NPOI.XSSF.UserModel.Helpers;

public class XSSFRowShifter : RowShifter
{
	public XSSFRowShifter(XSSFSheet sh)
		: base(sh)
	{
		sheet = sh;
	}

	[Obsolete("deprecated POI 3.15 beta 2. Use ShiftMergedRegions(int, int, int) instead.")]
	public List<CellRangeAddress> ShiftMerged(int startRow, int endRow, int n)
	{
		return ShiftMergedRegions(startRow, endRow, n);
	}

	public override void UpdateNamedRanges(FormulaShifter shifter)
	{
		IWorkbook workbook = sheet.Workbook;
		XSSFEvaluationWorkbook xSSFEvaluationWorkbook = XSSFEvaluationWorkbook.Create(workbook);
		foreach (IName allName in workbook.GetAllNames())
		{
			string refersToFormula = allName.RefersToFormula;
			int sheetIndex = allName.SheetIndex;
			Ptg[] ptgs = FormulaParser.Parse(refersToFormula, xSSFEvaluationWorkbook, FormulaType.NamedRange, sheetIndex, -1);
			if (shifter.AdjustFormula(ptgs, sheetIndex))
			{
				string refersToFormula2 = FormulaRenderer.ToFormulaString(xSSFEvaluationWorkbook, ptgs);
				allName.RefersToFormula = refersToFormula2;
			}
		}
	}

	public override void UpdateFormulas(FormulaShifter shifter)
	{
		UpdateSheetFormulas(sheet, shifter);
		foreach (XSSFSheet item in sheet.Workbook)
		{
			if (sheet != item)
			{
				UpdateSheetFormulas(item, shifter);
			}
		}
	}

	private void UpdateSheetFormulas(ISheet sh, FormulaShifter Shifter)
	{
		foreach (IRow item in sh)
		{
			UpdateRowFormulas(item, Shifter);
		}
	}

	public override void UpdateRowFormulas(IRow row, FormulaShifter Shifter)
	{
		XSSFSheet xSSFSheet = (XSSFSheet)row.Sheet;
		foreach (XSSFCell item in row)
		{
			CT_Cell cTCell = item.GetCTCell();
			if (!cTCell.IsSetF())
			{
				continue;
			}
			CT_CellFormula f = cTCell.f;
			string value = f.Value;
			if (value.Length > 0)
			{
				string text = ShiftFormula(row, value, Shifter);
				if (text != null)
				{
					f.Value = text;
					if (f.t == ST_CellFormulaType.shared)
					{
						int si = (int)f.si;
						xSSFSheet.GetSharedFormula(si).Value = text;
					}
				}
			}
			if (f.isSetRef())
			{
				string formula = f.@ref;
				string text2 = ShiftFormula(row, formula, Shifter);
				if (text2 != null)
				{
					f.@ref = text2;
				}
			}
		}
	}

	private static string ShiftFormula(IRow row, string formula, FormulaShifter Shifter)
	{
		ISheet sheet = row.Sheet;
		IWorkbook workbook = sheet.Workbook;
		int sheetIndex = workbook.GetSheetIndex(sheet);
		XSSFEvaluationWorkbook xSSFEvaluationWorkbook = XSSFEvaluationWorkbook.Create(workbook);
		try
		{
			Ptg[] ptgs = FormulaParser.Parse(formula, xSSFEvaluationWorkbook, FormulaType.Cell, sheetIndex, -1);
			string result = null;
			if (Shifter.AdjustFormula(ptgs, sheetIndex))
			{
				result = FormulaRenderer.ToFormulaString(xSSFEvaluationWorkbook, ptgs);
			}
			return result;
		}
		catch (FormulaParseException arg)
		{
			Console.WriteLine("Error shifting formula on row {0}, {1}", row.RowNum, arg);
			return formula;
		}
	}

	public override void UpdateConditionalFormatting(FormulaShifter Shifter)
	{
		XSSFSheet obj = (XSSFSheet)sheet;
		XSSFWorkbook obj2 = obj.Workbook as XSSFWorkbook;
		int sheetIndex = obj2.GetSheetIndex(sheet);
		XSSFEvaluationWorkbook xSSFEvaluationWorkbook = XSSFEvaluationWorkbook.Create(obj2);
		List<CT_ConditionalFormatting> conditionalFormatting = obj.GetCTWorksheet().conditionalFormatting;
		for (int num = conditionalFormatting.Count - 1; num >= 0; num--)
		{
			CT_ConditionalFormatting cT_ConditionalFormatting = conditionalFormatting[num];
			List<CellRangeAddress> list = new List<CellRangeAddress>();
			string[] array = cT_ConditionalFormatting.sqref.ToString().Split(new char[1] { ' ' });
			for (int i = 0; i < array.Length; i++)
			{
				list.Add(CellRangeAddress.ValueOf(array[i]));
			}
			bool flag = false;
			List<CellRangeAddress> list2 = new List<CellRangeAddress>();
			for (int j = 0; j < list.Count; j++)
			{
				CellRangeAddress cellRangeAddress = list[j];
				CellRangeAddress cellRangeAddress2 = ShiftRange(Shifter, cellRangeAddress, sheetIndex);
				if (cellRangeAddress2 == null)
				{
					flag = true;
					continue;
				}
				list2.Add(cellRangeAddress2);
				if (cellRangeAddress2 != cellRangeAddress)
				{
					flag = true;
				}
			}
			if (flag)
			{
				if (list2.Count == 0)
				{
					conditionalFormatting.RemoveAt(num);
					continue;
				}
				string text = string.Empty;
				foreach (CellRangeAddress item in list2)
				{
					text = ((text.Length != 0) ? (text + " " + item.FormatAsString()) : item.FormatAsString());
				}
				cT_ConditionalFormatting.sqref = text;
			}
			foreach (CT_CfRule item2 in cT_ConditionalFormatting.cfRule)
			{
				List<string> formula = item2.formula;
				for (int k = 0; k < formula.Count; k++)
				{
					Ptg[] ptgs = FormulaParser.Parse(formula[k], xSSFEvaluationWorkbook, FormulaType.Cell, sheetIndex, -1);
					if (Shifter.AdjustFormula(ptgs, sheetIndex))
					{
						string value = FormulaRenderer.ToFormulaString(xSSFEvaluationWorkbook, ptgs);
						formula[k] = value;
					}
				}
			}
		}
	}

	public override void UpdateHyperlinks(FormulaShifter shifter)
	{
		int sheetIndex = ((XSSFSheet)sheet).GetWorkbook().GetSheetIndex(sheet);
		foreach (IHyperlink hyperlink in sheet.GetHyperlinkList())
		{
			XSSFHyperlink xSSFHyperlink = hyperlink as XSSFHyperlink;
			CellRangeAddress cellRangeAddress = CellRangeAddress.ValueOf(xSSFHyperlink.CellRef);
			CellRangeAddress cellRangeAddress2 = ShiftRange(shifter, cellRangeAddress, sheetIndex);
			if (cellRangeAddress2 != null && cellRangeAddress2 != cellRangeAddress)
			{
				xSSFHyperlink.SetCellReference(cellRangeAddress2.FormatAsString());
			}
		}
	}

	private static CellRangeAddress ShiftRange(FormulaShifter Shifter, CellRangeAddress cra, int currentExternSheetIx)
	{
		AreaPtg areaPtg = new AreaPtg(cra.FirstRow, cra.LastRow, cra.FirstColumn, cra.LastColumn, firstRowRelative: false, lastRowRelative: false, firstColRelative: false, lastColRelative: false);
		Ptg[] array = new Ptg[1] { areaPtg };
		if (!Shifter.AdjustFormula(array, currentExternSheetIx))
		{
			return cra;
		}
		Ptg ptg = array[0];
		if (ptg is AreaPtg)
		{
			AreaPtg areaPtg2 = (AreaPtg)ptg;
			return new CellRangeAddress(areaPtg2.FirstRow, areaPtg2.LastRow, areaPtg2.FirstColumn, areaPtg2.LastColumn);
		}
		if (ptg is AreaErrPtg)
		{
			return null;
		}
		throw new InvalidOperationException("Unexpected Shifted ptg class (" + ptg.GetType().Name + ")");
	}
}
