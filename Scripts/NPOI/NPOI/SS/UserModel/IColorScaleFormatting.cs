namespace NPOI.SS.UserModel;

public interface IColorScaleFormatting
{
	int NumControlPoints { get; set; }

	IColor[] Colors { get; set; }

	IConditionalFormattingThreshold[] Thresholds { get; set; }

	IConditionalFormattingThreshold CreateThreshold();
}
