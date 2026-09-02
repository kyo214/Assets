namespace NPOI.SS.UserModel;

public interface IDrawing
{
	IPicture CreatePicture(IClientAnchor anchor, int pictureIndex);

	IComment CreateCellComment(IClientAnchor anchor);

	IChart CreateChart(IClientAnchor anchor);

	IClientAnchor CreateAnchor(int dx1, int dy1, int dx2, int dy2, int col1, int row1, int col2, int row2);
}
