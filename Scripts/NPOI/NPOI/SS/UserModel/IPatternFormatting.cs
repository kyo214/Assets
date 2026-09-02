namespace NPOI.SS.UserModel;

public interface IPatternFormatting
{
	short FillBackgroundColor { get; set; }

	short FillForegroundColor { get; set; }

	IColor FillBackgroundColorColor { get; set; }

	IColor FillForegroundColorColor { get; set; }

	FillPattern FillPattern { get; set; }
}
