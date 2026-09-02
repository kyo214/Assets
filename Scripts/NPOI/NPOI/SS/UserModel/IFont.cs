using System;

namespace NPOI.SS.UserModel;

public interface IFont
{
	string FontName { get; set; }

	double FontHeight { get; set; }

	double FontHeightInPoints { get; set; }

	bool IsItalic { get; set; }

	bool IsStrikeout { get; set; }

	short Color { get; set; }

	FontSuperScript TypeOffset { get; set; }

	FontUnderlineType Underline { get; set; }

	short Charset { get; set; }

	short Index { get; }

	[Obsolete("deprecated POI 3.15 beta 2. Use IsBold instead.")]
	short Boldweight { get; set; }

	bool IsBold { get; set; }

	void CloneStyleFrom(IFont src);
}
