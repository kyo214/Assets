namespace NPOI.SS.UserModel.Charts;

public interface IChartAxisFactory
{
	IValueAxis CreateValueAxis(AxisPosition pos);

	IChartAxis CreateCategoryAxis(AxisPosition pos);

	IChartAxis CreateDateAxis(AxisPosition pos);
}
