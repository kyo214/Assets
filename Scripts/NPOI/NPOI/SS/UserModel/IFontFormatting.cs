namespace NPOI.SS.UserModel;

public interface IFontFormatting
{
	FontSuperScript EscapementType { get; set; }

	short FontColorIndex { get; set; }

	IColor FontColor { get; set; }

	int FontHeight { get; set; }

	FontUnderlineType UnderlineType { get; set; }

	bool IsBold { get; }

	bool IsItalic { get; }

	void SetFontStyle(bool italic, bool bold);

	void ResetFontStyle();
}
