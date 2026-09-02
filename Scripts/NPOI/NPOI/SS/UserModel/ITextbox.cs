namespace NPOI.SS.UserModel;

public interface ITextbox : IShape
{
	IRichTextString String { get; set; }

	int MarginLeft { get; set; }

	int MarginRight { get; set; }

	int MarginTop { get; set; }

	int MarginBottom { get; set; }

	short HorizontalAlignment { get; set; }

	short VerticalAlignment { get; set; }
}
