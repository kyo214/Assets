using NPOI.SS.Util;

namespace NPOI.SS.UserModel;

public interface ISheetConditionalFormatting
{
	int NumConditionalFormattings { get; }

	int AddConditionalFormatting(CellRangeAddress[] regions, IConditionalFormattingRule rule);

	int AddConditionalFormatting(CellRangeAddress[] regions, IConditionalFormattingRule rule1, IConditionalFormattingRule rule2);

	int AddConditionalFormatting(CellRangeAddress[] regions, IConditionalFormattingRule[] cfRules);

	int AddConditionalFormatting(IConditionalFormatting cf);

	IConditionalFormattingRule CreateConditionalFormattingRule(ComparisonOperator comparisonOperation, string formula1, string formula2);

	IConditionalFormattingRule CreateConditionalFormattingRule(ComparisonOperator comparisonOperation, string formula);

	IConditionalFormattingRule CreateConditionalFormattingRule(string formula);

	IConditionalFormattingRule CreateConditionalFormattingRule(ExtendedColor color);

	IConditionalFormattingRule CreateConditionalFormattingRule(IconSet iconSet);

	IConditionalFormattingRule CreateConditionalFormattingColorScaleRule();

	IConditionalFormatting GetConditionalFormattingAt(int index);

	void RemoveConditionalFormatting(int index);
}
