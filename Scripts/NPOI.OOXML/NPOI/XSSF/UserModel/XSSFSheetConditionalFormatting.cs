using System;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.XSSF.UserModel;

public class XSSFSheetConditionalFormatting : ISheetConditionalFormatting
{
	protected static string CF_EXT_2009_NS_X14 = "http://schemas.microsoft.com/office/spreadsheetml/2009/9/main";

	private XSSFSheet _sheet;

	public int NumConditionalFormattings => _sheet.GetCTWorksheet().SizeOfConditionalFormattingArray();

	internal XSSFSheetConditionalFormatting(XSSFSheet sheet)
	{
		_sheet = sheet;
	}

	public IConditionalFormattingRule CreateConditionalFormattingRule(ComparisonOperator comparisonOperation, string formula1, string formula2)
	{
		XSSFConditionalFormattingRule xSSFConditionalFormattingRule = new XSSFConditionalFormattingRule(_sheet);
		CT_CfRule cTCfRule = xSSFConditionalFormattingRule.GetCTCfRule();
		cTCfRule.AddFormula(formula1);
		if (formula2 != null)
		{
			cTCfRule.AddFormula(formula2);
		}
		cTCfRule.type = ST_CfType.cellIs;
		cTCfRule.@operator = comparisonOperation switch
		{
			ComparisonOperator.Between => ST_ConditionalFormattingOperator.between, 
			ComparisonOperator.NotBetween => ST_ConditionalFormattingOperator.notBetween, 
			ComparisonOperator.LessThan => ST_ConditionalFormattingOperator.lessThan, 
			ComparisonOperator.LessThanOrEqual => ST_ConditionalFormattingOperator.lessThanOrEqual, 
			ComparisonOperator.GreaterThan => ST_ConditionalFormattingOperator.greaterThan, 
			ComparisonOperator.GreaterThanOrEqual => ST_ConditionalFormattingOperator.greaterThanOrEqual, 
			ComparisonOperator.Equal => ST_ConditionalFormattingOperator.equal, 
			ComparisonOperator.NotEqual => ST_ConditionalFormattingOperator.notEqual, 
			_ => throw new ArgumentException("Unknown comparison operator: " + comparisonOperation), 
		};
		return xSSFConditionalFormattingRule;
	}

	public IConditionalFormattingRule CreateConditionalFormattingRule(ComparisonOperator comparisonOperation, string formula)
	{
		return CreateConditionalFormattingRule(comparisonOperation, formula, null);
	}

	public IConditionalFormattingRule CreateConditionalFormattingRule(string formula)
	{
		XSSFConditionalFormattingRule xSSFConditionalFormattingRule = new XSSFConditionalFormattingRule(_sheet);
		CT_CfRule cTCfRule = xSSFConditionalFormattingRule.GetCTCfRule();
		cTCfRule.AddFormula(formula);
		cTCfRule.type = ST_CfType.expression;
		return xSSFConditionalFormattingRule;
	}

	public IConditionalFormattingRule CreateConditionalFormattingRule(IconSet iconSet)
	{
		XSSFConditionalFormattingRule xSSFConditionalFormattingRule = new XSSFConditionalFormattingRule(_sheet);
		xSSFConditionalFormattingRule.CreateMultiStateFormatting(iconSet);
		return xSSFConditionalFormattingRule;
	}

	public XSSFConditionalFormattingRule CreateConditionalFormattingRule(XSSFColor color)
	{
		XSSFConditionalFormattingRule xSSFConditionalFormattingRule = new XSSFConditionalFormattingRule(_sheet);
		xSSFConditionalFormattingRule.CreateDataBarFormatting(color);
		return xSSFConditionalFormattingRule;
	}

	public IConditionalFormattingRule CreateConditionalFormattingRule(ExtendedColor color)
	{
		return CreateConditionalFormattingRule((XSSFColor)color);
	}

	public IConditionalFormattingRule CreateConditionalFormattingColorScaleRule()
	{
		XSSFConditionalFormattingRule xSSFConditionalFormattingRule = new XSSFConditionalFormattingRule(_sheet);
		xSSFConditionalFormattingRule.CreateColorScaleFormatting();
		return xSSFConditionalFormattingRule;
	}

