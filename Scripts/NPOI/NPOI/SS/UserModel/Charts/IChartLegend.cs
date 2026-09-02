namespace NPOI.SS.UserModel.Charts;

public interface IChartLegend : ManuallyPositionable
{
	LegendPosition Position { get; set; }

	bool IsOverlay { get; set; }
}
