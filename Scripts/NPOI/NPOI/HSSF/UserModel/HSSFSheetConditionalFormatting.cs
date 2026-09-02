using System;
using NPOI.HSSF.Record;
using NPOI.HSSF.Record.Aggregates;
using NPOI.SS.UserModel;
using NPOI.SS.Util;

namespace NPOI.HSSF.UserModel;

public class HSSFSheetConditionalFormatting : ISheetConditionalFormatting
{
	private HSSFSheet _sheet;

	private ConditionalFormattingTable _conditionalFormattingTable;

	public int NumConditionalFormattings => _conditionalFormattingTable.Count;

	public HSSFSheetConditionalFormatting(HSSFSheet sheet)
	{
		_sheet = sheet;
		_conditionalFormattingTable = sheet.Sheet.ConditionalFormattingTable;
	}

	public IConditionalFormattingRule CreateConditionalFormattingRule(ComparisonOperator comparisonOperation, string formula1, string formula2)
	{
		CFRuleRecord pRuleRecord = CFRuleRecord.Create(_sheet, (byte)comparisonOperation, formula1, formula2);
		return new HSSFConditionalFormattingRule(_sheet, pRuleRecord);
	}

	public IConditionalFormattingRule CreateConditionalFormattingRule(ComparisonOperator comparisonOperation, string formula1)
	{
		CFRuleRecord pRuleRecord = CFRuleRecord.Create(_sheet, (byte)comparisonOperation, formula1, null);
		return new HSSFConditionalFormattingRule(_sheet, pRuleRecord);
	}

	public IConditionalFormattingRule CreateConditionalFormattingRule(string formula)
	{
		CFRuleRecord pRuleRecord = CFRuleRecord.Create(_sheet, formula);
		return new HSSFConditionalFormattingRule(_sheet, pRuleRecord);
	}

	public IConditionalFormattingRule CreateConditionalFormattingRule(IconSet iconSet)
	{
		CFRule12Record pRuleRecord = CFRule12Record.Create(_sheet, iconSet);
		return new HSSFConditionalFormattingRule(_sheet, pRuleRecord);
	}

	public HSSFConditionalFormattingRule CreateConditionalFormattingRule(HSSFExtendedColor color)
	{
		CFRule12Record pRuleRecord = CFRule12Record.Create(_sheet, color.ExtendedColor);
		return new HSSFConditionalFormattingRule(_sheet, pRuleRecord);
	}

	public IConditionalFormattingRule CreateConditionalFormattingRule(ExtendedColor color)
	{
		return CreateConditionalFormattingRule((HSSFExtendedColor)color);
	}

	public IConditionalFormattingRule CreateConditionalFormattingColorScaleRule()
	{
		CFRule12Record pRuleRecord = CFRule12Record.CreateColorScale(_sheet);
		return new HSSFConditionalFormattingRule(_sheet, pRuleRecord);
	}

	public int AddConditionalFormatting(HSSFConditionalFormatting cf)
	{
		CFRecordsAggregate cfAggregate = cf.CFRecordsAggregate.CloneCFAggregate();
		return _conditionalFormattingTable.Add(cfAggregate);
	}

	public int AddConditionalFormatting(IConditionalFormatting cf)
	{
		return AddConditionalFormatting((HSSFConditionalFormatting)cf);
	}

	public int AddConditionalFormatting(CellRangeAddress[] regions, IConditionalFormattingRule[] cfRules)
	{
		if (regions == null)
		{
			throw new ArgumentException("regions must not be null");
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
		CFRuleBase[] array = new CFRuleBase[cfRules.Length];
		for (int i = 0; i != cfRules.Length; i++)
		{
			array[i] = ((HSSFConditionalFormattingRule)cfRules[i]).CfRuleRecord;
		}
		CFRecordsAggregate cfAggregate = new CFRecordsAggregate(regions, array);
		return _conditionalFormattingTable.Add(cfAggregate);
	}

	public int AddConditionalFormatting(CellRangeAddress[] regions, HSSFConditionalFormattingRule rule1)
	{
		IConditionalFormattingRule[] cfRules = ((rule1 == null) ? null : new HSSFConditionalFormattingRule[1] { rule1 });
		return AddConditionalFormatting(regions, cfRules);
	}

	public int AddConditionalFormatting(CellRangeAddress[] regions, IConditionalFormattingRule rule1)
	{
		return AddConditionalFormatting(regions, (HSSFConditionalFormattingRule)rule1);
	}

	public int AddConditionalFormatting(CellRangeAddress[] regions, IConditionalFormattingRule rule1, IConditionalFormattingRule rule2)
	{
		IConditionalFormattingRule[] cfRules = new HSSFConditionalFormattingRule[2]
		{
			(HSSFConditionalFormattingRule)rule1,
			(HSSFConditionalFormattingRule)rule2
		};
		return AddConditionalFormatting(regions, cfRules);
	}

	public IConditionalFormatting GetConditionalFormattingAt(int index)
	{
		CFRecordsAggregate cFRecordsAggregate = _conditionalFormattingTable.Get(index);
		if (cFRecordsAggregate == null)
		{
			return null;
		}
		return new HSSFConditionalFormatting(_sheet, cFRecordsAggregate);
	}

	public void RemoveConditionalFormatting(int index)
	{
		_conditionalFormattingTable.Remove(index);
	}
}