	public int AddConditionalFormatting(CellRangeAddress[] regions, IConditionalFormattingRule[] cfRules)
	{
		if (regions == null)
		{
			throw new ArgumentException("regions must not be null");
		}
		CellRangeAddress[] array = regions;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Validate(SpreadsheetVersion.EXCEL2007);
		}
		if (cfRules == null)
		{
			throw new ArgumentException("cfRules must not be null");
		}
		if (cfRules.Length == 0)
		{
			throw new ArgumentException("cfRules must not be empty");
		}
		if (cfRules.Length > 3)
		{
			throw new ArgumentException("Number of rules must not exceed 3");
		}
		CellRangeAddress[] array2 = CellRangeUtil.MergeCellRanges(regions);
		CT_ConditionalFormatting cT_ConditionalFormatting = _sheet.GetCTWorksheet().AddNewConditionalFormatting();
		string text = string.Empty;
		array = array2;
		foreach (CellRangeAddress cellRangeAddress in array)
		{
			text = ((text.Length != 0) ? (text + " " + cellRangeAddress.FormatAsString()) : cellRangeAddress.FormatAsString());
		}
		cT_ConditionalFormatting.sqref = text;
		int num = 1;
		foreach (CT_ConditionalFormatting item in _sheet.GetCTWorksheet().conditionalFormatting)
		{
			num += item.sizeOfCfRuleArray();
		}
		for (int i = 0; i < cfRules.Length; i++)
		{
			XSSFConditionalFormattingRule xSSFConditionalFormattingRule = (XSSFConditionalFormattingRule)cfRules[i];
			xSSFConditionalFormattingRule.GetCTCfRule().priority = num++;
			cT_ConditionalFormatting.AddNewCfRule().Set(xSSFConditionalFormattingRule.GetCTCfRule());
		}
		return _sheet.GetCTWorksheet().SizeOfConditionalFormattingArray() - 1;
	}

	public int AddConditionalFormatting(CellRangeAddress[] regions, IConditionalFormattingRule rule1)
	{
		IConditionalFormattingRule[] cfRules = ((rule1 == null) ? null : new XSSFConditionalFormattingRule[1] { (XSSFConditionalFormattingRule)rule1 });
		return AddConditionalFormatting(regions, cfRules);
	}

	public int AddConditionalFormatting(CellRangeAddress[] regions, IConditionalFormattingRule rule1, IConditionalFormattingRule rule2)
	{
		IConditionalFormattingRule[] cfRules = ((rule1 == null) ? null : new XSSFConditionalFormattingRule[2]
		{
			(XSSFConditionalFormattingRule)rule1,
			(XSSFConditionalFormattingRule)rule2
		});
		return AddConditionalFormatting(regions, cfRules);
	}

	public int AddConditionalFormatting(IConditionalFormatting cf)
	{
		XSSFConditionalFormatting xSSFConditionalFormatting = (XSSFConditionalFormatting)cf;
		CT_Worksheet cTWorksheet = _sheet.GetCTWorksheet();
		cTWorksheet.AddNewConditionalFormatting().Set(xSSFConditionalFormatting.GetCTConditionalFormatting());
		return cTWorksheet.SizeOfConditionalFormattingArray() - 1;
	}

	public IConditionalFormatting GetConditionalFormattingAt(int index)
	{
		CheckIndex(index);
		CT_ConditionalFormatting conditionalFormattingArray = _sheet.GetCTWorksheet().GetConditionalFormattingArray(index);
		return new XSSFConditionalFormatting(_sheet, conditionalFormattingArray);
	}

	public void RemoveConditionalFormatting(int index)
	{
		CheckIndex(index);
		_sheet.GetCTWorksheet().conditionalFormatting.RemoveAt(index);
	}

	private void CheckIndex(int index)
	{
		int numConditionalFormattings = NumConditionalFormattings;
		if (index < 0 || index >= numConditionalFormattings)
		{
			throw new ArgumentException("Specified CF index " + index + " is outside the allowable range (0.." + (numConditionalFormattings - 1) + ")");
		}
	}
}
