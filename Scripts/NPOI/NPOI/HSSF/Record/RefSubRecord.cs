using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class RefSubRecord
{
	public const int ENCODED_SIZE = 6;

	private int _extBookIndex;

	private int _firstSheetIndex;

	private int _lastSheetIndex;

	public int ExtBookIndex => _extBookIndex;

	public int FirstSheetIndex => _firstSheetIndex;

	public int LastSheetIndex => _lastSheetIndex;

	public void AdjustIndex(int offset)
	{
		_firstSheetIndex += offset;
		_lastSheetIndex += offset;
	}

	public RefSubRecord(int extBookIndex, int firstSheetIndex, int lastSheetIndex)
	{
		_extBookIndex = extBookIndex;
		_firstSheetIndex = firstSheetIndex;
		_lastSheetIndex = lastSheetIndex;
	}

	public RefSubRecord(RecordInputStream in1)
		: this(in1.ReadShort(), in1.ReadShort(), in1.ReadShort())
	{
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("extBook=").Append(_extBookIndex);
		stringBuilder.Append(" firstSheet=").Append(_firstSheetIndex);
		stringBuilder.Append(" lastSheet=").Append(_lastSheetIndex);
		return stringBuilder.ToString();
	}

	public void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(_extBookIndex);
		out1.WriteShort(_firstSheetIndex);
		out1.WriteShort(_lastSheetIndex);
	}
}
