using NPOI.SS.Util;

namespace NPOI.SS.UserModel;

public interface IConditionalFormatting
{
	int NumberOfRules { get; }

	CellRangeAddress[] GetFormattingRanges();

	void SetRule(int idx, IConditionalFormattingRule cfRule);

	void AddRule(IConditionalFormattingRule cfRule);

	IConditionalFormattingRule GetRule(int idx);
}
