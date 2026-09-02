namespace NPOI.SS.UserModel;

public interface IClientAnchor
{
	int Col1 { get; set; }

	int Col2 { get; set; }

	int Row1 { get; set; }

	int Row2 { get; set; }

	int Dx1 { get; set; }

	int Dy1 { get; set; }

	int Dy2 { get; set; }

	int Dx2 { get; set; }

	AnchorType AnchorType { get; set; }
}
