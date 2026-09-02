using System.Collections.Generic;
using NPOI.SS.UserModel.Charts;

namespace NPOI.SS.UserModel;

public interface IChart : ManuallyPositionable
{
	IChartDataFactory ChartDataFactory { get; }

	IChartAxisFactory ChartAxisFactory { get; }

	IChartLegend GetOrCreateLegend();

	void DeleteLegend();

	List<IChartAxis> GetAxis();

	void Plot(IChartData data, params IChartAxis[] axis);

	void SetTitle(string newTitle);
}
