namespace NPOI.SS.UserModel.Charts;

public interface IChartDataFactory
{
	IScatterChartData<Tx, Ty> CreateScatterChartData<Tx, Ty>();

	ILineChartData<Tx, Ty> CreateLineChartData<Tx, Ty>();

	IBarChartData<Tx, Ty> CreateBarChartData<Tx, Ty>();
}
