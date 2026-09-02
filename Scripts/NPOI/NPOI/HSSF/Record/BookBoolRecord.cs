using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class BookBoolRecord : StandardRecord
{
	public const short sid = 218;

	private short field_1_save_link_values;

	public short SaveLinkValues
	{
		get
		{
			return field_1_save_link_values;
		}
		set
		{
			field_1_save_link_values = value;
		}
	}

	protected override int DataSize => 2;

	public override short Sid => 218;

	public BookBoolRecord()
	{
	}

	public BookBoolRecord(RecordInputStream in1)
	{
		field_1_save_link_values = in1.ReadShort();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[BOOKBOOL]\n");
		stringBuilder.Append("    .savelinkvalues  = ").Append(StringUtil.ToHexString(SaveLinkValues)).Append("\n");
		stringBuilder.Append("[/BOOKBOOL]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(field_1_save_link_values);
	}
}
