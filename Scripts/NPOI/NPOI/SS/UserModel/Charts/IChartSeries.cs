using NPOI.SS.Util;

namespace NPOI.SS.UserModel.Charts;

public interface IChartSeries
{
	void SetTitle(string title);

	void SetTitle(CellReference titleReference);

	string GetTitleString();

	CellReference GetTitleCellReference();

	TitleType? GetTitleType();
}
