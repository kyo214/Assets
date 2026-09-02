namespace NPOI.SS.UserModel;

public interface IPrintSetup
{
	short PaperSize { get; set; }

	short Scale { get; set; }

	short PageStart { get; set; }

	short FitWidth { get; set; }

	short FitHeight { get; set; }

	bool LeftToRight { get; set; }

	bool Landscape { get; set; }

	bool ValidSettings { get; set; }

	bool NoColor { get; set; }

	bool Draft { get; set; }

	bool Notes { get; set; }

	bool NoOrientation { get; set; }

	bool UsePage { get; set; }

	short HResolution { get; set; }

	short VResolution { get; set; }

	double HeaderMargin { get; set; }

	double FooterMargin { get; set; }

	short Copies { get; set; }

	bool EndNote { get; set; }

	DisplayCellErrorType CellError { get; set; }
}
