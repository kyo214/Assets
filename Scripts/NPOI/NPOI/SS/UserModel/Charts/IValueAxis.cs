namespace NPOI.SS.UserModel.Charts;

public interface IValueAxis : IChartAxis
{
	AxisCrossBetween GetCrossBetween();

	void SetCrossBetween(AxisCrossBetween crossBetween);
}
