using System.Collections.Generic;

namespace NPOI.SS.UserModel.Charts;

public interface ILineChartData<Tx, Ty> : IChartData
{
	ILineChartSeries<Tx, Ty> AddSeries(IChartDataSource<Tx> categories, IChartDataSource<Ty> values);

	List<ILineChartSeries<Tx, Ty>> GetSeries();
}
