namespace NPOI.SS.UserModel.Charts;

public interface IChartDataSource<T>
{
	int PointCount { get; }

	bool IsReference { get; }

	bool IsNumeric { get; }

	string FormulaString { get; }

	T GetPointAt(int index);
}
