using System.Collections.Generic;

namespace NPOI.SS.UserModel.Charts;

public interface IBarChartData<Tx, Ty> : IChartData
{
	IBarChartSeries<Tx, Ty> AddSeries(IChartDataSource<Tx> categories, IChartDataSource<Ty> values);

	List<IBarChartSeries<Tx, Ty>> GetSeries();
}
