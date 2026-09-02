using NPOI.SS.UserModel;

namespace NPOI.XSSF.Streaming.Values;

public class ErrorValue : Value
{
	public byte Value;

	public new CellType GetType()
	{
		return CellType.Error;
	}
}
