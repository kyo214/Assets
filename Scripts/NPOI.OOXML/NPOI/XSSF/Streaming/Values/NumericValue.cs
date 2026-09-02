using NPOI.SS.UserModel;

namespace NPOI.XSSF.Streaming.Values;

public class NumericValue : Value
{
	public double Value;

	public new CellType GetType()
	{
		return CellType.Numeric;
	}
}
