namespace NPOI.WP.UserModel;

public interface ICharacterRun
{
	bool IsBold { get; set; }

	bool IsItalic { get; set; }

	bool IsSmallCaps { get; set; }

	bool IsCapitalized { get; set; }

	bool IsStrikeThrough { get; set; }

	bool IsDoubleStrikeThrough { get; set; }

	bool IsShadowed { get; set; }

	bool IsEmbossed { get; set; }

	bool IsImprinted { get; set; }

	double FontSize { get; set; }

	int CharacterSpacing { get; set; }

	int Kerning { get; set; }

	bool IsHighlighted { get; }

	string FontName { get; }

	string Text { get; }
}
