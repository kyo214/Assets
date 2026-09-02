using System.Collections.Generic;

namespace NPOI.SS.UserModel.Charts;

public interface IScatterChartData<Tx, Ty> : IChartData
{
	IScatterChartSeries<Tx, Ty> AddSeries(IChartDataSource<Tx> xs, IChartDataSource<Ty> ys);

	List<IScatterChartSeries<Tx, Ty>> GetSeries();
}
