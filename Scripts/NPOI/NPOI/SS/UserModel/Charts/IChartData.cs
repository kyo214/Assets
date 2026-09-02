namespace NPOI.SS.UserModel.Charts;

public interface IChartData
{
	void FillChart(IChart chart, params IChartAxis[] axis);
}
