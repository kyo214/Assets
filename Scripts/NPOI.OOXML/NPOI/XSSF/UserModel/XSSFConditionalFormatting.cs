using System.Collections.Generic;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.XSSF.UserModel;

public class XSSFConditionalFormatting : IConditionalFormatting
{
	private CT_ConditionalFormatting _cf;

	private XSSFSheet _sh;

	public int NumberOfRules => _cf.sizeOfCfRuleArray();

	internal XSSFConditionalFormatting(XSSFSheet sh)
	{
		_cf = new CT_ConditionalFormatting();
		_sh = sh;
	}

	internal XSSFConditionalFormatting(XSSFSheet sh, CT_ConditionalFormatting cf)
	{
		_cf = cf;
		_sh = sh;
	}

	internal CT_ConditionalFormatting GetCTConditionalFormatting()
	{
		return _cf;
	}

	public CellRangeAddress[] GetFormattingRanges()
	{
		List<CellRangeAddress> list = new List<CellRangeAddress>();
		string[] array = _cf.sqref.Split(new char[1] { ' ' });
		for (int i = 0; i < array.Length; i++)
		{
			list.Add(CellRangeAddress.ValueOf(array[i]));
		}
		return list.ToArray();
	}

	public void SetRule(int idx, IConditionalFormattingRule cfRule)
	{
		XSSFConditionalFormattingRule xSSFConditionalFormattingRule = (XSSFConditionalFormattingRule)cfRule;
		_cf.GetCfRuleArray(idx).Set(xSSFConditionalFormattingRule.GetCTCfRule());
	}

	public void AddRule(IConditionalFormattingRule cfRule)
	{
		XSSFConditionalFormattingRule xSSFConditionalFormattingRule = (XSSFConditionalFormattingRule)cfRule;
		_cf.AddNewCfRule().Set(xSSFConditionalFormattingRule.GetCTCfRule());
	}

	public IConditionalFormattingRule GetRule(int idx)
	{
		return new XSSFConditionalFormattingRule(_sh, _cf.GetCfRuleArray(idx));
	}

	public override string ToString()
	{
		return _cf.ToString();
	}
}
