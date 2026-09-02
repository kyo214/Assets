using NPOI.SS.UserModel.Charts;

namespace NPOI.XSSF.UserModel.Charts;

public class XSSFChartDataFactory : IChartDataFactory
{
	private static XSSFChartDataFactory instance;

	private XSSFChartDataFactory()
	{
	}

	public IScatterChartData<Tx, Ty> CreateScatterChartData<Tx, Ty>()
	{
		return new XSSFScatterChartData<Tx, Ty>();
	}

	public ILineChartData<Tx, Ty> CreateLineChartData<Tx, Ty>()
	{
		return new XSSFLineChartData<Tx, Ty>();
	}

	public IBarChartData<Tx, Ty> CreateBarChartData<Tx, Ty>()
	{
		return new XSSFBarChartData<Tx, Ty>();
	}

	public static XSSFChartDataFactory GetInstance()
	{
		if (instance == null)
		{
			instance = new XSSFChartDataFactory();
		}
		return instance;
	}
}
