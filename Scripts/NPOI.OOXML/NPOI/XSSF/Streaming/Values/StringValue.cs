using NPOI.SS.UserModel;

namespace NPOI.XSSF.Streaming.Values;

public abstract class StringValue : Value
{
	public new CellType GetType()
	{
		return CellType.String;
	}

	public abstract bool IsRichText();
}
