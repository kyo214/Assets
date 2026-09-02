namespace NPOI.WP.UserModel;

public interface IParagraph
{
	int IndentFromRight { get; set; }

	int IndentFromLeft { get; set; }

	int FirstLineIndent { get; set; }

	int FontAlignment { get; set; }

	bool IsWordWrapped { get; set; }
}
