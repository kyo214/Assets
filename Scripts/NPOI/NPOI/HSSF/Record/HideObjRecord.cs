using System.Text;
using NPOI.Util;

namespace NPOI.HSSF.Record;

public class HideObjRecord : StandardRecord
{
	public const short sid = 141;

	public const short HIDE_ALL = 2;

	public const short SHOW_PLACEHOLDERS = 1;

	public const short SHOW_ALL = 0;

	private short field_1_hide_obj;

	protected override int DataSize => 2;

	public override short Sid => 141;

	public HideObjRecord()
	{
	}

	public HideObjRecord(RecordInputStream in1)
	{
		field_1_hide_obj = in1.ReadShort();
	}

	public void SetHideObj(short hide)
	{
		field_1_hide_obj = hide;
	}

	public short GetHideObj()
	{
		return field_1_hide_obj;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("[HIDEOBJ]\n");
		stringBuilder.Append("    .hideobj         = ").Append(StringUtil.ToHexString(GetHideObj())).Append("\n");
		stringBuilder.Append("[/HIDEOBJ]\n");
		return stringBuilder.ToString();
	}

	public override void Serialize(ILittleEndianOutput out1)
	{
		out1.WriteShort(GetHideObj());
	}
}
