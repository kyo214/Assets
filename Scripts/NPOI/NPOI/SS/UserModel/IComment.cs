using NPOI.SS.Util;

namespace NPOI.SS.UserModel;

public interface IComment
{
	bool Visible { get; set; }

	CellAddress Address { get; set; }

	int Row { get; set; }

	int Column { get; set; }

	string Author { get; set; }

	IRichTextString String { get; set; }

	IClientAnchor ClientAnchor { get; }

	void SetAddress(int row, int col);
}
