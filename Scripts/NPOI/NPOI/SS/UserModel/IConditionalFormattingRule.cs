namespace NPOI.SS.UserModel;

public interface IConditionalFormattingRule
{
	IBorderFormatting BorderFormatting { get; }

	IFontFormatting FontFormatting { get; }

	IPatternFormatting PatternFormatting { get; }

	IDataBarFormatting DataBarFormatting { get; }

	IIconMultiStateFormatting MultiStateFormatting { get; }

	IColorScaleFormatting ColorScaleFormatting { get; }

	ConditionType ConditionType { get; }

	ComparisonOperator ComparisonOperation { get; }

	string Formula1 { get; }

	string Formula2 { get; }

	IBorderFormatting CreateBorderFormatting();

	IFontFormatting CreateFontFormatting();

	IPatternFormatting CreatePatternFormatting();
}
