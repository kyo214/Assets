namespace NPOI.SS.UserModel.Charts;

public interface IScatterChartSeries<Tx, Ty> : IChartSeries
{
	IChartDataSource<Tx> GetXValues();

	IChartDataSource<Ty> GetYValues();
}
