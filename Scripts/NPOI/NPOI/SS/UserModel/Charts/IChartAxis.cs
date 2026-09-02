namespace NPOI.SS.UserModel.Charts;

public interface IChartAxis
{
	long Id { get; }

	AxisPosition Position { get; set; }

	string NumberFormat { get; set; }

	bool IsSetLogBase { get; }

	double LogBase { get; set; }

	bool IsSetMinimum { get; }

	double Minimum { get; set; }

	bool IsSetMaximum { get; }

	double Maximum { get; set; }

	AxisOrientation Orientation { get; set; }

	AxisCrosses Crosses { get; set; }

	bool IsVisible { get; set; }

	AxisTickMark MajorTickMark { get; set; }

	AxisTickMark MinorTickMark { get; set; }

	void CrossAxis(IChartAxis axis);
}
