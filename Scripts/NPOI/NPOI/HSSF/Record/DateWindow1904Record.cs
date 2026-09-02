using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class DateWindow1904Record : StandardRecord
{
	public const short sid = 34;

	private short field_1_window;

	public short Windowing
	{
		get
		{
			return field_1_window;
		}
		set
		{
			field_1_window = value;
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 34;

	public DateWindow1904Record()
	{
	}

	public DateWindow1904Record(RecordInputStream in1)
	{
		field_1_window = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[1904]\n");
		stringBuilder.Append("    .is1904          = ").Append(StringUtil.ToHexString(Windowing)).Append("\n");
		stringBuilder.Append("[/1904]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(Windowing);
	}
}
