namespace NPOI.SS.UserModel;

public interface IIconMultiStateFormatting
{
	IconSet IconSet { get; set; }

	bool IsIconOnly { get; set; }

	bool IsReversed { get; set; }

	IConditionalFormattingThreshold[] Thresholds { get; set; }

	IConditionalFormattingThreshold CreateThreshold();
}
