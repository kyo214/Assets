using System;
using NPOI.HSSF.Record;
using NPOI.HSSF.Record.Aggregates;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.HSSF.UserModel;

public class HSSFConditionalFormatting : IConditionalFormatting
{
	private HSSFSheet sheet;

	private CFRecordsAggregate cfAggregate;

	public CFRecordsAggregate CFRecordsAggregate => cfAggregate;

	public int NumberOfRules => cfAggregate.NumberOfRules;

	public HSSFConditionalFormatting(HSSFSheet sheet, CFRecordsAggregate cfAggregate)
	{
		if (sheet == null)
		{
			throw new ArgumentException("workbook must not be null");
		}
		if (cfAggregate == null)
		{
			throw new ArgumentException("cfAggregate must not be null");
		}
		this.sheet = sheet;
		this.cfAggregate = cfAggregate;
	}

	public CellRangeAddress[] GetFormattingRanges()
	{
		return cfAggregate.Header.CellRanges;
	}

	public void SetRule(int idx, HSSFConditionalFormattingRule cfRule)
	{
		cfAggregate.SetRule(idx, cfRule.CfRuleRecord);
	}

	public void SetRule(int idx, IConditionalFormattingRule cfRule)
	{
		SetRule(idx, (HSSFConditionalFormattingRule)cfRule);
	}

	public void AddRule(HSSFConditionalFormattingRule cfRule)
	{
		cfAggregate.AddRule(cfRule.CfRuleRecord);
	}

	public void AddRule(IConditionalFormattingRule cfRule)
	{
		AddRule((HSSFConditionalFormattingRule)cfRule);
	}

	public IConditionalFormattingRule GetRule(int idx)
	{
		CFRuleBase rule = cfAggregate.GetRule(idx);
		return new HSSFConditionalFormattingRule(sheet, rule);
	}

	public override string ToString()
	{
		return cfAggregate.ToString();
	}
}
