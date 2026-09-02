namespace NPOI.SS.UserModel.Charts;

public interface ILineChartSeries<Tx, Ty> : IChartSeries
{
	IChartDataSource<Tx> GetCategoryAxisData();

	IChartDataSource<Ty> GetValues();
}
