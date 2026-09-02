namespace NPOI.SS.UserModel;

public interface ICellStyle
{
	bool ShrinkToFit { get; set; }

	short Index { get; }

	short DataFormat { get; set; }

	short FontIndex { get; }

	bool IsHidden { get; set; }

	bool IsLocked { get; set; }

	HorizontalAlignment Alignment { get; set; }

	bool WrapText { get; set; }

	VerticalAlignment VerticalAlignment { get; set; }

	short Rotation { get; set; }

	short Indention { get; set; }

	BorderStyle BorderLeft { get; set; }

	BorderStyle BorderRight { get; set; }

	BorderStyle BorderTop { get; set; }

	BorderStyle BorderBottom { get; set; }

	short LeftBorderColor { get; set; }

	short RightBorderColor { get; set; }

	short TopBorderColor { get; set; }

	short BottomBorderColor { get; set; }

	FillPattern FillPattern { get; set; }

	short FillBackgroundColor { get; set; }

	short FillForegroundColor { get; set; }

	short BorderDiagonalColor { get; set; }

	BorderStyle BorderDiagonalLineStyle { get; set; }

	BorderDiagonal BorderDiagonal { get; set; }

	IColor FillBackgroundColorColor { get; }

	IColor FillForegroundColorColor { get; }

	string GetDataFormatString();

	void SetFont(IFont font);

	void CloneStyleFrom(ICellStyle source);

	IFont GetFont(IWorkbook parentWorkbook);
}
