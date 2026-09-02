namespace NPOI.SS.UserModel.Charts;

public interface IBarChartSeries<Tx, Ty> : IChartSeries
{
	IChartDataSource<Tx> GetCategoryAxisData();

	IChartDataSource<Ty> GetValues();
}
