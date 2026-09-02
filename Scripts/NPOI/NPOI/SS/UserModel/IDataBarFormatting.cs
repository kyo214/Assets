namespace NPOI.SS.UserModel;

public interface IDataBarFormatting
{
	bool IsLeftToRight { get; set; }

	bool IsIconOnly { get; set; }

	int WidthMin { get; set; }

	int WidthMax { get; set; }

	IColor Color { get; set; }

	IConditionalFormattingThreshold MinThreshold { get; }

	IConditionalFormattingThreshold MaxThreshold { get; }
}
